using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CarController : MonoBehaviour
{
    [Header("Car Configuration")]
    public CarStatsData stats;
    public bool canMove = false;
    [SerializeField] string carName = "Car";

    [Header("Physics")]
    public Transform centerOfMass;
    public WheelCollider frontLeftCollider, frontRightCollider, backLeftCollider, backRightCollider;
    public Transform frontLeft, frontRight, backLeft, backRight;

    [Header("Brake Lights")]
    public Light leftBrakeLight;
    public Light rightBrakeLight;

    private float brakeLightIntensity = 0.3f;
    private float idleLightIntensity = 0.1f;

    private Rigidbody rb;
    private bool isBraking;
    private float currentSpeed;
    public float CurrentSpeed => currentSpeed;
    private float mphSpeed => rb.velocity.magnitude * 2.237f;
    private float stuckTimer = 0f;
    public float stuckThreshold = 1.5f;
    public float minSpeed = 1f;

    private Text warningText;
    private Coroutine warningRoutine;

    private int defaultLayer;
    private bool isGhost = false;
    private Transform lastRespawnPoint;

    private void OnEnable()
    {
        RaceEvents.OnRaceStart += EnableMovement;
    }
    private void OnDisable()
    {
        RaceEvents.OnRaceStart -= EnableMovement;
    }
    private void EnableMovement()
    {
        canMove = true;
    }
    private void Awake()
    {
        gameObject.name = carName;
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.centerOfMass = centerOfMass.localPosition;
        lastRespawnPoint = transform;
        defaultLayer = gameObject.layer;
        GameObject wtObj = GameObject.Find("WarningText");
        if (wtObj != null)
        {
            warningText = wtObj.GetComponent<Text>();
            warningText.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (!canMove || PauseManager.isPaused)
        {
            if (backLeftCollider != null)
                backLeftCollider.motorTorque = 0f;
            if (backRightCollider != null)
                backRightCollider.motorTorque = 0f;
            if (stats != null)
                ApplyBraking(stats.brakeStrength);
            return;
        }
        if (!PauseManager.isPaused)
        {
            ApplyDownforce();
            HandleStuckReset();

            float verticalInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");

            currentSpeed = mphSpeed;

            float speedFactor = Mathf.Clamp01(currentSpeed / stats.topSpeed);

            // stronger steering reduction at speed
            float steerReduction = Mathf.Lerp(1f, 0.35f, speedFactor);

            float steerAngle = horizontalInput * stats.maxSteerAngle * steerReduction;
            frontLeftCollider.steerAngle = steerAngle;
            frontRightCollider.steerAngle = steerAngle;

            if (currentSpeed < stats.topSpeed)
            {
                float turnAmount = Mathf.Abs(horizontalInput);

                // reduce torque while turning
                float turnTorqueReduction = Mathf.Lerp(1f, 0.7f, turnAmount);

                float torque = verticalInput * stats.accelerationPower * turnTorqueReduction;
                backLeftCollider.motorTorque = torque;
                backRightCollider.motorTorque = torque;
            }
            else
            {
                backLeftCollider.motorTorque = 0f;
                backRightCollider.motorTorque = 0f;
            }

            isBraking = Input.GetKey(KeyCode.Space);
            float brakeForce = isBraking ? stats.brakeStrength : 0f;
            ApplyBraking(brakeForce);
            UpdateBrakeLights(isBraking);

            Vector3 pos;
            Quaternion rot;

            frontLeftCollider.GetWorldPose(out pos, out rot);
            frontLeft.position = pos;
            frontLeft.rotation = rot;

            frontRightCollider.GetWorldPose(out pos, out rot);
            frontRight.position = pos;
            frontRight.rotation = rot * Quaternion.Euler(0, 180, 0);

            backLeftCollider.GetWorldPose(out pos, out rot);
            backLeft.position = pos;
            backLeft.rotation = rot;

            backRightCollider.GetWorldPose(out pos, out rot);
            backRight.position = pos;
            backRight.rotation = rot * Quaternion.Euler(0, 180, 0);
        }
    }
    private void Update()
    {
        if (!PauseManager.isPaused && canMove)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetCar();
            }
        }
    }
    private void ApplyBraking(float force)
    {
        frontLeftCollider.brakeTorque = force * 0.5f;
        frontRightCollider.brakeTorque = force * 0.5f;
        backLeftCollider.brakeTorque = force;
        backRightCollider.brakeTorque = force;
    }

    private void ApplyDownforce()
    {
        float force = Mathf.Clamp(rb.velocity.magnitude * stats.downforceStrength, 0f, 10000f);
        rb.AddForce(-transform.up * force);
    }

    private void UpdateBrakeLights(bool braking)
    {
        float targetIntensity = braking ? brakeLightIntensity : idleLightIntensity;
        leftBrakeLight.intensity = targetIntensity;
        rightBrakeLight.intensity = targetIntensity;
    }

    private void HandleStuckReset()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        bool playerIsTryingToMove = Mathf.Abs(verticalInput) > 0.1f || Mathf.Abs(horizontalInput) > 0.1f;
        bool flippedOver = Vector3.Dot(transform.up, Vector3.up) < 0.3f;

        if (playerIsTryingToMove && rb.velocity.magnitude < minSpeed)
        {
            stuckTimer += Time.fixedDeltaTime;
        }
        else
        {
            stuckTimer = 0f;
        }

        if (stuckTimer >= stuckThreshold || flippedOver)
        {
            ResetCar();
            stuckTimer = 0f;
        }
    }

    private void ResetCar()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (lastRespawnPoint != null)
        {
            Vector3 resetPos = lastRespawnPoint.position + Vector3.up * 1f;
            Quaternion resetRot = Quaternion.Euler(0, lastRespawnPoint.eulerAngles.y, 0);
            transform.SetPositionAndRotation(resetPos, resetRot);
        }
        StartCoroutine(GhostBlinkRoutine(5f));
    }
    private IEnumerator ShowWarning()
    {
        if (warningText == null) yield break;

        float duration = 3f;
        float elapsed = 0f;
        warningText.enabled = true;

        while (elapsed < duration)
        {
            warningText.enabled = !warningText.enabled; // blink
            yield return new WaitForSeconds(0.5f);
            elapsed += 0.5f;
        }

        warningText.enabled = false;
    }

    private void TriggerWarning()
    {
        if (warningText == null) return;

        if (warningRoutine != null)
            StopCoroutine(warningRoutine);

        warningRoutine = StartCoroutine(ShowWarning());
    }
    private IEnumerator GhostBlinkRoutine(float duration)
    {
        if (isGhost) yield break; // already ghosting
        isGhost = true;

        // Switch to Ghost layer
        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Ghost"));

        float elapsed = 0f;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        while (elapsed < duration)
        {
            // Toggle visibility
            foreach (Renderer r in renderers)
                r.enabled = !r.enabled;

            yield return new WaitForSeconds(0.1f); // blink speed
            elapsed += 0.25f;
        }

        // Ensure visible at end
        foreach (Renderer r in renderers)
            r.enabled = true;

        // Restore default layer
        SetLayerRecursively(gameObject, defaultLayer);

        isGhost = false;
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RespawnPoint"))
        {
            lastRespawnPoint = other.transform;
        }

        if (other.CompareTag("Plane"))
        {
            ResetCar();
            TriggerWarning();
        }
    }
}
