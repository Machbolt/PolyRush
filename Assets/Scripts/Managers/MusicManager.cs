using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] musicTracks;  // Assign in Inspector
    private AudioSource audioSource;
    private Queue<AudioClip> shuffledQueue;
    private bool wasPaused = false;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Shuffle and store tracks
        List<AudioClip> trackList = new List<AudioClip>(musicTracks);
        Shuffle(trackList);
        shuffledQueue = new Queue<AudioClip>(trackList);

        PlayNextTrack();
    }

    void Update()
    {
        if (PauseManager.isPaused)
        {
            if (!wasPaused)
            {
                audioSource.Pause();
                wasPaused = true;
            }
        }
        else
        {
            if (wasPaused)
            {
                audioSource.UnPause();
                wasPaused = false;
            }

            if (!audioSource.isPlaying && shuffledQueue.Count > 0)
            {
                PlayNextTrack();
            }
        }
    }

    void PlayNextTrack()
    {
        if (shuffledQueue.Count == 0) return;

        AudioClip nextTrack = shuffledQueue.Dequeue();
        audioSource.clip = nextTrack;
        audioSource.Play();
    }

    // Fisher-Yates shuffle
    void Shuffle(List<AudioClip> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            AudioClip temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
