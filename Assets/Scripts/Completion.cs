using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Completion : MonoBehaviour
{
    public Transform Bar;

    private void Update()
    {
        Bar.localScale = new Vector3(Mathf.Clamp01(WorldGenerator.CurrentBossCountDown / 100f), 1, 1);
    }
}
