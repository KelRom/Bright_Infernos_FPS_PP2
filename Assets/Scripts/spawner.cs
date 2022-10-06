using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    [SerializeField] GameObject[] enemies;
    [SerializeField] int maxEnemies;
    [SerializeField] int timer;

    bool startSpawning;
    bool isSpawning;
    int enemiesSpawned;

    void Start()
    {
        gameManager.instance.increaseEnemyCount(maxEnemies);
    }

    // Update is called once per frame
    void Update()
    {
        if (startSpawning)
            StartCoroutine(spawn());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            startSpawning = true;
    }

    IEnumerator spawn()
    {
        if (!isSpawning && enemiesSpawned < maxEnemies)
        {
            isSpawning = true;
            enemiesSpawned++;
            GameObject enemy = enemies[Random.Range(0, enemies.Length)];
            Instantiate(enemy, transform.position, enemy.transform.rotation);
            yield return new WaitForSeconds(timer);
            isSpawning = false;
        }
    }
}
