using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossHealthBar : MonoBehaviour
{
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    Camera camera;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    private void Start()
    {
        camera = FindObjectOfType<Camera>();
        GetComponent<Canvas>().worldCamera = camera;
    }
    void Update()
    {
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
    }
}
