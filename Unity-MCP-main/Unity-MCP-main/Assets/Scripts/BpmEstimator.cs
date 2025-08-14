using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BpmEstimator : MonoBehaviour
{
    public float estimatedBpm;
    private List<float> tapTimes = new List<float>();
    private const int maxTaps = 1000; // Number of taps to average
    private const float resetThreshold = 2f; // Reset if no tap for 2 seconds

    private float lastTapTime;

    void Update()
    {
        // Check for spacebar press or left mouse click
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            RegisterTap();
        }

        // Reset if too much time passed
        if (tapTimes.Count > 0 && Time.time - lastTapTime > resetThreshold)
        {
            tapTimes.Clear();
        }
    }

    private void RegisterTap()
    {
        float currentTime = Time.time;
        lastTapTime = currentTime;

        tapTimes.Add(currentTime);
        
        // Keep only the last maxTaps number of taps
        if (tapTimes.Count > maxTaps)
        {
            tapTimes.RemoveAt(0);
        }

        // Need at least 2 taps to calculate BPM
        if (tapTimes.Count >= 2)
        {
            float averageInterval = 0f;
            
            // Calculate average time between taps
            for (int i = 1; i < tapTimes.Count; i++)
            {
                averageInterval += tapTimes[i] - tapTimes[i - 1];
            }
            averageInterval /= (tapTimes.Count - 1);

            // Convert to BPM
            estimatedBpm = 60f / averageInterval;
        }
    }
}
