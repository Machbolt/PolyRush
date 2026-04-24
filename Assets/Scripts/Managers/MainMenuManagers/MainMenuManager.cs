using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static bool loadCarSelect = false;
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject carSelectionPanel;
    public GameObject carUI;
    // public GameObject optionsPanel;

    public MainMenuCameraTransition cameraTransition;

    void Start()
    {
        if (loadCarSelect)
        {
            loadCarSelect = false; // reset
            ShowCarSelection();
        }
        else
        {
            ShowMainMenu();
        }
    }

    public void ShowMainMenu()
    {
        carSelectionPanel.SetActive(false);
        carUI.SetActive(false);
        cameraTransition.StartCameraTransitionBack();
        // optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void ShowCarSelection()
    {
        mainMenuPanel.SetActive(false);
        // optionsPanel.SetActive(false);
        carSelectionPanel.SetActive(true);
        carUI.SetActive(true);
        cameraTransition.StartCameraTransition();
    }

    public void ShowOptions()
    {
        mainMenuPanel.SetActive(false);
        carSelectionPanel.SetActive(false);
        carUI.SetActive(false);
        // optionsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void SelectTrack(int index)
    {
        TrackSelection.selectedTrackIndex = index;
        // SceneManager.LoadScene("RaceScene");
    }
}
