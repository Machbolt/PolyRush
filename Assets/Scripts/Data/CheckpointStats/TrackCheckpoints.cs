using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{
    [SerializeField] List<Transform> carTransformList;
    private List<CheckpointSingle> checkpointSingleList;
    private List<int> lapCounts;
    private List<List<float>> carLapTimes; // Each car has a list of its lap times
    private List<float> currentLapStartTime; // When the current lap started for each car
    private List<int> nextCheckpointSingleIndexList; // stores each car's progress
    private List<bool> hasRaceStarted;
    private List<bool> hasCompletedFirstLap;
    private List<int> lastCheckpointSingleIndexList;

    public void RegisterCar(Transform carTransform)
    {
        carTransformList.Add(carTransform);
        nextCheckpointSingleIndexList.Add(0);
        lastCheckpointSingleIndexList.Add(-1);
        carLapTimes.Add(new List<float>());
        currentLapStartTime.Add(Time.time);
        hasRaceStarted.Add(false);
        hasCompletedFirstLap.Add(false);
        lapCounts.Add(0);
    }

    private void Awake()
    {
        Transform trackTransform = GetComponent<Transform>();

        checkpointSingleList = new List<CheckpointSingle>();
        lapCounts = new List<int>();
        carLapTimes = new List<List<float>>();
        currentLapStartTime = new List<float>();
        hasRaceStarted = new List<bool>();
        hasCompletedFirstLap = new List<bool>();
        lastCheckpointSingleIndexList = new List<int>();
        // For every child transform of parent game object (Checkpoints)
        foreach (Transform checkpointSingleTransform in trackTransform)
        {
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();

            // For every child object, set the track checkpoints to parent object (Checkpoints)
            checkpointSingle.SetTrackCheckpoints(this);

            // Add the CheckpointSingle component in each child to CheckpointSingle list
            checkpointSingleList.Add(checkpointSingle);

            // lapCounts.Add(0); // start at lap 0
        }

        // For each car, start checkpoint index = 0
        nextCheckpointSingleIndexList = new List<int>();
    }

    /* 
        This method is called whenever a car goes through a checkpoint. Its job is to determine:
        1. Whether the car hit the correct next checkpoint.
        2. If yes, advance its progress to the next checkpoint.
        3. If not, log that it hit the wrong one. 
     */
    public void CarThroughCheckpoint(CheckpointSingle checkpointSingle, Transform carTransform)
    {
        int carIndex = carTransformList.IndexOf(carTransform);
        int checkpointIndex = checkpointSingleList.IndexOf(checkpointSingle);
        int nextCheckpointSingleIndex = nextCheckpointSingleIndexList[carIndex];

        // START/FINISH line assumed to be checkpoint 0
        bool isStartFinishLine = checkpointIndex == 0;

        if (checkpointIndex == nextCheckpointSingleIndex)
        {
            lastCheckpointSingleIndexList[carIndex] = checkpointIndex;
            nextCheckpointSingleIndexList[carIndex] = (nextCheckpointSingleIndex + 1) % checkpointSingleList.Count;

            if (isStartFinishLine)
            {
                if (!hasRaceStarted[carIndex])
                {
                    // Start race
                    hasRaceStarted[carIndex] = true;
                    currentLapStartTime[carIndex] = Time.time;
                }
                else if (hasCompletedFirstLap[carIndex])
                {
                    float currentTime = Time.time;
                    float lapTime = currentTime - currentLapStartTime[carIndex];
                    carLapTimes[carIndex].Add(lapTime);
                    lapCounts[carIndex]++;
                    currentLapStartTime[carIndex] = currentTime;
                }
                else
                {
                    // First time returning to start after full lap
                    float currentTime = Time.time;
                    float lapTime = currentTime - currentLapStartTime[carIndex];
                    carLapTimes[carIndex].Add(lapTime);
                    hasCompletedFirstLap[carIndex] = true;
                    lapCounts[carIndex] = 1;
                    currentLapStartTime[carIndex] = currentTime;
                }
            }
        }
        else
        {
            Debug.Log($"{carTransform.name} - Wrong");
        }
    }

    public int GetCarPosition(Transform carTransform)
    {
        List<CarProgress> progressList = new List<CarProgress>();

        for (int i = 0; i < carTransformList.Count; i++)
        {
            Transform car = carTransformList[i];
            int lastCheckpointIndex = lastCheckpointSingleIndexList[i];
            int lap = lapCounts[i];
            int progressScore = lap * checkpointSingleList.Count + lastCheckpointIndex;

            int nextCheckpointIndex = nextCheckpointSingleIndexList[i];
            float distanceToNext = Vector3.Distance(car.position, checkpointSingleList[nextCheckpointIndex].transform.position);

            progressList.Add(new CarProgress
            {
                car = car,
                progressScore = progressScore,
                distanceToNextCheckpoint = distanceToNext
            });
        }

        progressList.Sort((a, b) =>
        {
            int progressCompare = b.progressScore.CompareTo(a.progressScore);
            if (progressCompare == 0)
                return a.distanceToNextCheckpoint.CompareTo(b.distanceToNextCheckpoint);
            return progressCompare;
        });

        for (int i = 0; i < progressList.Count; i++)
        {
            if (progressList[i].car == carTransform)
            {
                return i + 1;
            }
        }

        return -1;
    }

    public int GetCarCount()
    {
        return carTransformList.Count;
    }

    public float GetCurrentLapTime(Transform carTransform)
    {
        int carIndex = carTransformList.IndexOf(carTransform);
        if (!hasRaceStarted[carIndex]) return 0f;
        return Time.time - currentLapStartTime[carIndex];
    }

    public float GetBestLapTime(Transform carTransform)
    {
        int carIndex = carTransformList.IndexOf(carTransform);
        if (carLapTimes[carIndex].Count == 0) return 0f;
        return Mathf.Min(carLapTimes[carIndex].ToArray());
    }

    public int GetLapCount(Transform carTransform)
    {
        int carIndex = carTransformList.IndexOf(carTransform);
        return lapCounts[carIndex];
    }
    public List<Transform> GetAllCars()
    {
        return carTransformList;
    }
    public List<float> GetLapTimes(Transform car)
    {
        int carIndex = carTransformList.IndexOf(car);
        if (carIndex < 0 || carIndex >= carLapTimes.Count) return new List<float>();
        return carLapTimes[carIndex];
    }
    private class CarProgress
    {
        public Transform car;
        public int progressScore;
        public float distanceToNextCheckpoint;
    }
}