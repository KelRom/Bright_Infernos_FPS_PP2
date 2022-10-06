using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireball : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public Rigidbody rb;
    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.right * Time.deltaTime * 7; 
    }
}
