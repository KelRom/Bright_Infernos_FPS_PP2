using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class npcAI : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;

    Vector3 startingPos;
    [Range(1, 50)] [SerializeField] int roamRadius;

    private bool canRotate = true;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agent.velocity.normalized.magnitude, Time.deltaTime * 4));

        roam();
    }
    void roam()
    {
        agent.stoppingDistance = 0;

        Vector3 randomDir = Random.insideUnitSphere * roamRadius;
        randomDir += startingPos;

        NavMeshHit hit;

        NavMesh.SamplePosition(randomDir, out hit, 1, 1);
        NavMeshPath path = new NavMeshPath();

        if (hit.hit)
        {
            agent.CalculatePath(hit.position, path);
            agent.SetPath(path);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Fence") && canRotate) // If the animal collides with something that is not the ground, spin it around.
        {
            StartCoroutine(SpinMeRound());
        }
    }
    
        private IEnumerator SpinMeRound()
    {
        // Disable option to rotate.
        canRotate = false;

        //Move angle
        float moveAngle = Random.Range(45, 180);

        // Rotate animal.
        this.transform.rotation *= Quaternion.Euler(0, moveAngle, 0);

        // Wait...
        yield return new WaitForSeconds(1f);

        // Enable option to rotate.
        canRotate = true;
    }
}
