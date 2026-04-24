using UnityEngine;
using System.Collections.Generic;

public class TrackRoot : MonoBehaviour
{
    public TrackCheckpoints trackCheckpoints;
    public WaypointManager waypointManager;
    public List<Transform> spawnPoints;
    public Transform playerSpawnPoint;
    public TrackData trackData;
}
