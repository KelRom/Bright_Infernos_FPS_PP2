using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class door : MonoBehaviour
{
    // Start is called before the first frame update

    bool playerInRange;
    bool openDoor;
    [SerializeField] Animator animator;
    [SerializeField] bool goToNextScene;

    private void Update()
    {
        StartCoroutine(openCloseDoor());   
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.interactPopUpWindow.SetActive(true);
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.interactPopUpWindow.SetActive(false);
            playerInRange = false;
        }

    }

    IEnumerator openCloseDoor()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInRange)
        {
            openDoor = !openDoor;
            animator.SetBool("openDoor", openDoor);
            if (goToNextScene)
            {
                yield return new WaitForSeconds(2f);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}
