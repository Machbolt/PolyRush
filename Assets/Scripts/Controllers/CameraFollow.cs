using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform carTarget;
    private Vector3 velocity = Vector3.zero;
    private CarCameraSettings cameraSettings;

    public void InitializeWithCar(GameObject car)
    {
        carTarget = car.transform;
        LoadCameraSettingsFromCar();
    }

    void LateUpdate()
    {
        if (!carTarget || cameraSettings == null) return;
        FollowTarget();
    }

    void LoadCameraSettingsFromCar()
    {
        if (!carTarget) return;

        var carController = carTarget.GetComponent<CarController>();
        if (carController != null && carController.stats != null)
        {
            cameraSettings = carController.stats.cameraSettings;
        }
        else
        {
            Debug.LogWarning("CameraFollow: Could not find CarCameraSettings from CarStatsData.");
        }
    }

    void FollowTarget()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        Vector3 targetPos = carTarget.TransformPoint(cameraSettings.moveOffset);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 1f / cameraSettings.moveSmoothness);
    }


    void HandleRotation()
    {
        var direction = carTarget.position - transform.position;
        var rotation = new Quaternion();

        rotation = Quaternion.LookRotation(direction + cameraSettings.rotOffset, Vector3.up);

        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, cameraSettings.rotSmoothness * Time.deltaTime);
    }
}
