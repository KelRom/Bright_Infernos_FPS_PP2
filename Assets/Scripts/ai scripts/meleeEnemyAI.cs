using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class meleeEnemyAI : MonoBehaviour, IDamageable
{
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer rend;
    [SerializeField] Animator anim;
    [Header("optional")]
    [SerializeField] UnityEngine.UI.Image hpBar;

    [Header("----- Enemy Stats -----")]
    [Range(0, 100)] [SerializeField] int HP;
    [Range(0, 10)] [SerializeField] float speedRoam;
    [Range(0, 10)] [SerializeField] float speedChase;
    [Range(0, 10)] [SerializeField] int playerFaceSpeed;
    [Range(0, 50)] [SerializeField] int roamRadius;
    [Range(1, 180)] [SerializeField] int viewAngle;
    [SerializeField] GameObject headPosition;
    [SerializeField] float knockbackForce;
    [SerializeField] float knockbackTime;
    private float knockbackCounter;
    private Vector3 enemyVelocity;


    [Header("----- Attack Stats -----")]
    [SerializeField] float aggroRange;
    [SerializeField] float attackRange; //How far the enemy can attack from, keep in mind the distance between colliders
    [SerializeField] float attackRate;
    [SerializeField] int attackDamage;
    [Header("----- Enemy Drops -----")]
    [Header("optional")]
    [SerializeField] GameObject enemyDrop;

    [Header("----- Audio -----")]

    [SerializeField] AudioSource aud;

    [SerializeField] AudioClip[] enemyDamageSound;
    [Range(0, 1)] [SerializeField] float enemyDamageSoundVol;

    [SerializeField] AudioClip[] enemyDeathSound;
    [Range(0, 1)] [SerializeField] float enemyDeathSoundVol;

    [SerializeField] AudioClip[] enemyAttackSound;
    [Range(0, 1)] [SerializeField] float enemyAttackSoundVol;

    Vector3 playerDir;
    bool takingDamage;
    bool isMeleeing;
    bool playerIsSeen;
    Vector3 lastPlayerPos;
    float stoppingDistOrig;
    bool hasSeen;
    Vector3 startingPos;
    float animSpeedOrig;
    int origHP;


    void Start()
    {
        lastPlayerPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
        agent.speed = speedRoam;
        startingPos = transform.position;
        animSpeedOrig = anim.speed;
        origHP = HP;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.enabled)
        {
            
            playerDir = gameManager.instance.player.transform.position - headPosition.transform.position;
            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agent.velocity.normalized.magnitude, Time.deltaTime * 4));

            if (!takingDamage)
            {
                if (playerIsSeen)
                    rayToPlayer();


                if (agent.remainingDistance < 0.001f && agent.destination != gameManager.instance.player.transform.position)
                    roam();
            }
        }
    }

    void roam()
    {
        agent.stoppingDistance = 0;
        agent.speed = speedRoam;


        Vector3 randomDir = Random.insideUnitSphere * roamRadius;
        randomDir += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDir, out hit, 1, 1);
        NavMeshPath path = new NavMeshPath();

        if (hit.hit) //Fix for Infinity Bug
        {
            agent.CalculatePath(hit.position, path);
            agent.SetPath(path);
        }
    }

    public void SphereTriggerEnter(Collider other)
    {
        //Debug.Log("On Trigger Enter : From " + gameObject.name.ToString());
        if (other.CompareTag("Player"))
            playerIsSeen = true;
    }

    public void SphereTriggerExit(Collider other)
    {
        //Debug.Log("On Trigger Exit : From " + gameObject.name.ToString());
        if (other.CompareTag("Player"))
        {
            playerIsSeen = false;
            lastPlayerPos = gameManager.instance.player.transform.position;
            agent.stoppingDistance = 0;
            hasSeen = false;
        }
    }

    void facePlayer()
    {
        playerDir.y = 0;
        Quaternion rotation = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * playerFaceSpeed);
    }

    public void takeDamage(int dmg)
    {
        HP -= dmg;
        anim.SetTrigger("Damage");
        agent.speed = 0;

        StartCoroutine(flashColor());
        knockback();
        lastPlayerPos = gameManager.instance.player.transform.position;

        if (HP <= 0 && agent.enabled)
        {
            enemyDead();
        }
        else
        {
            aud.PlayOneShot(enemyDamageSound[Random.Range(0, enemyDamageSound.Length)], enemyDamageSoundVol);
            //print("Damage");
        }


        if (!playerIsSeen && agent.enabled)
        {
            agent.SetDestination(lastPlayerPos);
            //agent.SetPath();
        }

        if(hpBar != null)
        {
            hpBar.fillAmount = (float)HP / (float)origHP;
            print("test");
        }
    }
    IEnumerator flashColor()
    {
        takingDamage = true;
        rend.material.color = Color.red;
        agent.speed = 0;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = Color.white;
        agent.speed = speedChase;
        takingDamage = false;
    }

    IEnumerator melee()
    {
        //If range < attack range && player in site 
        isMeleeing = true;
        anim.speed = attackRate;
        anim.SetTrigger("Attack");
        
        
        yield return new WaitForSeconds(attackRate);
        anim.speed = animSpeedOrig;
        isMeleeing = false;
    }

    public void AttackSound()
    {
        aud.PlayOneShot(enemyAttackSound[Random.Range(0, enemyAttackSound.Length)], enemyAttackSoundVol);
        //print("Attack");
    }

    public void DealDamageToPlayer() //This is an animation event, called on the enemy attack animations
    {
        //Debug.Log("Damage " + attackDamage + " : From " + gameObject.name);
        gameManager.instance.playerScript.takeDamage(attackDamage);
    }

    void rayToPlayer()
    {
        //Debug.Log("Player In Range : From " + gameObject.name.ToString());
        float angle = Vector3.Angle(playerDir, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(headPosition.transform.position, playerDir + (transform.up / 2), out hit))
        {
#if UNITY_EDITOR
            Debug.DrawRay(headPosition.transform.position, playerDir + (transform.up / 2));
#endif
            agent.speed = speedChase;
            
            if (hit.collider.CompareTag("Player") && (angle <= viewAngle || Vector3.Distance(transform.position, gameManager.instance.player.transform.position) < aggroRange))
            {
                hasSeen = true;
                lastPlayerPos = gameManager.instance.player.transform.position;

                agent.SetDestination(gameManager.instance.player.transform.position);
                agent.stoppingDistance = stoppingDistOrig;
                //Debug.Log("Remaining Dist 1 : " + agent.remainingDistance);
                facePlayer();
                //if not already shooting and the remaing distance from the player is less than or equal to the shoot 
                //Distance of the enemy then open fire.
                agent.stoppingDistance = attackRange;
                //Debug.Log("Stopping Distance : " + agent.stoppingDistance);
                
                if (!isMeleeing && agent.remainingDistance <= attackRange)
                {
                    StartCoroutine(melee());
                    //Debug.Log("Remaining Dist 2 : " + agent.remainingDistance);
                    
                }
                //Debug.Log("Remaining Dist 3 : " + agent.remainingDistance);
            }
            else if (hasSeen == true)
            {
                //If the enemy has seen the player, they will follow after
                //Just like exiting the range, but instead exiting sight
                agent.SetDestination(lastPlayerPos);
                agent.stoppingDistance = 0;
                hasSeen = false;
            }
            else
            {
                //roam();
            }
        }
    }

    public void playerDied()
    {
        //Added to fix bug: OnCollisionExit not being called when player dies

        playerIsSeen = false;
        agent.stoppingDistance = 0;
        roam();
    }

    void enemyDead()
    {
        aud.PlayOneShot(enemyDeathSound[Random.Range(0, enemyDeathSound.Length)], enemyDeathSoundVol);
        //print("Death");

        anim.SetBool("Dead", true);
        agent.enabled = false;

        if (enemyDrop != null) //Removes null reference for enemies that don't drop items
        {
            Instantiate(enemyDrop, transform.position, enemyDrop.transform.rotation);
        }

        //Turn off all the enemy collision models.
        foreach (Collider col in GetComponents<Collider>())
            col.enabled = false;
    }
    public void knockback()
    {
        Vector3 hitDirection = -transform.position;
        knockbackCounter = knockbackTime;
        enemyVelocity.y = knockbackForce;
        agent.Move(hitDirection.normalized * knockbackForce);
    }
}
