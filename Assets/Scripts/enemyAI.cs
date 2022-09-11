using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamageable
{

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer rend;

    [SerializeField] int HP;
    [SerializeField] int playerFaceSpeed;

    [SerializeField] float shootRate;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPosition;

    Vector3 playerDirection;
    bool isShooting;
    private bool isPlayerInRange;
    Vector3 playerLastKnownPosition;
    private float originalStoppingDistance;

    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.increaseEnemyCount();
        playerLastKnownPosition = transform.position;
        originalStoppingDistance = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        playerDirection = gameManager.instance.player.transform.position - transform.position;
        if(isPlayerInRange)
            canEnemySeePlayer();
        else
        {
            agent.SetDestination(playerLastKnownPosition);
            agent.stoppingDistance = 0;
        }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerLastKnownPosition = gameManager.instance.player.transform.position;
            agent.stoppingDistance = 0;
        }
    }

    void facePlayer()
    {
        playerDirection.y = 0;
        Quaternion rotation = Quaternion.LookRotation(playerDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * playerFaceSpeed);
    }

    public void takeDamage(int damage)
    {
        HP -= damage;

        playerLastKnownPosition = gameManager.instance.player.transform.position;
        StartCoroutine(flashDamage());

        if(HP <= 0)
        {
            gameManager.instance.decreaseEnemyCount();
            Destroy(gameObject);
        }
    }

    IEnumerator flashDamage()
    {
        rend.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = Color.white;
    }

    IEnumerator shoot()
    {
        isShooting = true;
        Instantiate(bullet, shootPosition.transform.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    private void canEnemySeePlayer()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, playerDirection, out hit))
        {
            Debug.DrawRay(transform.position, playerDirection);
            if (hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(gameManager.instance.player.transform.position);
                agent.stoppingDistance = originalStoppingDistance;

                if (agent.stoppingDistance <= agent.remainingDistance)
                    facePlayer();

                if (!isShooting)
                    StartCoroutine(shoot());
            }
        }
    }
}
