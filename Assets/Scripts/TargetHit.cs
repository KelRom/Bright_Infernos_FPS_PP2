using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class TargetHit : MonoBehaviour
{
    [SerializeField] Animator animator;
    private void OnParticleCollision(GameObject other)
    {
        animator.SetBool("Target Hit", true);
    }
}
