using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    public Image HeartImage;
    public Sprite HeartStrongSprite;
    public Sprite HeartWeakSprite;
    public Sprite EmptySprite;
    public Sprite FullSprite;

    public float BPM = 120f;
    public int AmmoCount = 0;

    public Transform SpatterIcon;
    public Transform AmmoBlips;

    private float timeSinceBeat = 0f;
    private float timeSinceWeakBeat = 0f;
    private float beatLength;

    private bool beat = false;
    private bool weakBeat = false;


    void Start()
    {
        if(Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        beatLength = 60f / BPM;

        timeSinceBeat = 0;
        timeSinceWeakBeat = (beatLength / 4f);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBeatState();

        if (beat)
        {
            DoBeatThings();
        }

        if (weakBeat)
        {
            DoWeakBeatThings();
        }
    }

    public void UpdateAmmoBlips(int increment = -999)
    {
        AmmoCount += increment;

        AmmoCount = Mathf.Clamp(AmmoCount, 0, 11);

        var blips = AmmoBlips.GetComponentsInChildren<Image>();

        for(var i = 0; i < 10; i++)
        {
            if(i >= AmmoCount)
            {
                blips[i].sprite = EmptySprite;
            }
            else
            {
                blips[i].sprite = FullSprite;
            }
        }

        SpatterIcon.gameObject.SetActive(AmmoCount == 11);
    }

    private void DoBeatThings()
    {
        HeartImage.sprite = HeartStrongSprite;

        UpdateAmmoBlips(1);
    }

    private void DoWeakBeatThings()
    {
        HeartImage.sprite = HeartWeakSprite;
    }

    private void UpdateBeatState()
    {
        weakBeat = false;
        beat = false;

        timeSinceBeat += Time.deltaTime;

        if (timeSinceBeat > beatLength)
        {
            timeSinceBeat %= beatLength;

            beat = true;
        }

        timeSinceWeakBeat += Time.deltaTime;

        if (timeSinceWeakBeat > beatLength)
        {
            timeSinceWeakBeat %= beatLength;

            weakBeat = true;
        }
    }
}
