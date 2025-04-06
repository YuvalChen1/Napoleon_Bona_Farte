using UnityEngine;

public class AmbientSoundManager : MonoBehaviour
{
    public AudioSource windSource;
    public float windDelayMin = 10f;
    public float windDelayMax = 20f;

    private float nextWindTime = 0f;

    void Start()
    {
        ScheduleNextWind();
    }

    void Update()
    {
        // Wait for the next scheduled time
        if (Time.time >= nextWindTime)
        {
            windSource.Play();
            ScheduleNextWindAfterPlayback();
        }
    }

    private void ScheduleNextWind()
    {
        nextWindTime = Time.time + Random.Range(windDelayMin, windDelayMax);
    }

    private void ScheduleNextWindAfterPlayback()
    {
        // Add delay AFTER sound finishes playing
        float clipDuration = windSource.clip.length;
        float delayAfter = Random.Range(windDelayMin, windDelayMax);
        nextWindTime = Time.time + clipDuration + delayAfter;
    }
}
