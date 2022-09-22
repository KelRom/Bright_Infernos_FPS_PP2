using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class bossAI : MonoBehaviour, IDamageable
{
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer rend;
    [SerializeField] Animator anim;

    [Header("----- Enemy Stats -----")]
    [Range(0, 10)] [SerializeField] int HP;
    private int HPOriginal;
    [Range(1, 10)] [SerializeField] int playerFaceSpeed;
    [Range(1, 50)] [SerializeField] int roamRadius;
    [Range(1, 180)] [SerializeField] int viewAngle;
    [Range(1, 10)] [SerializeField] float chaseSpeed;
    [SerializeField] float knockbackStrength;
    [SerializeField] float knockbackResistance;

    [Header("----- Weapon Stats -----")]
    [SerializeField] float shootRate;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPosition;

    [Header("----- Drops -----")]
    [SerializeField] GameObject[] drops;
    [Range(1, 5)] [SerializeField] int dropRadius;

    Vector3 playerDirection;
    bool isShooting;
    private bool isPlayerInRange;
    Vector3 playerLastKnownPosition;
    private float originalStoppingDistance;
    float origSpeed;
    Vector3 startingPos;
    bool alive = true;
    bool isTakingDamage;

    private float angle;

    // Start is called before the first frame update
    void Start()
    {
        HPOriginal = HP;
        playerLastKnownPosition = transform.position;
        originalStoppingDistance = agent.stoppingDistance;
        origSpeed = agent.speed;
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        playerDirection = gameManager.instance.player.transform.position - transform.position;
        angle = Vector3.Angle(playerDirection, transform.forward);
        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agent.velocity.normalized.magnitude, Time.deltaTime * 4));

        if (!isTakingDamage)
        {
            if (isPlayerInRange)
            {
                canEnemySeePlayer();
            }
            if (agent.remainingDistance < 0.1f && agent.destination != gameManager.instance.player.transform.position)
            {
                roam();
            }
        }
    }

    void roam()
    {
        agent.stoppingDistance = 0;
        agent.speed = origSpeed;

        Vector3 randomDir = Random.insideUnitSphere * roamRadius;
        randomDir += startingPos;

        NavMeshHit hit;

        NavMesh.SamplePosition(randomDir, out hit, 1, 1);
        NavMeshPath path = new NavMeshPath();

        if (hit.hit)
        {
            agent.CalculatePath(hit.position, path);
            agent.SetPath(path);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = true;
        gameManager.instance.bossHealthMenu.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerLastKnownPosition = gameManager.instance.player.transform.position;
            agent.stoppingDistance = 0;
            gameManager.instance.bossHealthMenu.SetActive(false);
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
        updateBossHP();
        anim.SetTrigger("Damage");
        playerLastKnownPosition = gameManager.instance.player.transform.position;
        agent.SetDestination(playerLastKnownPosition);
        StartCoroutine(flashDamage());

        if (HP <= 0)
        {
            enemyDead();
        }
    }
    public void updateBossHP()
    {
        gameManager.instance.enemyHPBar.fillAmount = (float)HP / (float)HPOriginal;
    }
    void enemyDead()
    {
        dropItem();

        gameManager.instance.decreaseEnemyCount();
        anim.SetBool("Dead", true);
        foreach (Collider col in GetComponents<Collider>())
        {
            col.enabled = false;
        }
        agent.isStopped = true;
        this.enabled = false;

    }

    void dropItem()
    {
        Vector3 randomDir = Random.insideUnitSphere * dropRadius;
        randomDir += transform.position;
        randomDir.y = .5f;

        Instantiate(drops[Random.Range(0, drops.Length - 1)], randomDir, transform.rotation);
    }

    IEnumerator flashDamage()
    {
        isTakingDamage = true;
        agent.speed = 0;
        rend.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = Color.white;
        agent.speed = origSpeed;
        isTakingDamage = false;
    }

    IEnumerator shoot()
    {
        isShooting = true;
        anim.SetBool("isShooting", true);
        Instantiate(bullet, shootPosition.transform.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
        anim.SetBool("isShooting", false);
    }

    private void canEnemySeePlayer()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, playerDirection, out hit))
        {
            Debug.DrawRay(transform.position + Vector3.up, playerDirection);
            if (hit.collider.CompareTag("Player") && angle <= viewAngle)
            {
                agent.SetDestination(gameManager.instance.player.transform.position);
                agent.stoppingDistance = originalStoppingDistance;

                facePlayer();

                if (!isShooting)
                    StartCoroutine(shoot());
            }
            else
                agent.stoppingDistance = 0;
        }

        if (gameManager.instance.playerDeadMenu.activeSelf)
        {
            isPlayerInRange = false;
            agent.stoppingDistance = 0;
        }
    }
}
