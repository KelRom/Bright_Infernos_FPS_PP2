using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleQuests : MonoBehaviour
{
    [SerializeField] KeyCode toggleKey = KeyCode.Q;
    [SerializeField] GameObject uiContainer = null;
    void Start()
    {
        uiContainer.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey)) 
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        uiContainer.SetActive(!uiContainer.activeSelf);
    }
}
