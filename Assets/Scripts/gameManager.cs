using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    public GameObject player;
    public playerController playerScript;
    public GameObject playerSpawnPoint;

    public Image HPBar;

    public GameObject pauseMenu;
    public GameObject playerDamage;
    GameObject textObject;

    public bool isPaused;
    float timeScaleOriginal;
    int numberOfEnemies;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        playerSpawnPoint = GameObject.Find("Player Spawn Point");
        playerScript.playerRespawn();
        timeScaleOriginal = Time.timeScale;
        textObject = GameObject.Find("Enemies");
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        numberOfEnemies = enemies.Length;
        if (Input.GetButtonDown("Cancel"))
            togglePauseMenu();
        displayNumberOfEnemies();
    }

    public void togglePauseMenu()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Cursor.visible = isPaused ? true : false;
        Cursor.lockState = isPaused ? CursorLockMode.Confined : CursorLockMode.Locked;
        Time.timeScale = isPaused ? 0f : timeScaleOriginal;
    }

    void displayNumberOfEnemies()
    {
        textObject.GetComponent<TextMeshProUGUI>().text = "Number of Enemies: " + numberOfEnemies;
            
    }
}
