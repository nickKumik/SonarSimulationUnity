using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SonarEmitter : MonoBehaviour
{
    public float detectionRange = 100f;
    public float emissionInterval = 2f;
    public float coneAngle = 30f; // Angle of the cone in degrees
    public float baseSignalStrength = 100f; // Maximum signal strength
    public int reflectionBounces = 3; // Number of reflections to simulate
    public LayerMask sonarReflectiveLayers; // Layer mask for reflective objects

    private void Start()
    {
        InvokeRepeating(nameof(EmitSonar), 0, emissionInterval);
    }

    // Emit sonar rays with cone-shaped coverage
    void EmitSonar()
    {
        Vector3 direction = transform.forward;
        Ray ray = new Ray(transform.position, direction);

        for (float angle = -coneAngle / 2; angle <= coneAngle / 2; angle += 5f)
        {
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 rotatedDirection = rotation * direction;

            // Launch sonar ray in the rotated direction
            TraceSonarRay(transform.position, rotatedDirection, detectionRange, baseSignalStrength, reflectionBounces);
        }
    }

    // Trace sonar ray, simulate reflections and signal attenuation
    void TraceSonarRay(Vector3 origin, Vector3 direction, float range, float signalStrength, int remainingBounces)
    {
        Ray ray = new Ray(origin, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range, sonarReflectiveLayers))
        {
            float distance = hit.distance;
            float attenuatedSignal = Mathf.Max(0, signalStrength - (distance / range) * signalStrength);

            // If signal is still strong enough, handle the hit
            if (attenuatedSignal > 0)
            {
                UnityEngine.Debug.DrawLine(ray.origin, hit.point, Color.green, 1f);
                UnityEngine.Debug.Log($"Sonar hit: {hit.collider.gameObject.name} at distance {distance}m, Signal Strength: {attenuatedSignal}");

                // Reflect the ray if there are remaining bounces
                if (remainingBounces > 0)
                {
                    Vector3 reflectDirection = Vector3.Reflect(direction, hit.normal);
                    TraceSonarRay(hit.point, reflectDirection, range - distance, attenuatedSignal, remainingBounces - 1);
                }
            }
        }
        else
        {
            // Visualize rays that didn't hit anything
            UnityEngine.Debug.DrawLine(ray.origin, ray.origin + direction * range, Color.red, 1f);
        }
    }
}