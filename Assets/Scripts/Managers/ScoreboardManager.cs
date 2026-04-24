using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardManager : MonoBehaviour
{
    [Header("References")]
    private TrackCheckpoints trackCheckpoints;
    public GameObject entryPrefab; // The prefab with PositionText, CarNameText, LapTimesText
    public Transform contentParent; // ScrollView Content transform
    public GameObject scoreboardPanel;
    public int totalLaps = 2; // Set this to the race's total laps, or get from RaceManager
    public Button nextButton;

    private bool nextButtonClicked = false;
    private List<Transform> cars;
    private Dictionary<Transform, ScoreboardEntry> entries = new Dictionary<Transform, ScoreboardEntry>();
    private HashSet<Transform> finishedCars = new HashSet<Transform>();
    private List<Transform> finalPlacements = new List<Transform>();

    void NextButtonClicked()
    {
        nextButtonClicked = true;
    }
    private void Awake()
    {
        trackCheckpoints = TrackLoader.activeTrack.trackCheckpoints;
    }
    void Start()
    {
        StartCoroutine(WaitAndInit());
    }

    IEnumerator WaitAndInit()
    {
        // Wait 1 frame or until cars are registered
        yield return new WaitForEndOfFrame();

        if (trackCheckpoints == null)
            trackCheckpoints = FindObjectOfType<TrackCheckpoints>();

        while (trackCheckpoints.GetAllCars().Count == 0)
            yield return null;

        InitializeScoreboard();
    }


    void InitializeScoreboard()
    {
        cars = trackCheckpoints.GetAllCars();

        foreach (Transform car in cars)
        {
            GameObject entryGO = Instantiate(entryPrefab, contentParent);
            Text[] texts = entryGO.GetComponentsInChildren<Text>();

            Text positionText = null;
            Text carNameText = null;
            Text lapTimesText = null;

            foreach (Text t in texts)
            {
                string lower = t.name.ToLower();
                if (lower.Contains("position")) positionText = t;
                else if (lower.Contains("carname")) carNameText = t;
                else if (lower.Contains("laptimes")) lapTimesText = t;
            }

            if (carNameText != null)
                carNameText.text = car.name;

            entries[car] = new ScoreboardEntry
            {
                entryObject = entryGO,
                positionText = positionText,
                carNameText = carNameText,
                lapTimesText = lapTimesText
            };
        }
    }
    void Update()
    {
        nextButton.onClick.AddListener(NextButtonClicked);
        if (cars == null || cars.Count == 0) return;

        // 1. Check for new finishers
        foreach (Transform car in cars)
        {
            if (finishedCars.Contains(car)) continue;

            int lapCount = trackCheckpoints.GetLapCount(car);
            if (lapCount >= totalLaps)
            {
                finishedCars.Add(car);
                finalPlacements.Add(car);
            }
        }

        // 2. Gather live (unfinished) car standings
        List<CarStanding> unfinishedStandings = new List<CarStanding>();
        foreach (Transform car in cars)
        {
            if (finishedCars.Contains(car)) continue;

            int position = trackCheckpoints.GetCarPosition(car);
            List<float> lapTimes = trackCheckpoints.GetLapTimes(car);

            unfinishedStandings.Add(new CarStanding
            {
                car = car,
                position = position,
                lapTimes = lapTimes
            });
        }

        // Sort unfinished cars by live position
        unfinishedStandings.Sort((a, b) => a.position.CompareTo(b.position));

        // 3. Build full list: finished cars in order, then sorted unfinished
        List<Transform> orderedCars = new List<Transform>();
        orderedCars.AddRange(finalPlacements);
        foreach (var standing in unfinishedStandings)
            orderedCars.Add(standing.car);

        // 4. Update UI entries and sibling index
        for (int i = 0; i < orderedCars.Count; i++)
        {
            Transform car = orderedCars[i];
            if (!entries.ContainsKey(car)) continue;

            ScoreboardEntry entry = entries[car];

            int finalPos = i + 1;
            if (entry.positionText != null)
                entry.positionText.text = $"#{finalPos}";

            List<float> lapTimes = trackCheckpoints.GetLapTimes(car);
            // Always update lap times until they match totalLaps
            if (lapTimes.Count <= totalLaps)
            {
                entry.lapTimesText.text = FormatLapTimes(lapTimes);
            }
            entry.entryObject.transform.SetSiblingIndex(i);
        }
        Transform playerCar = FindPlayerCar(); // or store this reference earlier
        if (trackCheckpoints.GetLapCount(playerCar) >= totalLaps && !nextButtonClicked)
        {
            if (!scoreboardPanel.activeSelf)
            {
                scoreboardPanel.SetActive(true);
            }
        }
    }
    string FormatLapTimes(List<float> lapTimes)
    {
        if (lapTimes == null || lapTimes.Count == 0) return "--";

        string result = "";
        for (int i = 0; i < lapTimes.Count; i++)
        {
            result += $"Lap {i + 1}: {FormatTime(lapTimes[i])}\n";
        }

        return result.Trim();
    }
    string FormatTime(float timeSeconds)
    {
        int minutes = Mathf.FloorToInt(timeSeconds / 60f);
        float seconds = timeSeconds % 60f;
        return $"{minutes:00}:{seconds:00.00}";
    }
    Transform FindPlayerCar()
    {
        foreach (Transform car in cars)
        {
            if (car.CompareTag("Player"))
                return car;
        }
        return null;
    }
    private class ScoreboardEntry
    {
        public GameObject entryObject;
        public Text positionText;
        public Text carNameText;
        public Text lapTimesText;
    }

    private class CarStanding
    {
        public Transform car;
        public int position;
        public List<float> lapTimes;
    }
}
