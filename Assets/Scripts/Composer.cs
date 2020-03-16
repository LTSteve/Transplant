using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Composer : MonoBehaviour
{
    public static Composer Instance = null;

    public AudioSource SoundtrackSource;
    public AudioSource SecondarySoundtrackSource;

    public float BPM = 130f;

    private float timeSinceBeat = 0f;
    private float timeSinceWeakBeat = 0f;
    private float beatLength;

    public bool Beat = false;
    public bool WeakBeat = false;

    private void Awake()
    {
        Instance = this;

        beatLength = 60f / BPM;

        timeSinceBeat = -100f;
        timeSinceWeakBeat = -100f;
    }

    private void Update()
    {

        UpdateBeatState();

        if (Beat)
        {
            DoBeatThings();
        }

        if (WeakBeat)
        {
            DoWeakBeatThings();
        }
    }

    void FixedUpdate()
    {
        if ((!SecondarySoundtrackSource.isPlaying && !SoundtrackSource.isPlaying) ||
            (SoundtrackSource.isPlaying && SoundtrackSource.time + Time.fixedDeltaTime >= SoundtrackSource.clip.length))
        {
            SecondarySoundtrackSource.Play();

            timeSinceBeat = 0;
            timeSinceWeakBeat = (beatLength / 4f);

            TextCrawl.WindUpOver = true;
        }
    }

    private void UpdateBeatState()
    {
        WeakBeat = false;
        Beat = false;

        timeSinceBeat += Time.deltaTime;

        if (timeSinceBeat > beatLength)
        {
            timeSinceBeat %= beatLength;

            Beat = true;
        }

        timeSinceWeakBeat += Time.deltaTime;

        if (timeSinceWeakBeat > beatLength)
        {
            timeSinceWeakBeat %= beatLength;

            WeakBeat = true;
        }
    }

    private void DoBeatThings()
    {
        UIController.Instance.DoBeatThings();
    }

    private void DoWeakBeatThings()
    {
        UIController.Instance.DoWeakBeatThings();
    }
}
