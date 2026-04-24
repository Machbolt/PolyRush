using UnityEngine;

[CreateAssetMenu(fileName = "NewCarStats", menuName = "Racing/Car Stats")]
public class CarStatsData : ScriptableObject
{
    [Header("Upgrade Stats")]
    public float topSpeed = 320f;
    public float accelerationPower = 2000f;
    public float maxSteerAngle = 25f;
    public float brakeStrength = 8000f;

    [Header("Physics")]
    public float downforceStrength = 50f;

    [Header("Camera Settings")]
    public CarCameraSettings cameraSettings;

    public float PerformanceRating
    {
        get
        {
            return (topSpeed / 10f) + (accelerationPower / 50f) + (maxSteerAngle * 4f) + (brakeStrength / 50f);
        }
    }
}
