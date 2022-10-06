using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    public Animator fadeScreen;
    public GameObject player;
    public playerController playerScript;
    public GameObject playerSpawnPoint;

    public Image HPBar;
    public Image MPBar;
    public Image enemyHPBar;
    public GameObject bossHealthMenu;

    public GameObject menuCurrentlyOpen;
    public GameObject pauseMenu;
    public GameObject playerDamage;
    public GameObject bossHPBar;
    public GameObject winMenu;
    public GameObject playerDeadMenu;
    public GameObject gameOverMenu;
    public GameObject interactPopUpWindow;

    public bool isPaused;
    float timeScaleOriginal;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        playerSpawnPoint = GameObject.Find("Player Spawn Point");
        timeScaleOriginal = Time.timeScale;
        StartCoroutine(turnFadeScreenOff());
        // It is save to remove listeners even if they
        // didn't exist so far.
        // This makes sure it is added only once

        // Store the creating scene as the scene to trigger start
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && menuCurrentlyOpen != playerDeadMenu && menuCurrentlyOpen != winMenu)
        {
            isPaused = !isPaused;
            menuCurrentlyOpen = pauseMenu;
            menuCurrentlyOpen.SetActive(isPaused);

            if (isPaused)
                cursorLockPause();
            else
                cursorUnlockUnpause();
        }
    }

    public void cursorLockPause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0;
    }

    public void cursorUnlockUnpause()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = timeScaleOriginal;
        if (menuCurrentlyOpen != null)
            menuCurrentlyOpen.SetActive(false);
        menuCurrentlyOpen = null;
    }

    public void playerIsDead()
    {
        isPaused = true;
        menuCurrentlyOpen = playerDeadMenu;
        menuCurrentlyOpen.SetActive(true);
        cursorLockPause();
    } 

    public void gameOver()
    {
        isPaused = true;
        menuCurrentlyOpen = gameOverMenu;
        menuCurrentlyOpen.SetActive(true);
        cursorLockPause();
    }

    IEnumerator turnFadeScreenOff()
    {
        if (GameObject.Find("FadeScreen"))
        {
            yield return new WaitForSeconds(1);
            GameObject.Find("FadeScreen").SetActive(false);
        }
    }
}