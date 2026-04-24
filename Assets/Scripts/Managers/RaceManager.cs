using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceManager : MonoBehaviour
{
    private TrackData trackData;

    private Transform playerCarTransform;
    private bool raceEnded = false;

    [Header("References")]
    private TrackCheckpoints trackCheckpoints;
    public Text positionText;
    public Text currentLapTimeText;
    public Text bestLapTimeText;
    public Text lapCountText;
    public int totalLaps = 2;
    public GameObject raceUI;
    public Text speedometerText;
    public GameObject rewardUI;
    public Text cashGainedText;
    public Text finalPositionText;
    public Text countdownText;

    private int reward = 0;
    private CarController playerCarController;
    private int totalCars;
    private void Awake()
    {
        trackCheckpoints = TrackLoader.activeTrack.trackCheckpoints;
        trackData = TrackLoader.activeTrack.trackData;
    }
    public void InitializeWithCar(Transform car)
    {
        playerCarTransform = car;
        playerCarController = car.GetComponent<CarController>();
    }
    private void Start()
    {
        StartCoroutine(RaceCountdown());
        if (trackCheckpoints == null)
        {
            trackCheckpoints = FindObjectOfType<TrackCheckpoints>();
        }

        totalCars = trackCheckpoints.GetCarCount();
    }

    private void Update()
    {
        if (playerCarTransform == null || trackCheckpoints == null || positionText == null || PauseManager.isPaused) return;

        float currentLapTime = trackCheckpoints.GetCurrentLapTime(playerCarTransform);
        float bestLapTime = 0f;
        int lapCount = trackCheckpoints.GetLapCount(playerCarTransform);
        if (lapCount < 1)
        {
            bestLapTime = currentLapTime;
        }
        else
        {
            bestLapTime = trackCheckpoints.GetBestLapTime(playerCarTransform);
        }

        currentLapTimeText.text = $"Lap Time: {currentLapTime:F2}s";
        bestLapTimeText.text = $"Best: {bestLapTime:F2}s";
        lapCountText.text = $"Current Lap: {lapCount + 1}";

        int position = trackCheckpoints.GetCarPosition(playerCarTransform);
        positionText.text = $"Position: {position} / {totalCars}";

        if (lapCount >= totalLaps && !raceEnded)
        {
            raceEnded = true;
            raceUI.SetActive(false);
            int finishPosition = trackCheckpoints.GetCarPosition(playerCarTransform);

            // Calculate reward
            reward = (finishPosition <= trackData.positionRewards.Length)
                ? trackData.positionRewards[finishPosition - 1]
                : trackData.lastPlaceReward;

            // Add to currency
            CurrencyManager.Instance.AddMoney(reward);
            cashGainedText.text = "Awarded " + reward + " cash!";
            finalPositionText.text = "Final position: " + finishPosition;
            playerCarController.canMove = false;
            Debug.Log($"Player finished in position {finishPosition}. Awarded {reward} coins!");
        }
        float speed = playerCarController.CurrentSpeed;
        speedometerText.text = $"{speed:F0} mph";
    }
    private IEnumerator RaceCountdown()
    {
        int countdown = 3;
        while (countdown > 0)
        {
            countdownText.text = countdown.ToString();
            RaceEvents.OnRaceCountdown?.Invoke();
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        countdownText.text = "GO!";
        RaceEvents.OnRaceStart?.Invoke();
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);
    }
}
