using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour, IDamageable
{
    [SerializeField] CharacterController controller;

    [SerializeField] int HP;
    [SerializeField] float playerSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravityValue;

    [SerializeField] float fallThreshold;
    [SerializeField] int fallDamage;

    [SerializeField] int jumpsMax;

    [SerializeField] float shootRate;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] GameObject gunPos;
    
    [SerializeField] List<weaponStats> weaponInventory = new List<weaponStats>();

    int HPOriginal;
    float playerSpeedOriginal;
    int timesJumped;
    private Vector3 playerVelocity;
    Vector3 move;
    bool isShooting;
    float weaponZoomSpeed;
    float weaponFOV;
    float originalFOV;


    private void Start()
    {
        HPOriginal = HP;
        playerSpeedOriginal = playerSpeed;
        originalFOV = Camera.main.fieldOfView;
        playerRespawn();
    }

    void Update()
    {
        if (!gameManager.instance.isPaused)
        {
            movement();
            StartCoroutine(shoot());
        }
        //if (playerVelocity.y < -fallThreshold)
        //{
        //    takeFallDamage(fallDamage);
        //    playerVelocity.y = 0f;
        //}
    }

    void movement()
    {
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            timesJumped = 0;
        }

        move = (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (Input.GetButtonDown("Jump") && timesJumped < jumpsMax)
        {
            playerVelocity.y = jumpHeight;
            timesJumped++;
        }

        playerVelocity.y -= gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (Input.GetButton("Run"))
        {
            controller.Move((playerSpeed * 2) * Time.deltaTime * move);
        }
        else
        {
            controller.Move(playerSpeed * Time.deltaTime * move);
        }
    }

    IEnumerator shoot()
    {
        if (!isShooting && Input.GetButton("Shoot"))
        {
            isShooting = true;

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(.5f, .5f)), out hit, shootDistance))
            {
                if (hit.collider.GetComponent<IDamageable>() != null)
                {
                    hit.collider.GetComponent<IDamageable>().takeDamage(shootDamage);
                }
            }

            yield return new WaitForSeconds(shootRate);
            isShooting = false;
        }

        if (Input.GetButton("Zoom"))
            zoomWeapon();
        else
            unZoomWeapon();
    }

    public void takeDamage(int dmg)
    {
        HP -= dmg;
        updatePlayerHP();

        StartCoroutine(damageFlash());
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
        takeDamage(fallDamage);
    }

    public void pickup(weaponStats weapon)
    {
        shootDamage = weapon.damage;
        shootDistance = weapon.range;
        shootRate = weapon.fireRate;
        weaponFOV = weapon.weaponFOV;
        weaponZoomSpeed = weapon.weaponZoomSpeed;
        gunPos.GetComponent<MeshFilter>().sharedMesh = weapon.model.GetComponent<MeshFilter>().sharedMesh;
        gunPos.GetComponent<MeshRenderer>().sharedMaterial = weapon.model.GetComponent<MeshRenderer>().sharedMaterial;

        weaponInventory.Add(weapon);
    }

    private void zoomWeapon()
    {
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, weaponFOV, weaponZoomSpeed * Time.deltaTime); 
    }

    private void unZoomWeapon()
    {
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, originalFOV, weaponZoomSpeed * Time.deltaTime);
    }
}
