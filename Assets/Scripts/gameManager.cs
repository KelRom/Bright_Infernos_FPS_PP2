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
    public GameObject[] enemySpawnPoint = new GameObject[5];

    System.Random random;

    public Image HPBar;

    public GameObject menuCurrentlyOpen;
    public GameObject pauseMenu;
    public GameObject playerDamage;
    public TextMeshProUGUI enemyCounterText;
    public TextMeshProUGUI enemyLeftText;
    public GameObject winMenu;
    public GameObject playerDeadMenu;

    public bool isPaused;
    float timeScaleOriginal;
    int numberOfEnemies;
    int enemyCount;
    int enemiesKilled;
    bool isSpawnDelay;

    [SerializeField] int maxEnemiesSpawned;
    [SerializeField] int totalEnemies;
    [SerializeField] int spawnDelay;
    [SerializeField] GameObject enemy;


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        playerSpawnPoint = GameObject.Find("Player Spawn Point");
        timeScaleOriginal = Time.timeScale;

        for(int i = 0; i < 5; i++) 
        {
            enemySpawnPoint[i] = GameObject.Find("Enemy Spawn Point " + i);
        }

        enemyLeftText.text = (totalEnemies).ToString("F0");

        random = new System.Random();
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

        StartCoroutine(spawnEnemies());

    }

    IEnumerator spawnEnemies() 
    {
        if (numberOfEnemies < maxEnemiesSpawned && enemyCount < totalEnemies)
        {
            if (isSpawnDelay == false)
            {
                isSpawnDelay = true;
                spawnEnemy();

                yield return new WaitForSeconds(spawnDelay);
                isSpawnDelay = false;
            }
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

    public void increaseEnemyCount()
    {
        numberOfEnemies++;
        enemyCount++;
        enemyCounterText.text = numberOfEnemies.ToString("F0");

    }

    public void decreaseEnemyCount()
    {
        numberOfEnemies--;
        enemiesKilled++;
        enemyCounterText.text = numberOfEnemies.ToString("F0");
        enemyLeftText.text = (totalEnemies - enemiesKilled).ToString("F0");
        StartCoroutine(checkIfEnemyCountIsZero());
    }

    public void playerIsDead()
    {
        isPaused = true;
        menuCurrentlyOpen = playerDeadMenu;
        menuCurrentlyOpen.SetActive(true);
        cursorLockPause();
    }

    IEnumerator checkIfEnemyCountIsZero()
    {
        if(numberOfEnemies <= 0)
        {
            yield return new WaitForSeconds(2);
            menuCurrentlyOpen = winMenu;
            menuCurrentlyOpen.SetActive(true);
            cursorLockPause();
        }
    }

    void spawnEnemy() 
    {
        Instantiate(enemy, enemySpawnPoint[random.Next() % 5].transform.position, transform.rotation);
    }
}
