using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OofBoi : MonoBehaviour
{
    public float OofTime = 0.2f;

    public float ShakeIntensity = 1f;

    private Image myImage;

    private float oofing = 0f;

    private Color baseColor;

    public Transform Screen;

    private Vector3 initialScreenPosition;

    private void Start()
    {
        myImage = GetComponent<Image>();
        baseColor = myImage.color;

        initialScreenPosition = Screen.position;
    }

    private void Update()
    {
        oofing -= Time.deltaTime;

        if(oofing <= 0)
        {
            Screen.position = initialScreenPosition;
            myImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
            return;
        }

        if(Settings.ScreenShake)
            Screen.position = initialScreenPosition + Random.insideUnitSphere * ShakeIntensity;

        if(Settings.ScreenFlash)
            myImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, Mathf.Pow(oofing-OofTime,2));
    }

    public void Oof()
    {
        oofing = OofTime;
    }
}
