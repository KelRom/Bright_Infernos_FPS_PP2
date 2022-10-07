using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Timeline;

public class spell : MonoBehaviour
{
    public int damage;
    public int speed;
    public int destroyTime;
    private Vector3 playerLastLookDirection;
    private void Start()
    {
        playerLastLookDirection = gameManager.instance.player.transform.forward;
    }

    private void Update()
    { 
        transform.position += playerLastLookDirection * Time.deltaTime * speed;
    }

    private void OnParticleCollision(GameObject other)
    {
        if(other.GetComponent<IDamageable>() != null)
        {
            other.GetComponent<IDamageable>().takeDamage(damage);
            Destroy(gameObject);
        }
    }
}