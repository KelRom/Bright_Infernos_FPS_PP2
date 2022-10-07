using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSphere : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GetComponentInParent<meleeEnemyAI>().SphereTriggerEnter(other);
    }
    private void OnTriggerExit(Collider other)
    {
        GetComponentInParent<meleeEnemyAI>().SphereTriggerExit(other);
    }
}
