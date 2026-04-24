using UnityEngine;

[CreateAssetMenu(menuName = "Racing/Track Data")]
public class TrackData : ScriptableObject
{
    public int[] positionRewards = { 2000, 1500, 1000, 500 };
    public int lastPlaceReward = 250;
    public float waypointThresholdMultiplier = 1f;
}