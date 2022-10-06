using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class knockback : MonoBehaviour
{
    public float force = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void knockbackForce(Vector3 direction)
    {
        Vector3 pushDirection = gameManager.instance.player.transform.position - transform.position;

        pushDirection = -pushDirection.normalized;

        GetComponent<Rigidbody>().AddForce(pushDirection * force * 100);


        //knockbackCounter = knockbackTime;
        //moveDirection = direction * knockbackForce;
        //moveDirection.y = knockbackForce;
    }

}
