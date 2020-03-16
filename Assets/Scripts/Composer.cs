using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Composer : MonoBehaviour
{
    public static Composer Instance = null;

    public AudioSource SoundtrackSource;
    public AudioSource SecondarySoundtrackSource;

    public float BPM = 130f;

    public float FudgeTime = 0.5f;

    private float timeSinceBeat = 0f;
    private float timeSinceWeakBeat = 0f;

    [HideInInspector]
    public float BeatLength;

    public bool Beat = false;
    public bool WeakBeat = false;

    private void Awake()
    {
        Instance = this;

        BeatLength = 60f / BPM;

        timeSinceBeat = -100f;
        timeSinceWeakBeat = -100f;
    }

    private bool playing = false;
    private bool paused = false;

    private void Update()
    {
        if (Settings.Active)
        {
            if (!paused)
            {
                playing = SoundtrackSource.isPlaying;

                if (playing)
                {
                    SoundtrackSource.Pause();
                }
                else
                {
                    SecondarySoundtrackSource.Pause();
                }
            }

            paused = true;
            return;
        }
        else
        {

            if (paused)
            {
                paused = false;


                if (playing)
                {
                    SoundtrackSource.UnPause();
                }
                else
                {
                    SecondarySoundtrackSource.UnPause();
                }
            }
        }

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
        if (Settings.Active || paused)
        {
            return;
        }

        if ((!SecondarySoundtrackSource.isPlaying && !SoundtrackSource.isPlaying) ||
            (SoundtrackSource.isPlaying && SoundtrackSource.time + Time.fixedDeltaTime >= SoundtrackSource.clip.length))
        {
            SecondarySoundtrackSource.Play();

            timeSinceBeat = 0;
            timeSinceWeakBeat = (BeatLength / 4f);

            TextCrawl.WindUpOver = true;
        }
    }

    private void UpdateBeatState()
    {
        WeakBeat = false;
        Beat = false;

        timeSinceBeat += Time.deltaTime;

        if (timeSinceBeat > BeatLength)
        {
            timeSinceBeat %= BeatLength;

            Beat = true;
        }

        timeSinceWeakBeat += Time.deltaTime;

        if (timeSinceWeakBeat > BeatLength)
        {
            timeSinceWeakBeat %= BeatLength;

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

    public bool CheckTiming()
    {
        var time = timeSinceBeat > (BeatLength / 2f) ? (BeatLength - timeSinceBeat) : timeSinceBeat;

        return time < FudgeTime;
    }
}
