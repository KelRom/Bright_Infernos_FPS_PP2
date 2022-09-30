using Saving;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour, IDamageable, ISaveable
{
    [SerializeField] CharacterController controller;

    [Header("-----Player Attributes-----")]
    [SerializeField] int HP;
    [SerializeField] float playerSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravityValue;
    [SerializeField] float sprintMultiplier;
    [SerializeField] float knockbackResistance;
    float enemyKnockbackStrength = 10;

    [SerializeField] float fallTimeThreshold;
    [SerializeField] int fallDamage;
    float airTime;
    bool isJumping;
    bool isGrounded;
    int distToGround = 1;

    [SerializeField] int jumpsMax;

    [Header("-----Gun Stats-----")]
    [SerializeField] float shootRate;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] GameObject gunPos;
    public int maxAmmoCount;
    public int currentAmmoCount;
    public int maxGunCapacity;
    public int currentGunCapacity;
    int selectedGun;

    [SerializeField] List<weaponStats> weaponInventory = new List<weaponStats>();

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

    private bool isShooting;
    private float weaponZoomSpeed;
    private float weaponFOV;
    [SerializeField] float originalFOV;

    private bool isSprinting;
    private bool isReloading;

    private void Start()
    {
        HPOriginal = HP;
        playerSpeedOriginal = playerSpeed;
        playerRespawn();
    }

    void Update()
    {
        if (!gameManager.instance.isPaused)
        {
            movement();
            switchWeapon();
            StartCoroutine(footSteps());
            if(!isShooting) 
            {
                StartCoroutine(swing());
            }
            //else if(!isReloading && weaponInventory.Count > 0 && currentAmmoCount != 0) 
            //{
            //    StartCoroutine(reload());
            //}

            // Debug.Log(controller.isGrounded);
            // Debug.Log(isGrounded);
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

        move = (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * playerSpeed);
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
            animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), Mathf.Clamp(controller.velocity.normalized.magnitude,0, .5f), Time.deltaTime * 5));
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
        if (/*weaponInventory.Count > 0 && */!isShooting && Input.GetButton("Shoot"))
        {
            isShooting = true;
            animator.SetInteger("SwordAttack", Random.Range(1, 4));
          
            //aud.PlayOneShot(weaponInventory[selectedGun].sound, gunShootVol); //undo comment when weapon scroll is implemented

            //RaycastHit hit;
            //if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(.5f, .5f)), out hit, shootDistance))
            //{
            //    if (hit.collider.GetComponent<IDamageable>() != null)
            //        hit.collider.GetComponent<IDamageable>().takeDamage(shootDamage);

            //    Instantiate(weaponInventory[selectedGun].hitEffect, hit.point, transform.rotation);
            //}
            yield return new WaitForSeconds(shootRate);
            animator.SetInteger("SwordAttack", 0);
            isShooting = false;
        }

        //if (Input.GetButton("Zoom"))
        //    zoomWeapon();
        //else
        //    unZoomWeapon();
    }

    IEnumerator reload() 
    {

            isReloading = true;
            aud.PlayOneShot(weaponInventory[selectedGun].reload, gunShootVol);
            yield return new WaitForSeconds(weaponInventory[selectedGun].reload.length);
            if(currentAmmoCount < maxGunCapacity) 
            {
                currentGunCapacity = currentAmmoCount;
                currentAmmoCount -= currentAmmoCount;
            }
            else
            {
            currentGunCapacity = maxGunCapacity;
            currentAmmoCount -= maxGunCapacity; 
            }

            isReloading = false;
        
    }

    public void takeDamage(int dmg)
    {
        HP -= dmg;
        updatePlayerHP();
      
        aud.PlayOneShot(playerDamageSound[Random.Range(0, playerDamageSound.Length)], playerDamageVol);

        StartCoroutine(damageFlash());
        if (enemyKnockbackStrength > knockbackResistance)
        {
            //transform.position = gameManager.instance.playerKnockbackPoint.transform.position;
           // Debug.Log("knockback");
        }
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

    public void updatePlayerHP()
    {
        gameManager.instance.HPBar.fillAmount = (float)HP / (float)HPOriginal;
    }
    public void takeFallDamage(int fallDamage)
    {
        //Debug.Log("fall damage");

        //takeDamage(fallDamage);
    }

    public void pickup(weaponStats weapon)
    {
        shootDamage = weapon.damage;
        shootDistance = weapon.range;
        shootRate = weapon.fireRate;
        weaponFOV = weapon.weaponFOV;
        weaponZoomSpeed = weapon.weaponZoomSpeed;
        currentGunCapacity = weapon.currentGunCapacity;
        maxGunCapacity = weapon.maxGunCapacity;
        currentAmmoCount = weapon.currentAmmoCount;
        maxAmmoCount = weapon.maxAmmoCount;

        gunPos.GetComponent<MeshFilter>().sharedMesh = weapon.model.GetComponent<MeshFilter>().sharedMesh;
        gunPos.GetComponent<MeshRenderer>().sharedMaterial = weapon.model.GetComponent<MeshRenderer>().sharedMaterial;

        weaponInventory.Add(weapon);
        selectedGun = weaponInventory.Count - 1;
    }

    public void pickupHealth(int healAmount) 
    {
       if( (HP += healAmount) > HPOriginal) 
        {
            HP = HPOriginal;
        }
        aud.PlayOneShot(healthPickup, healthPickupVol);
        updatePlayerHP();
    }
    
    public void pickupAmmo() 
    {
        if((currentAmmoCount += (int)(maxAmmoCount * .2)) > maxAmmoCount)
        {
            currentAmmoCount = maxAmmoCount;
        }
    }

    public bool checkPlayerAmmo() 
    {
        return currentAmmoCount < maxAmmoCount;
    }

    public bool checkPlayerHealth() 
    {
        return HP < HPOriginal;
    }

    public void switchWeapon()
    {
        if (weaponInventory.Count > 1)
        {
            weaponInventory[selectedGun].currentGunCapacity = currentGunCapacity;
            weaponInventory[selectedGun].currentAmmoCount = currentAmmoCount;

            if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < weaponInventory.Count - 1)
            {
                selectedGun++;
                shootRate = weaponInventory[selectedGun].fireRate;
                shootDistance = weaponInventory[selectedGun].range;
                shootDamage = weaponInventory[selectedGun].damage;
                currentGunCapacity = weaponInventory[selectedGun].currentGunCapacity;
                currentAmmoCount = weaponInventory[selectedGun].currentAmmoCount;
                maxGunCapacity = weaponInventory[selectedGun].maxGunCapacity;
                maxAmmoCount = weaponInventory[selectedGun].maxAmmoCount;

                gunPos.GetComponent<MeshFilter>().sharedMesh = weaponInventory[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
                gunPos.GetComponent<MeshRenderer>().sharedMaterial = weaponInventory[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
            {
                selectedGun--;
                shootRate = weaponInventory[selectedGun].fireRate;
                shootDistance = weaponInventory[selectedGun].range;
                shootDamage = weaponInventory[selectedGun].damage;
                currentGunCapacity = weaponInventory[selectedGun].currentGunCapacity;
                currentAmmoCount = weaponInventory[selectedGun].currentAmmoCount;
                maxGunCapacity = weaponInventory[selectedGun].maxGunCapacity;
                maxAmmoCount = weaponInventory[selectedGun].maxAmmoCount;

                gunPos.GetComponent<MeshFilter>().sharedMesh = weaponInventory[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
                gunPos.GetComponent<MeshRenderer>().sharedMaterial = weaponInventory[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;
            }
        }
    }

    //private void zoomWeapon()
    //{
    //    Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, weaponFOV, weaponZoomSpeed * Time.deltaTime); 
    //}

    //private void unZoomWeapon()
    //{
    //    Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, originalFOV, weaponZoomSpeed * Time.deltaTime);
    // }
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

    public object CaptureState()
    {
        Dictionary<string, object> playerStats = new Dictionary<string, object>();

        playerStats.Add("health", HP);
        playerStats.Add("position", new SerializableVector3(transform.position));
        playerStats.Add("rotation", new SerializableVector3(transform.eulerAngles));

        return playerStats;
    }

    public void RestoreState(object state)
    {
        Dictionary<string, object> stateDict = (Dictionary<string, object>)state;

        SerializableVector3 position = (SerializableVector3)stateDict["position"];
        SerializableVector3 rotation = (SerializableVector3)stateDict["rotation"];

        HP = (int)stateDict["health"];

        controller.enabled = false;
        transform.position = position.toVector();
        transform.eulerAngles = rotation.toVector();
        controller.enabled = true;

    }
}
