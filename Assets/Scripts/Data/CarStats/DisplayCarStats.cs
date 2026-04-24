using UnityEngine;
using UnityEngine.UI;

public class DisplayCarStats : MonoBehaviour
{
    [Header("Reference to Car Stats")]
    public CarStatsData carStats;
    public Text topSpeedText, accelerationText, handlingText, brakeForceText, totalPRText;

    private void Update()
    {
        topSpeedText.text = $"Top Speed: {GetTopSpeed():F2} mph";
        accelerationText.text = $"Acceleration: {GetAccelerationPower():F2}";
        handlingText.text = $"Handling: {GetMaxSteerAngle():F2}";
        brakeForceText.text = $"Brake Force: {GetBrakeStrength():F2}";
        totalPRText.text = $"Total PR: {GetPerformanceRating():F2}";

    }

    public float GetTopSpeed()
    {
        return carStats != null ? carStats.topSpeed : 0f;
    }

    public float GetAccelerationPower()
    {
        return carStats != null ? carStats.accelerationPower : 0f;
    }

    public float GetMaxSteerAngle()
    {
        return carStats != null ? carStats.maxSteerAngle : 0f;
    }

    public float GetBrakeStrength()
    {
        return carStats != null ? carStats.brakeStrength : 0f;
    }

    public float GetPerformanceRating()
    {
        return carStats != null ? carStats.PerformanceRating : 0f;
    }
}
