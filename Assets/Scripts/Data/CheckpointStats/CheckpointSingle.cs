using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{
    private TrackCheckpoints trackCheckpoints;
    
    // If car collides w/ checkpoint, CarThroughCheckpoint gets called, leading to checkpoint incrementing for that specific car
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CarController>(out CarController car) || other.TryGetComponent<AICarController>(out AICarController ai))
        {
            trackCheckpoints.CarThroughCheckpoint(this, other.transform);
        }
    }

    public void SetTrackCheckpoints(TrackCheckpoints trackCheckpoints)
    {
        this.trackCheckpoints = trackCheckpoints;
    }
}
