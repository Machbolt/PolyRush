using UnityEngine;
using System.Collections;

public class AICarController : MonoBehaviour
{
    [Header("Stats")]
    public AICarStatsData stats;
    [SerializeField] string carName = "Car";

    [Header("Waypoint Navigation")]
    public WaypointManager waypointManager;
    public float thresholdMultiplier;

    [Header("Wheels")]
    public Transform centerOfMass;
    public WheelCollider frontLeftCollider, frontRightCollider, backLeftCollider, backRightCollider;
    public Transform frontLeft, frontRight, backLeft, backRight;

    private Rigidbody rb;
    private float adjustedMotorTorque;
    private float currentBrakeForce;
    private bool isBraking = false;
    private float mphSpeed => rb.velocity.magnitude * 2.237f;
    private int currentWaypointIndex = 0;
    private Transform currentWaypoint;
    private Vector3 waypointOffset;
    private Coroutine brakeCoroutine;

    private float nextSteerJitterTime;
    private float currentSteerJitter;

    private float stuckTimer = 0f;
    private int ghostLayer;
    private int aiCarLayer;
    public bool canMove = false;

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
        rb.centerOfMass = centerOfMass.localPosition;
        currentWaypoint = waypointManager.GetWaypoint(currentWaypointIndex);
        waypointOffset = GetRandomWaypointOffset();
        adjustedMotorTorque = stats.acceleration;
        ghostLayer = LayerMask.NameToLayer("Ghost");
        aiCarLayer = LayerMask.NameToLayer("AI Car");
    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            ApplyBraking();
            return;
        }
        ApplyDownforce();
        HandleTraction();
        NavigateToWaypoint();
        ApplyBraking();
        HandleStuckReset();
    }

    private void Update() => UpdateWheelVisuals();

    private void ApplyDownforce()
    {
        rb.AddForce(-transform.up * stats.downforce * rb.velocity.magnitude);
    }

    private void HandleTraction()
    {
        if (!TractionControlEnabled()) return;

        bool slipping = false;
        WheelHit hit;

        if (backLeftCollider.GetGroundHit(out hit) && Mathf.Abs(hit.forwardSlip) > stats.traction)
            slipping = true;
        if (backRightCollider.GetGroundHit(out hit) && Mathf.Abs(hit.forwardSlip) > stats.traction)
            slipping = true;

        float slipAmount = 0f;

        if (backLeftCollider.GetGroundHit(out hit))
            slipAmount += Mathf.Abs(hit.forwardSlip);
        if (backRightCollider.GetGroundHit(out hit))
            slipAmount += Mathf.Abs(hit.forwardSlip);

        slipAmount *= 0.5f; // average

        float gripFactor = Mathf.Clamp01(1f - (slipAmount / (stats.traction + 0.01f)));

        adjustedMotorTorque = stats.acceleration * gripFactor;
    }

    private bool TractionControlEnabled() => stats.traction > 0f;

    private void NavigateToWaypoint()
    {
        if (!currentWaypoint) return;

        Vector3 targetPos = currentWaypoint.position + waypointOffset;
        Vector3 localTarget = transform.InverseTransformPoint(targetPos);

        float distance = localTarget.magnitude;

        // === AI INPUTS (replace player input) ===
        float verticalInput = 1f; // always accelerating

        float horizontalInput = Mathf.Clamp(localTarget.x / distance, -1f, 1f);

        // Add slight human-like imperfection
        if (Time.time >= nextSteerJitterTime)
        {
            currentSteerJitter = Random.Range(-stats.steeringJitterStrength, stats.steeringJitterStrength);
            nextSteerJitterTime = Time.time + Random.Range(0.1f, 0.3f);
        }

        horizontalInput += currentSteerJitter;

        // === MATCH PLAYER CONTROLLER LOGIC ===
        float currentSpeed = mphSpeed;

        float steerAngle = horizontalInput * Mathf.Lerp(
            stats.handling,
            stats.handling / 2f,
            currentSpeed / stats.topSpeed
        );

        frontLeftCollider.steerAngle = steerAngle;
        frontRightCollider.steerAngle = steerAngle;

        // Throttle (same as player)
        if (currentSpeed < stats.topSpeed)
        {
            float torque = verticalInput * stats.acceleration;
            backLeftCollider.motorTorque = torque;
            backRightCollider.motorTorque = torque;
        }
        else
        {
            backLeftCollider.motorTorque = 0f;
            backRightCollider.motorTorque = 0f;
        }

        // Simple braking logic (like player pressing space)
        float turnSharpness = Mathf.Abs(horizontalInput);
        isBraking = turnSharpness > 0.6f;

        float adjustedThreshold = stats.waypointThreshold * thresholdMultiplier;

        if (Vector3.Distance(transform.position, currentWaypoint.position) < adjustedThreshold)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypointManager.WaypointCount;
            currentWaypoint = waypointManager.GetWaypoint(currentWaypointIndex);
            waypointOffset = GetRandomWaypointOffset();
        }
    }

    private void ApplyBraking()
    {
        float brakeForce = isBraking ? stats.brakingPower : 0f;

        frontLeftCollider.brakeTorque = brakeForce * 0.5f;
        frontRightCollider.brakeTorque = brakeForce * 0.5f;
        backLeftCollider.brakeTorque = brakeForce;
        backRightCollider.brakeTorque = brakeForce;
    }

    private void UpdateWheelVisuals()
    {
        UpdateWheel(frontLeftCollider, frontLeft);
        UpdateWheel(frontRightCollider, frontRight, true);
        UpdateWheel(backLeftCollider, backLeft);
        UpdateWheel(backRightCollider, backRight, true);
    }

    private void UpdateWheel(WheelCollider collider, Transform wheelTransform, bool flipY = false)
    {
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot * (flipY ? Quaternion.Euler(0, 180, 0) : Quaternion.identity);
    }

    private Vector3 GetRandomWaypointOffset()
    {
        return new Vector3(
            Random.Range(-stats.waypointOffsetRange, stats.waypointOffsetRange),
            0f,
            Random.Range(-stats.waypointOffsetRange, stats.waypointOffsetRange)
        );
    }

    private void HandleStuckReset()
    {
        if (mphSpeed < stats.minSpeed)
        {
            stuckTimer += Time.fixedDeltaTime;
            if (stuckTimer >= stats.stuckThreshold)
            {
                ResetCar();
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }
    }

    private void ResetCar()
    {
        Vector3 basePos = currentWaypoint.position + Vector3.up * 1.5f;
        int nextIndex = (currentWaypointIndex + 1) % waypointManager.WaypointCount;
        Vector3 forwardDir = (waypointManager.GetWaypoint(nextIndex).position - currentWaypoint.position).normalized;

        Vector3 sideOffset = Vector3.Cross(Vector3.up, forwardDir).normalized;
        float offsetAmount = Random.Range(-1.5f, 1.5f);
        Vector3 resetPos = basePos + sideOffset * offsetAmount;

        Quaternion resetRot = Quaternion.LookRotation(forwardDir, Vector3.up);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.SetPositionAndRotation(resetPos, resetRot);

        // Switch to Ghost layer temporarily
        SetLayerRecursively(gameObject, ghostLayer);

        // Start blinking + revert layer after 5 seconds
        StartCoroutine(GhostBlinkRoutine(5f));
    }

    private IEnumerator GhostBlinkRoutine(float duration)
    {
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

        // Restore AI Car layer
        SetLayerRecursively(gameObject, aiCarLayer);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BrakeZone"))
        {
            if (brakeCoroutine != null) StopCoroutine(brakeCoroutine);
            brakeCoroutine = StartCoroutine(DelayedBrake(true));
        }

        if (other.CompareTag("Plane"))
        {
            ResetCar();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("BrakeZone"))
        {
            if (brakeCoroutine != null) StopCoroutine(brakeCoroutine);
            brakeCoroutine = StartCoroutine(DelayedBrake(false));
        }
    }

    private IEnumerator DelayedBrake(bool braking)
    {
        float speedFactor = mphSpeed / stats.topSpeed;
        float dynamicDelay = stats.brakingDelay * Mathf.Lerp(0.5f, 1.5f, speedFactor);

        yield return new WaitForSeconds(Random.Range(dynamicDelay * 0.5f, dynamicDelay * 1.2f));
        isBraking = braking;
    }

    private IEnumerator RevertLayerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetLayerRecursively(gameObject, aiCarLayer);
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
