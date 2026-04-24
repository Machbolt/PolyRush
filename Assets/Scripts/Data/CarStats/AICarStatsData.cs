using UnityEngine;

[CreateAssetMenu(fileName = "NewAICarStats", menuName = "Racing/AI Car Stats")]
public class AICarStatsData : ScriptableObject
{
    [Header("Performance")]
    public float topSpeed = 89.4f; // m/s (~200mph)
    public float acceleration = 15f;
    public float handling = 20f;
    public float brakingPower = 150f;
    public float traction = 0.001f;
    public float minBrakeSpeed = 10f;
    [Header("Physics")]
    public float downforce = 50f;
    public float tractionAdjustRate = 3f;

    [Header("Navigation Tuning")]
    public float waypointThreshold = 5f;
    public float steeringJitterStrength = 0.1f;
    public float waypointOffsetRange = 2f;
    public float brakingDelay = 0.2f;
    public float maxWaypointThreshold = 10f;

    [Header("Reset Settings")]
    public float stuckThreshold = 3f;
    public float minSpeed = 3f;
}