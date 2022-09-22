using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    public GameObject player;
    public playerController playerScript;
    public GameObject playerSpawnPoint;
    //public GameObject playerKnockbackPoint;

    public bool hostageInRange;

    public Image HPBar;
    public Image hostageHPBar;
    public Image enemyHPBar;
    public GameObject bossHealthMenu;

    public GameObject menuCurrentlyOpen;
    public GameObject pauseMenu;
    public GameObject playerDamage;
    public TextMeshProUGUI totalEnemiesAlive;
    public TextMeshProUGUI enemyCounterText;
    public TextMeshProUGUI enemyLeftText;
    public TextMeshProUGUI mission;
    public TextMeshProUGUI enemiesDead;
    public TextMeshProUGUI hostageRescued;
    public TextMeshProUGUI sceneMessage;
    public GameObject hostageBar;
    public GameObject winMenu;
    public GameObject playerDeadMenu;
    public GameObject gameOverMenu;
    public TextMeshProUGUI reloadingText;
    public TextMeshProUGUI reloadingTime;

    public bool isPaused;
    float timeScaleOriginal;
    int enemyCount;

    private Scene scene;


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        playerSpawnPoint = GameObject.Find("Player Spawn Point");
        timeScaleOriginal = Time.timeScale;

        // It is save to remove listeners even if they
        // didn't exist so far.
        // This makes sure it is added only once
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Add the listener to be called when a scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;

        increaseEnemyCount(GameObject.FindGameObjectsWithTag("Enemy").Length);

        DontDestroyOnLoad(gameObject);

        // Store the creating scene as the scene to trigger start
        scene = SceneManager.GetActiveScene();
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

        if (hostageInRange) 
        {
            StartCoroutine(checkIfEnemyCountIsZero());
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // return if not the start calling scene
        if (!string.Equals(scene.path, this.scene.path) )
            return;

        if (scene.buildIndex == 1) 
        {
            playerScript.Reset();
            totalEnemiesAlive.gameObject.SetActive(true);
            mission.gameObject.SetActive(true);
        }
        else if(scene.buildIndex == 2) 
        {
            totalEnemiesAlive.gameObject.SetActive(true);
            mission.gameObject.SetActive(true);
            hostageRescued.gameObject.SetActive(true);
            hostageBar.SetActive(true);
        }

        if(scene.buildIndex > 0)
            playerScript.playerRespawn();
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

    public void increaseEnemyCount(int amount)
    {
        enemyCount += amount;
        enemyCounterText.text = enemyCount.ToString("F0");

    }

    public void decreaseEnemyCount()
    {
        enemyCount--;
        enemyCounterText.text = enemyCount.ToString("F0");
        StartCoroutine(checkIfEnemyCountIsZero());
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

    IEnumerator checkIfEnemyCountIsZero()
    {
        if (enemyCount <= 0 && hostageInRange) // and player in range of hostage
        {
            enemiesDead.faceColor = Color.green;
            yield return new WaitForSeconds(2);
            menuCurrentlyOpen = winMenu;
            menuCurrentlyOpen.SetActive(true);
            cursorLockPause();
        }
        else if(enemyCount <= 0) 
        {
            enemiesDead.faceColor = Color.green;

            if (scene.buildIndex == 2)
                sceneMessage.text = "Find the Hostage!";

            sceneMessage.gameObject.SetActive(true);
            yield return new WaitForSeconds(2);
            sceneMessage.gameObject.SetActive(false);

        }
    }

}