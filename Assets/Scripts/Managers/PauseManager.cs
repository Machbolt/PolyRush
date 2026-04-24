using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public static bool isPaused = false;
    public Slider masterVol, musicVol, sfxVol;
    public AudioMixer mainAudioMixer;
    void Start()
    {
        pauseMenu.SetActive(false);
        float value;
        if (mainAudioMixer.GetFloat("MasterVol", out value)) masterVol.value = value;
        if (mainAudioMixer.GetFloat("MusicVol", out value)) musicVol.value = value;
        if (mainAudioMixer.GetFloat("SFXVol", out value)) sfxVol.value = value;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (!isPaused) { pauseMenu.SetActive(true); }
        Time.timeScale = 0f;
        isPaused = true;
    }
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
        isPaused = false;
    }
    public void QuitGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        MainMenuManager.loadCarSelect = true;
        SceneManager.LoadScene("MainMenu");
    }
    public void ChangeMasterVolume()
    {
        mainAudioMixer.SetFloat("MasterVol", masterVol.value);
    }
    public void ChangeMusicVolume()
    {
        mainAudioMixer.SetFloat("MusicVol", musicVol.value);

        if (musicVol.value <= -10f)
        {
            mainAudioMixer.SetFloat("MusicVol", -80f);
        }
    }
    public void ChangeSFXVolume()
    {
        mainAudioMixer.SetFloat("SFXVol", sfxVol.value);
    }
}
