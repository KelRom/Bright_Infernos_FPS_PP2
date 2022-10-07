using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WizardBossAI : MonoBehaviour, IDamageable
{
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer rend;
    [SerializeField] Animator anim;

    [Header("----- Enemy Stats -----")]
    [Range(0, 10)] [SerializeField] int HP;
    private int HPOriginal;
    [Range(1, 10)] [SerializeField] int playerFaceSpeed;

    [Range(1, 10)] [SerializeField] float engagementSpeed;
    [SerializeField] float knockbackStrength;
    [SerializeField] float knockbackResistance;

    [SerializeField] float manaRegenerationTime;

    [Header("----- Weapon Stats -----")]
    [SerializeField] float castRate;
    [SerializeField] List<spellStats> spellInventory = new List<spellStats>();

    [Header("----- Drops -----")]
    [SerializeField] GameObject[] drops;
    [Range(1, 5)] [SerializeField] int dropRadius;

    [Header("-----Magic Stats-----")]
    [SerializeField] float maxMana;
    [SerializeField] float currentMana;
    [SerializeField] Transform castingPosition;

    Vector3 playerDirection;
    [SerializeField] bool isCasting;
    private float originalStoppingDistance;
    Vector3 startingPos;
    bool isTakingDamage;

    [SerializeField] int selectedSpell;
    private float angle;
    // Start is called before the first frame update
    void Start()
    {
        //killTheBoy();
        currentMana = maxMana;
    }

    // Update is called once per frame
    void Update()
    {
        playerDirection = gameManager.instance.player.transform.position - transform.position;
        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agent.velocity.normalized.magnitude, Time.deltaTime * 4));
        KillTheBoy();
    }

    void FacePlayer()
    {
        playerDirection.y = 0;
        Quaternion rotation = Quaternion.LookRotation(playerDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * playerFaceSpeed);
    }

    public void takeDamage(int damage)
    {
        HP -= damage;
        anim.SetTrigger("Damage");

        //playerLastKnownPosition = gameManager.instance.player.transform.position;
        //agent.SetDestination(playerLastKnownPosition);
        StartCoroutine(FlashDamage());

        if (HP <= 0)
        {
            enemyDead();
        }
    }

    public void UpdateEnemyHP()
    {
        //gameManager.instance.hostageHPBar.fillAmount = (float)HP / (float)HPOriginal;
    }

    void enemyDead()
    {
        DropItem();

        //gameManager.instance.decreaseEnemyCount();
        anim.SetBool("Dead", true);
        foreach (Collider col in GetComponents<Collider>())
        {
            col.enabled = false;
        }
        agent.isStopped = true;
        this.enabled = false;

    }

    void DropItem()
    {
        Vector3 randomDir = Random.insideUnitSphere * dropRadius;
        randomDir += transform.position;
        randomDir.y = .5f;

        Instantiate(drops[Random.Range(0, drops.Length)], randomDir, transform.rotation);
    }

    IEnumerator FlashDamage()
    {
        isTakingDamage = true;
        agent.speed = 0;
        rend.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = Color.white;
        //agent.speed = origSpeed;
        isTakingDamage = false;
    }

    IEnumerator CastSpells()
    {
        isCasting = true;
        //currentMana = currentMana - spellInventory[selectedSpell].manaCost;
        if (currentMana > 500)
        {
            selectedSpell = 0;
            Instantiate(spellInventory[selectedSpell].spellDisplay, castingPosition.position, castingPosition.rotation);
            currentMana = currentMana - spellInventory[selectedSpell].manaCost;
            yield return new WaitForSeconds(spellInventory[selectedSpell].Cooldown);
        }
        else if (currentMana <= 500 && currentMana > 250)
        {
            selectedSpell = 1;
            Instantiate(spellInventory[selectedSpell].spellDisplay, castingPosition.position, castingPosition.rotation);
            currentMana = currentMana - spellInventory[selectedSpell].manaCost;
            yield return new WaitForSeconds(spellInventory[selectedSpell].Cooldown);
        }
        else if (currentMana <= 250 && currentMana > 0)
        {
            selectedSpell = 2;
            Instantiate(spellInventory[selectedSpell].spellDisplay, castingPosition.position, castingPosition.rotation);
            currentMana = currentMana - spellInventory[selectedSpell].manaCost;
            yield return new WaitForSeconds(spellInventory[selectedSpell].Cooldown);
        }
        else if (currentMana <= 0)
        {
            yield return new WaitForSeconds(manaRegenerationTime + 5);
            maxMana = 750;
            currentMana = maxMana;
        }
        //yield return new WaitForSeconds(2);//spellInventory[selectedSpell].Cooldown);
        isCasting = false;
    }

    void KillTheBoy()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, playerDirection, out hit))
        {

            FacePlayer();
            if (hit.collider.CompareTag("Player") && !isCasting)
            {
                StartCoroutine(CastSpells());
            }
        }
    }
}
