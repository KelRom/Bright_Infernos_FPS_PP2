using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject enemy2;
    [SerializeField] GameObject enemy3;
    [SerializeField] int maxEnemies;
    [SerializeField] int timer;

    bool startSpawning;
    bool isSpawning;
    int enemiesSpawned;


    // Start is called before the first frame update
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
            Instantiate(enemy, transform.position, enemy.transform.rotation);
            yield return new WaitForSeconds(timer);
            isSpawning = false;
        }
    }
}
