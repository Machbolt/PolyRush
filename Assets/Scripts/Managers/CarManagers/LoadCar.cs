using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCar : MonoBehaviour
{
    public GameObject[] carPrefabs;

    public CameraFollow cameraFollow;
    public RaceManager raceManager;
    private TrackCheckpoints trackCheckpoints;

    private void Start()
    {
        trackCheckpoints = TrackLoader.activeTrack.trackCheckpoints;
        int selectedCar = PlayerPrefs.GetInt("selectedCar");
        GameObject prefab = carPrefabs[selectedCar];
        Transform spawn = TrackLoader.activeTrack.playerSpawnPoint;
        GameObject clone = Instantiate(prefab, spawn.position, spawn.rotation);
        cameraFollow.InitializeWithCar(clone);
        raceManager.InitializeWithCar(clone.transform);
        trackCheckpoints.RegisterCar(clone.transform);
    }
}
