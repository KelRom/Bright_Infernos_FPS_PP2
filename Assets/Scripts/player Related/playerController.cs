using Saving;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour, IDamageable
{
    [SerializeField] CharacterController controller;

    [SerializeField] Transform cam;
    [SerializeField] Collider swordCollider;

    [Header("-----Player Attributes-----")]
    [SerializeField] int HP;
    [SerializeField] float playerSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravityValue;
    [SerializeField] float sprintMultiplier;
    [SerializeField] float turnSmoothTime;

    public float knockbackForce;
    public float knockbackTime;
    private float knockbackCounter;
    //[SerializeField] float knockbackResistance

    [SerializeField] float fallTimeThreshold;
    [SerializeField] int fallDamage;
    float airTime;
    bool isJumping;
    bool isGrounded;
    int distToGround = 1;
    [SerializeField] bool isMeleeActive;

    [SerializeField] int jumpsMax;

    [Header("-----Magic Stats-----")]
    [SerializeField] float maxMana;
    [SerializeField] float currentMana;
    [SerializeField] Transform castingPos;


    [Header("-----Weapon Stats-----")]
    [SerializeField] GameObject gunPos;
    [SerializeField] int selectedSpell;
    [SerializeField] float swingRate;
    [SerializeField] List<spellStats> spellInventory = new List<spellStats>();

    [Header("-----Audio-----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] playerDamageSound;
    [Range(0, 1)] [SerializeField] float playerDamageVol;
    [Range(0, 1)] [SerializeField] float gunShootVol;
    [SerializeField] AudioClip[] playerJumpSound;
    [Range(0, 1)] [SerializeField] float playerJumpVol;
    [SerializeField] AudioClip[] playerFootstepsSound;
    [Range(0, 1)] [SerializeField] float playerFootstepsVol;
    [SerializeField] AudioClip healthPickup;
    [Range(0, 1)] [SerializeField] float healthPickupVol;

    bool playingFootsteps;

    [SerializeField] int HPOriginal;
    [SerializeField] float playerSpeedOriginal;
    [SerializeField] Animator animator;
    private int timesJumped;
    private Vector3 playerVelocity;
    private Vector3 move;

    private bool isSwinging;
    private bool isCasting;

    private bool isSprinting;
    private float turnSmoothVelocity;
    private void Start()
    {
        HPOriginal = HP;
        playerSpeedOriginal = playerSpeed;
        playerRespawn();
        currentMana = maxMana;
    }

    void Update()
    {
        if (!gameManager.instance.isPaused)
        {
            if (knockbackCounter <= 0)
            {
                movement();
                switchingSpells();
                StartCoroutine(footSteps());
                if (Input.GetButtonDown("Activate Melee"))
                {
                    isMeleeActive = true;
                    swordCollider.enabled = true;
                }
                else if (Input.GetButtonDown("Activate Spells"))
                {
                    isMeleeActive = false;
                    swordCollider.enabled = false;
                }

                if (!isSwinging && isMeleeActive)
                {
                    StartCoroutine(swing());
                }
                else if (!isCasting && !isMeleeActive && currentMana > 0 && currentMana >= spellInventory[selectedSpell].manaCost)
                {
                    StartCoroutine(cast());
                } 
            }
            else
            {
                knockbackCounter -= Time.deltaTime;
            }
        }
    }
    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, Vector3.down, distToGround + 0.1f))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    void movement()
    {
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            timesJumped = 0;
            isJumping = false;
        }
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        move = new Vector3(horizontal, 0f, vertical).normalized;

        if (move.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * Time.deltaTime * playerSpeed);
        }

        if (Input.GetButtonDown("Jump") && timesJumped < jumpsMax)
        {
            isJumping = true;
            playerVelocity.y = jumpHeight;
            timesJumped++;
            aud.PlayOneShot(playerJumpSound[Random.Range(0, playerJumpSound.Length)], playerJumpVol);
        }
        animator.SetInteger("TimesJumped", timesJumped);
        sprint();
        if (!isSprinting)
            animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), Mathf.Clamp(controller.velocity.normalized.magnitude, 0, .5f), Time.deltaTime * 5));
        else
            animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), Mathf.Clamp(controller.velocity.normalized.magnitude, .5f, 1), Time.deltaTime * 2));

        playerVelocity.y -= gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
        if (!isGrounded && !isJumping)
        {
            airTime += Time.deltaTime;
        }
        if (isGrounded)
        {
            if (airTime > fallTimeThreshold)
            {
                takeFallDamage(fallDamage);
                airTime = 0;
            }
        }
    }

    private void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            isSprinting = true;
            playerSpeed *= sprintMultiplier;

        }
        else if (Input.GetButtonUp("Sprint"))
        {
            isSprinting = false;
            playerSpeed = playerSpeedOriginal;
        }
    }

    IEnumerator swing()
    {
        if (!isSwinging && Input.GetButtonDown("Shoot"))
        {
            isSwinging = true;
            swordCollider.enabled = true;
            //Debug.Log("Swinging Sword");
            animator.SetInteger("SwordAttack", Random.Range(1, 4));
            
            yield return new WaitForSeconds(swingRate);
            isSwinging = false;
            animator.SetInteger("SwordAttack", 0);
        }
    }

    IEnumerator cast()
    {
        if (Input.GetButtonDown("Shoot"))
        {
            isCasting = true;
            currentMana -= spellInventory[selectedSpell].manaCost;
            updatePlayerMana();
            Instantiate(spellInventory[selectedSpell].spellDisplay, castingPos.position, castingPos.rotation);
            yield return new WaitForSeconds(spellInventory[selectedSpell].Cooldown);
            isCasting = false;
        }
    }

    public void switchingSpells()
    {
        if (Input.GetButtonDown("Next Spell") && selectedSpell < spellInventory.Count - 1)
        {
            selectedSpell++;
        }
        else if (Input.GetButtonDown("Previous Spell") && selectedSpell > 0)
            selectedSpell--;
    }

    public void takeDamage(int dmg)
    {
        HP -= dmg;
        updatePlayerHP();

        aud.PlayOneShot(playerDamageSound[Random.Range(0, playerDamageSound.Length)], playerDamageVol);

        StartCoroutine(damageFlash());

        knockback();
        if (HP <= 0)
        {
            gameManager.instance.playerIsDead();
        }

    }

    IEnumerator damageFlash()
    {
        gameManager.instance.playerDamage.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamage.SetActive(false);
    }

    public void playerRespawn()
    {
        controller.enabled = false;
        HP = HPOriginal;
        updatePlayerHP();
        transform.position = gameManager.instance.playerSpawnPoint.transform.position;
        controller.enabled = true;
    }

    public void knockback()
    {
        Vector3 hitDirection = -gameManager.instance.player.transform.forward;
        knockbackCounter = knockbackTime;
        playerVelocity.y = knockbackForce;
        controller.Move(hitDirection.normalized * knockbackForce);
    }

    public void updatePlayerHP()
    {
        gameManager.instance.HPBar.fillAmount = (float)HP / (float)HPOriginal;
    }
    public void takeFallDamage(int fallDamage)
    {
        //Debug.Log("fall damage");

        //takeDamage(fallDamage);
    }

    public void pickupHealth(int healAmount)
    {
        if ((HP += healAmount) > HPOriginal)
        {
            HP = HPOriginal;
        }
        aud.PlayOneShot(healthPickup, healthPickupVol);
        updatePlayerHP();
    }

    public void updatePlayerMana()
    {
        gameManager.instance.MPBar.fillAmount = (float)currentMana / (float)maxMana;
    }

    public void pickupMana(int manaAmount)
    {
        if ((currentMana += manaAmount) > maxMana)
        {
            currentMana = maxMana;
        }
        aud.PlayOneShot(healthPickup, healthPickupVol); //change sound
        updatePlayerMana();
    }

    public bool checkPlayerMana()
    {
        return currentMana < maxMana;
    }

    public bool checkPlayerHealth()
    {
        return HP < HPOriginal;
    }

    //public void switchWeapon()
    //{
    //    if (weaponInventory.Count > 1)
    //    {
    //        weaponInventory[selectedGun].currentGunCapacity = currentGunCapacity;
    //        weaponInventory[selectedGun].currentAmmoCount = currentAmmoCount;

    //        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < weaponInventory.Count - 1)
    //        {
    //            selectedGun++;
    //            shootRate = weaponInventory[selectedGun].fireRate;
    //            shootDistance = weaponInventory[selectedGun].range;
    //            shootDamage = weaponInventory[selectedGun].damage;
    //            currentGunCapacity = weaponInventory[selectedGun].currentGunCapacity;
    //            currentAmmoCount = weaponInventory[selectedGun].currentAmmoCount;
    //            maxGunCapacity = weaponInventory[selectedGun].maxGunCapacity;
    //            maxAmmoCount = weaponInventory[selectedGun].maxAmmoCount;

    //            gunPos.GetComponent<MeshFilter>().sharedMesh = weaponInventory[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
    //            gunPos.GetComponent<MeshRenderer>().sharedMaterial = weaponInventory[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;
    //        }
    //        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
    //        {
    //            selectedGun--;
    //            shootRate = weaponInventory[selectedGun].fireRate;
    //            shootDistance = weaponInventory[selectedGun].range;
    //            shootDamage = weaponInventory[selectedGun].damage;
    //            currentGunCapacity = weaponInventory[selectedGun].currentGunCapacity;
    //            currentAmmoCount = weaponInventory[selectedGun].currentAmmoCount;
    //            maxGunCapacity = weaponInventory[selectedGun].maxGunCapacity;
    //            maxAmmoCount = weaponInventory[selectedGun].maxAmmoCount;

    //            gunPos.GetComponent<MeshFilter>().sharedMesh = weaponInventory[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
    //            gunPos.GetComponent<MeshRenderer>().sharedMaterial = weaponInventory[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;
    //        }
    //    }
    //}

    IEnumerator footSteps()
    {
        if (!playingFootsteps && controller.isGrounded && move.normalized.magnitude > 0.3f)
        {
            playingFootsteps = true;
            aud.PlayOneShot(playerFootstepsSound[Random.Range(0, playerFootstepsSound.Length)], playerFootstepsVol);

            if (isSprinting)
            {
                yield return new WaitForSeconds(0.3f);
            }
            else
            {
                yield return new WaitForSeconds(0.4f);
            }
            playingFootsteps = false;

        }
    }

    public bool playerSwinging() { return isSwinging; }
}
