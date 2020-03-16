using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OofBoi : MonoBehaviour
{
    public float OofTime = 0.2f;

    private Image myImage;

    private float oofing = 0f;

    private Color baseColor;

    private void Start()
    {
        myImage = GetComponent<Image>();
        baseColor = myImage.color;
    }

    private void Update()
    {
        oofing -= Time.deltaTime;

        if(oofing <= 0)
        {
            myImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
            return;
        }

        myImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, Mathf.Pow(oofing-OofTime,2) * 0.8f);
    }

    public void Oof()
    {
        oofing = OofTime;
    }
}
