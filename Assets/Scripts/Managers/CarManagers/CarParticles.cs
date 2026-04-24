using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarParticles : MonoBehaviour
{
    public ParticleSystem damageParticles; // assign in Inspector or instantiate at runtime

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall") || other.CompareTag("AI") || other.CompareTag("Player"))
        {
            // Try to get the closest point between the car and the wall
            Vector3 contactPoint = other.ClosestPoint(transform.position);

            damageParticles.transform.position = contactPoint;
            damageParticles.Play();
        }
        else
        {
            damageParticles.Stop();
        }
    }
}
