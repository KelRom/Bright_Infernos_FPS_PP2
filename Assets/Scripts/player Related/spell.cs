using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spell : MonoBehaviour
{

    [SerializeField] Rigidbody rb;

    public int damage;
    public int speed;
    public int destroyTime;

    private void Start()
    {
        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IDamageable>() != null)
        {
            other.GetComponent<IDamageable>().takeDamage(damage);
        }
        Destroy(gameObject);
    }
}