using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manaPickup : MonoBehaviour
{
    [SerializeField] int manaAmount;
    [SerializeField] bool useButtonToPickUp;
    private bool playerInRange;


    private void Update()
    {
        pickUp();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    public void pickUp()
    {
        if (!useButtonToPickUp && playerInRange && gameManager.instance.playerScript.checkPlayerMana())
        {
            gameManager.instance.playerScript.pickupMana(manaAmount);
            Destroy(gameObject);
        }
        else if (playerInRange && useButtonToPickUp && Input.GetKeyDown(KeyCode.E) && gameManager.instance.playerScript.checkPlayerMana())
        {
            gameManager.instance.playerScript.pickupMana(manaAmount);
            Destroy(gameObject);
        }
    }
}