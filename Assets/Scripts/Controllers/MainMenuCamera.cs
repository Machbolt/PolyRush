using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuCamera : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;   // The car
    public float distance = 5f;

    [Header("Orbit Settings")]
    public float rotationSpeed = 5f;
    public float yMinLimit = -20f; // Vertical angle min
    public float yMaxLimit = 80f;  // Vertical angle max

    private float xRotation = 0f;
    private float yRotation = 20f;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("CarOrbitCamera: No target assigned!");
            return;
        }

        // Initialize rotation based on current camera position
        Vector3 angles = transform.eulerAngles;
        xRotation = angles.y;
        yRotation = angles.x;
    }

    void LateUpdate()
    {
        if (target == null) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;
        if (Input.GetMouseButton(0)) // Left click + hold
        {
            xRotation += Input.GetAxis("Mouse X") * rotationSpeed;
            yRotation -= Input.GetAxis("Mouse Y") * rotationSpeed;
            yRotation = Mathf.Clamp(yRotation, yMinLimit, yMaxLimit);
        }

        // Convert rotations into position around car
        Quaternion rotation = Quaternion.Euler(yRotation, xRotation, 0);
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + target.position;

        transform.rotation = rotation;
        transform.position = position;
    }
}
