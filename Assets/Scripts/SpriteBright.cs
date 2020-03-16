using System;
using UnityEngine;

[System.Serializable]
public class SpriteBright
{
    public Color DarkColor;
    public Color OofColor;
    public Color ShotColor;
    public Color BaseColor;

    public float OofTime;
    public float ShotTime;

    public float MaxFadeDistance = 150f;
    public float MinFadeDistance = 50f;

    private float oofTimer = 0f;
    private float shootTimer = 0f;

    public void Update()
    {
        oofTimer -= Time.deltaTime;
        shootTimer -= Time.deltaTime;
    }

    public Color GetColor(float distance)
    {
        if(oofTimer > 0)
        {
            return OofColor;
        }
        
        if(shootTimer > 0)
        {
            return ShotColor;
        }

        var fade = Mathf.Clamp(distance <= MinFadeDistance ? 0f : ((distance - MinFadeDistance) / (MaxFadeDistance - MinFadeDistance)), 0, 1f);

        var antiFade = 1 - fade;

        return new Color(fade * DarkColor.r + antiFade * BaseColor.r, fade * DarkColor.g + antiFade * BaseColor.g, fade * DarkColor.b + antiFade * BaseColor.b);
    }

    public void Shoot()
    {
        shootTimer = ShotTime;
    }

    public void Oof()
    {
        oofTimer = OofTime;
    }
}