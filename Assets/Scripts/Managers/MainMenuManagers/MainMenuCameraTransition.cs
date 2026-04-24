using UnityEngine;
using System.Collections;

public class MainMenuCameraTransition : MonoBehaviour
{
    [Header("References")]
    public Transform transitionTarget; // Starting target (initial car view)
    public Transform originalPos;      // Menu view
    public MainMenuCamera orbitCameraScript;

    [Header("Transition Settings")]
    public float transitionDuration = 2f;
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Coroutine transitionCoroutine;
    private bool isTransitioning = false;

    // We'll remember the last camera orbit position
    private Vector3 savedOrbitPos;
    private Quaternion savedOrbitRot;

    void Start()
    {
        if (orbitCameraScript != null)
            orbitCameraScript.enabled = false;

        // Initialize saved orbit position to the transition target
        if (transitionTarget != null)
        {
            savedOrbitPos = transitionTarget.position;
            savedOrbitRot = transitionTarget.rotation;
        }
    }

    public void StartCameraTransition()
    {
        // Move to the *saved* car view position
        StartTransitionToSavedOrbit();
    }

    public void StartCameraTransitionBack()
    {
        // Save the camera's current orbit before leaving
        if (orbitCameraScript != null && orbitCameraScript.enabled)
        {
            savedOrbitPos = transform.position;
            savedOrbitRot = transform.rotation;
        }

        StartTransition(originalPos, enableOrbitAfter: false);
    }

    private void StartTransitionToSavedOrbit()
    {
        // Move to the last saved orbit view
        Transform tempTarget = new GameObject("TempOrbitTarget").transform;
        tempTarget.position = savedOrbitPos;
        tempTarget.rotation = savedOrbitRot;

        StartTransition(tempTarget, enableOrbitAfter: true);

        // Clean up temporary object after the transition
        Destroy(tempTarget.gameObject, transitionDuration + 0.1f);
    }

    private void StartTransition(Transform target, bool enableOrbitAfter)
    {
        if (target == null) return;

        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        transitionCoroutine = StartCoroutine(MoveToTarget(target, enableOrbitAfter));
    }

    private IEnumerator MoveToTarget(Transform target, bool enableOrbitAfter)
    {
        isTransitioning = true;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        float elapsed = 0f;

        if (orbitCameraScript != null)
            orbitCameraScript.enabled = false;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = easeCurve.Evaluate(elapsed / transitionDuration);

            transform.position = Vector3.Lerp(startPos, target.position, t);
            transform.rotation = Quaternion.Slerp(startRot, target.rotation, t);

            yield return null;
        }

        transform.position = target.position;
        transform.rotation = target.rotation;

        if (orbitCameraScript != null)
            orbitCameraScript.enabled = enableOrbitAfter;

        isTransitioning = false;
        transitionCoroutine = null;
    }
}
