using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponPickup : MonoBehaviour
{
    [SerializeField] weaponStats stats;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.pickup(stats);
            Destroy(gameObject);
        }
    }
}