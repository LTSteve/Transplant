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

    public int AmmoCount = 0;

    public Transform SpatterIcon;
    public Transform AmmoBlips;


    void Start()
    {
        if(Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    public void UpdateAmmoBlips(int increment = -999)
    {
        AmmoCount += increment;

        if(AmmoCount < 0)
        {
            PlayerController.Instance.Died();
        }

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

    public void DoBeatThings()
    {
        HeartImage.sprite = HeartStrongSprite;

        UpdateAmmoBlips(1);
    }

    public void DoWeakBeatThings()
    {
        HeartImage.sprite = HeartWeakSprite;
    }
}
