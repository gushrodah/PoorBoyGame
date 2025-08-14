using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatTracker : MonoBehaviour
{
    [Header("Rhythm Settings")]
    public float bpm = 120f;
    public float hitWindowSeconds = 0.15f; // Timing window for input accuracy
    public KeyCode inputKey = KeyCode.Space;

    [Header("Audio Settings")]
    public AudioSource audioSource; // Assign an AudioSource in the Inspector

    private float secPerBeat;
    private float songPosition;
    private float lastBeatTime;
    private float nextBeatTime;

    // Start is called before the first frame update
    void Start()
    {
        secPerBeat = 60f / bpm;
        nextBeatTime = secPerBeat;
    }

    // Update is called once per frame
    void Update()
    {
        // Update song position
        songPosition += Time.deltaTime;

        // Check for input
        if (Input.GetKeyDown(inputKey))
        {
            IsOnBeat();
        }

        // Update beat tracking
        if (songPosition >= nextBeatTime)
        {
            lastBeatTime = nextBeatTime;
            nextBeatTime += secPerBeat;
        }
    }

    public bool IsOnBeat()
    {
        // Calculate how far we are from the nearest beat
        float distanceFromBeat = Mathf.Min(
            Mathf.Abs(songPosition - lastBeatTime),
            Mathf.Abs(songPosition - nextBeatTime)
        );

        bool hitSuccess = distanceFromBeat <= hitWindowSeconds;
        Debug.Log($"Hit Success: {hitSuccess} (Offset: {distanceFromBeat:F3} seconds)");
        return hitSuccess;
    }

    public float SongPosition => songPosition;
}
