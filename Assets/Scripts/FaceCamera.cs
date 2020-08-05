using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    Camera camera;
    private void Awake()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        transform.rotation = camera.transform.rotation;
    }
}
