using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();

    private void Awake()
    {
        // Automatically grab all children as waypoints
        foreach (Transform child in transform)
        {
            waypoints.Add(child);
        }
    }

    public Transform GetNextWaypoint(int currentIndex)
    {
        if (waypoints.Count == 0) return null;
        return waypoints[(currentIndex + 1) % waypoints.Count];
    }

    public Transform GetWaypoint(int index)
    {
        if (waypoints.Count == 0 || index < 0 || index >= waypoints.Count) return null;
        return waypoints[index];
    }

    public int WaypointCount => waypoints.Count;
}