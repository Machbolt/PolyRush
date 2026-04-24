using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarSounds : MonoBehaviour
{
    [Header("Engine Sound")]
    public float minSpeed;
    public float maxSpeed;
    public float minPitch;
    public float maxPitch;

    private float currentSpeed;
    private float pitchFromCar;

    private Rigidbody carRb;
    private AudioSource engineAudio;

    [Header("Wall Slide Sound")]
    public AudioSource wallSlideAudio;
    public float slideThreshold = 1f;        // Minimum lateral speed to trigger slide
    public float fadeSpeed = 2f;             // Speed of fade-in/out
    private float targetWallSlideVolume = 0f;
    private float currentLateralSpeed = 0f;
    private bool wasPaused = false;
    void Start()
    {
        engineAudio = GetComponent<AudioSource>();
        carRb = GetComponent<Rigidbody>();

        if (wallSlideAudio != null)
        {
            wallSlideAudio.volume = 0f;
            wallSlideAudio.loop = true;
            wallSlideAudio.Play(); // Play silently and control volume
        }
    }

    void Update()
    {
        if (PauseManager.isPaused)
        {
            if (!wasPaused)
            {
                engineAudio.Pause();
                wallSlideAudio.Pause();
                wasPaused = true;
            }
        }
        else
        {
            if (wasPaused)
            {
                engineAudio.UnPause();
                wallSlideAudio.UnPause();
                wasPaused = false;
            }
            EngineSound();

            if (wallSlideAudio != null)
            {
                // Smoothly fade the volume
                wallSlideAudio.volume = Mathf.Lerp(wallSlideAudio.volume, targetWallSlideVolume, Time.deltaTime * fadeSpeed);

                // Update pitch based on lateral speed
                wallSlideAudio.pitch = 0.8f + 0.2f * Mathf.Clamp01(currentLateralSpeed / 10f);
            }
        }
    }

    void EngineSound()
    {
        currentSpeed = carRb.velocity.magnitude;
        pitchFromCar = currentSpeed / 60f;

        if (currentSpeed < minSpeed)
        {
            engineAudio.pitch = minPitch;
        }
        else if (currentSpeed < maxSpeed)
        {
            engineAudio.pitch = minPitch + pitchFromCar;
        }
        else
        {
            engineAudio.pitch = maxPitch;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Vector3 wallNormal = contact.normal;
                Vector3 velocity = carRb.velocity;

                // Lateral velocity: sliding along the wall
                Vector3 lateralVelocity = velocity - Vector3.Project(velocity, wallNormal);
                float lateralSpeed = lateralVelocity.magnitude;

                if (lateralSpeed > slideThreshold)
                {
                    targetWallSlideVolume = 1f;
                    currentLateralSpeed = lateralSpeed; // Store for pitch calculation
                    return;
                }
            }
        }

        // Not sliding enough — fade out
        targetWallSlideVolume = 0f;
        currentLateralSpeed = 0f;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            targetWallSlideVolume = 0f;
            currentLateralSpeed = 0f;
        }
    }
}
