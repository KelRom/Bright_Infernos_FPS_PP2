using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponPickup : MonoBehaviour
{
    [SerializeField] weaponStats stats;
    [SerializeField] bool useButtonToPickUp;
    private bool playerInRange;


    private void Update()
    {
        pickUp();    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && useButtonToPickUp)
        {
            gameManager.instance.interactPopUpWindow.SetActive(true);
            playerInRange = true;
        }
        else if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.interactPopUpWindow.SetActive(false);
            playerInRange = false;
        }
    }


    public void pickUp()
    {
        if (!useButtonToPickUp && playerInRange)
        {
            gameManager.instance.playerScript.pickup(stats);
            Destroy(gameObject);
        }
        else if (playerInRange && useButtonToPickUp && Input.GetKeyDown(KeyCode.E))
        {
            gameManager.instance.playerScript.pickup(stats);
            Destroy(gameObject);
        }
    }
}