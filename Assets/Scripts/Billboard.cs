using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera camera;

    private Quaternion originalRotation;

    protected virtual void Start()
    {
        camera = Camera.main;
        originalRotation = transform.rotation;
    }

    protected virtual void Update()
    {
        transform.rotation = camera.transform.rotation * originalRotation;
    }
}
