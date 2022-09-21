using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hostageAI : MonoBehaviour
{
    [SerializeField] int HP;


    void Update()
    {
        StartCoroutine(hostageDamage());
    }

    IEnumerator hostageDamage()
    {
        HP -= 1;
        yield return new WaitForSeconds(30);
        if (HP <= 0)
        {
            gameManager.instance.playerIsDead(); //make mission failed menu??
        }
    }
}
