using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHit : MonoBehaviour
{
    [SerializeField] Animator animator;
    private void OnParticleCollision(GameObject other)
    {
        animator.SetTrigger("Target Hit");
    }

    private void OnParticleSystemStopped()
    {
        animator.SetTrigger("Target Hit");
    }
}
