using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{

    [SerializeField] int sensHoir;
    [SerializeField] int sensVert;

    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;

    [SerializeField] bool invert;

    float xRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        // Get input
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensHoir;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensVert;

        if (invert)
        {
            xRotation += mouseY;
        }
        else
        {
            xRotation -= mouseY;
        }

        // Clamp rotation
        xRotation = Mathf.Clamp(xRotation, lockVertMin, lockVertMax);

        // Camera rotation x-axis
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        // Rotate the player
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
