using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suicidal : MonoBehaviour
{
    public float time = 1f;

    void Update()
    {
        time -= Time.deltaTime;

        if(time <= 0f)
        {
            Destroy(this.gameObject);
        }
    }
}
