using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovment : MonoBehaviour
{
    public float speed = 5.0f;
    private float horizontalInput;
    private float forwardInput;
    private List<Vector3> detectedOrigins = new List<Vector3>();
    public float detectionRange = 100f;
    public int numReceivers = 5; // Simulate multiple receivers for beamforming
    public float receiverSpacing = 1.0f; // Distance between receivers
    public float noiseLevel = 10f; // Environmental noise level

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardInput);
        transform.Translate(Vector3.right * Time.deltaTime * speed * horizontalInput);

        DetectSignals();
        ProcessBeamforming();
    }


    void DetectSignals()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var collider in hitColliders)
        {
            if (collider.gameObject == gameObject) continue;

            RaycastHit hit;
            if (Physics.Raycast(collider.transform.position, (transform.position - collider.transform.position).normalized, out hit, detectionRange))
            {
                // Simulate signal strength with noise
                float signalStrength = Random.Range(0f, 100f); // Simulate a detected signal strength
                if (signalStrength > noiseLevel)
                {
                    detectedOrigins.Add(collider.transform.position);
                    Debug.Log($"Detected sonar signal from: {collider.transform.position}, Signal Strength: {signalStrength}");
                }
            }
        }
    }

    void ProcessBeamforming()
    {
        for (int i = 0; i < numReceivers; i++)
        {
            Vector3 receiverPosition = transform.position + transform.right * (i - (numReceivers / 2)) * receiverSpacing;
            foreach (var origin in detectedOrigins)
            {
                float distance = Vector3.Distance(receiverPosition, origin);
                float delay = distance / 343.0f; // Speed of sound in water

                // Simulate environmental effects or processing delays
                float receivedSignalStrength = Mathf.Max(0, 100 - (distance / detectionRange) * 100);
                if (receivedSignalStrength > noiseLevel)
                {
                    Debug.Log($"Receiver {i}: Range = {distance}, Bearing = {Mathf.Atan2((origin.x - receiverPosition.x), (origin.z - receiverPosition.z)) * Mathf.Rad2Deg}, Received Signal Strength: {receivedSignalStrength}");
                }
            }
        }
    }
}