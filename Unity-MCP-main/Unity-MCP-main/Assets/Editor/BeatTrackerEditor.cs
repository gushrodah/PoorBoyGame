using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BeatTracker))]
public class BeatTrackerEditor : Editor
{
    private const float TimelineHeight = 80f; // Increased from 40f to 80f
    private const float ViewDuration = 4f; // Show 4 seconds of timeline

    private float[] waveformData;
    private const int WaveformResolution = 1000;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BeatTracker beatTracker = (BeatTracker)target;
        float secPerBeat = 60f / beatTracker.bpm;

        // Timeline background
        Rect timelineRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, TimelineHeight);
        EditorGUI.DrawRect(timelineRect, new Color(0.2f, 0.2f, 0.2f));

        // Draw waveform if audio clip exists
        if (beatTracker.audioSource != null && beatTracker.audioSource.clip != null)
        {
            DrawWaveform(timelineRect, beatTracker);
        }

        // Draw beat markers
        float startTime = Application.isPlaying ? beatTracker.SongPosition : 0f;
        float endTime = startTime + ViewDuration;

        for (float time = 0; time < endTime; time += secPerBeat)
        {
            float x = timelineRect.x + (time - startTime) / ViewDuration * timelineRect.width;

            // Draw beat line
            Rect beatRect = new Rect(x - 1, timelineRect.y, 2, timelineRect.height);
            EditorGUI.DrawRect(beatRect, Color.white);

            // Draw hit window
            Rect hitWindowRect = new Rect(
                x - (beatTracker.hitWindowSeconds / ViewDuration * timelineRect.width),
                timelineRect.y,
                (beatTracker.hitWindowSeconds * 2 / ViewDuration * timelineRect.width),
                timelineRect.height
            );
            EditorGUI.DrawRect(hitWindowRect, new Color(0, 1, 0, 0.2f));
        }

        // Draw current time marker
        if (Application.isPlaying)
        {
            float currentX = timelineRect.x + (beatTracker.SongPosition - startTime) / ViewDuration * timelineRect.width;
            Rect currentTimeRect = new Rect(currentX - 1, timelineRect.y, 2, timelineRect.height);
            EditorGUI.DrawRect(currentTimeRect, Color.red);
        }

        // Repaint continuously in play mode
        if (Application.isPlaying)
        {
            Repaint();
        }
    }

    private void DrawWaveform(Rect rect, BeatTracker beatTracker)
    {
        AudioClip clip = beatTracker.audioSource.clip;

        // Cache waveform data
        if (waveformData == null || waveformData.Length != WaveformResolution)
        {
            waveformData = new float[WaveformResolution];
            float[] samples = new float[clip.samples];
            clip.GetData(samples, 0);

            int packSize = clip.samples / WaveformResolution;
            for (int i = 0; i < WaveformResolution; i++)
            {
                float sum = 0;
                for (int j = 0; j < packSize && (i * packSize + j) < samples.Length; j++)
                {
                    sum += Mathf.Abs(samples[i * packSize + j]);
                }
                waveformData[i] = sum / packSize;
            }
        }

        // Draw waveform
        float startTime = Application.isPlaying ? beatTracker.SongPosition : 0f;
        float endTime = startTime + ViewDuration;

        int startIndex = Mathf.FloorToInt((startTime / clip.length) * WaveformResolution);
        int endIndex = Mathf.CeilToInt((endTime / clip.length) * WaveformResolution);

        startIndex = Mathf.Clamp(startIndex, 0, WaveformResolution - 1);
        endIndex = Mathf.Clamp(endIndex, 0, WaveformResolution - 1);

        for (int i = startIndex; i < endIndex - 1; i++)
        {
            float viewProgress = (i - startIndex) / (float)(endIndex - startIndex);
            float xPos = rect.x + viewProgress * rect.width;
            float nextViewProgress = (i + 1 - startIndex) / (float)(endIndex - startIndex);
            float nextXPos = rect.x + nextViewProgress * rect.width;

            float height = waveformData[i] * rect.height;
            Vector2 start = new Vector2(xPos, rect.y + rect.height / 2 + height / 2);
            Vector2 end = new Vector2(nextXPos, rect.y + rect.height / 2 - height / 2);

            Handles.color = new Color(0.5f, 0.5f, 1f, 0.5f);
            Handles.DrawLine(start, end);
        }
    }
}