using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseBoi : MonoBehaviour
{
    public static float DeltaUp = 0f;
    public static float DeltaRight = 0f;

    private static MouseBoi Instance = null;

    private void Start()
    {
        if(Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    private void FixedUpdate()
    {
        DeltaUp = -Input.GetAxis("Mouse Y");
        DeltaRight = Input.GetAxis("Mouse X");
    }
}
