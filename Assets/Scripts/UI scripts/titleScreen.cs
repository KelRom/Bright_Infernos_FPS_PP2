using Microsoft.Unity.VisualStudio.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;

public class titleScreen : MonoBehaviour
{
    // Start is called before the first frame update
    public static titleScreen instance;
    
    public GameObject currentMenu;
    public Animator titleFade;
    public GameObject cirleTransition;
    public GameObject titleMenu;
    public GameObject settingsMenu;
    public GameObject creditsMenu;
    public GameObject backButton;

    private void Awake()
    {
        instance = this;
        currentMenu = titleMenu;
        
    }

    public IEnumerator transitionScene()
    {
        titleFade.SetTrigger("TitleFade");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public IEnumerator showNextMenu(string nextMenu)
    {
        cirleTransition.SetActive(true);
        yield return new WaitForSeconds(.9f);
        currentMenu.SetActive(false);
        switch (nextMenu)
        {
            case "Settings":
                currentMenu = settingsMenu;
                break;
            case "Credits":
                currentMenu = creditsMenu;
                break;
            default:
                currentMenu = titleMenu;
                break;
        }
        backButton.SetActive(!backButton.activeSelf);
        currentMenu.SetActive(true);
    }
}
