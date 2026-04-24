using System.Collections.Generic;
using UnityEngine;

public class AICarSpawner : MonoBehaviour
{
    public List<GameObject> carPrefabs;
    private List<Transform> spawnPoints;
    private TrackCheckpoints trackCheckpoints;
    private WaypointManager waypointManager;
    private float trackThresholdMultiplier;

    void Start()
    {
        spawnPoints = TrackLoader.activeTrack.spawnPoints;
        trackCheckpoints = TrackLoader.activeTrack.trackCheckpoints;
        waypointManager = TrackLoader.activeTrack.waypointManager;
        trackThresholdMultiplier = TrackLoader.activeTrack.trackData.waypointThresholdMultiplier;
        SpawnCars();
    }

    void SpawnCars()
    {
        List<Transform> shuffledSpawnPoints = new List<Transform>(spawnPoints);
        Shuffle(shuffledSpawnPoints);

        for (int i = 0; i < shuffledSpawnPoints.Count; i++)
        {
            GameObject carPrefab = carPrefabs[Random.Range(0, carPrefabs.Count)];
            Transform spawnPoint = shuffledSpawnPoints[i];
            GameObject car = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);

            // Assign WaypointManager to AI
            AICarController aiController = car.GetComponent<AICarController>();
            if (aiController != null)
            {
                aiController.waypointManager = waypointManager;
                aiController.thresholdMultiplier = trackThresholdMultiplier;
            }

            // Activate one random body color
            ActivateRandomBodyColor(car);

            // Register car for race tracking
            trackCheckpoints.RegisterCar(car.transform);
        }
    }

    void ActivateRandomBodyColor(GameObject car)
    {
        // Find BodyColors child object
        Transform bodyColorsParent = car.transform.Find("BodyColors");
        if (bodyColorsParent == null)
        {
            Debug.LogWarning($"No 'BodyColors' object found in car prefab: {car.name}");
            return;
        }

        // Collect all child color GameObjects
        List<Transform> colorOptions = new List<Transform>();
        foreach (Transform child in bodyColorsParent)
        {
            colorOptions.Add(child);
        }

        if (colorOptions.Count == 0)
        {
            Debug.LogWarning($"No color options under 'BodyColors' in car prefab: {car.name}");
            return;
        }

        // Pick one at random and activate it
        int chosenIndex = Random.Range(0, colorOptions.Count);
        colorOptions[chosenIndex].gameObject.SetActive(true);
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
