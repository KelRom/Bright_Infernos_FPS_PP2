using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class sword : MonoBehaviour

{
    [SerializeField] int damage;
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<IDamageable>() != null && other.GetType() != typeof(SphereCollider) && gameManager.instance.playerScript.playerSwinging())
        {
            GetComponent<Collider>().enabled = false;
            other.GetComponent<IDamageable>().takeDamage(damage);
        }
    }
}
