using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace Dialog
{
    public class AIConversant : MonoBehaviour
    {
        [SerializeField] Dialog dialog;
        [SerializeField] bool useButtonToStartDialog;
        [SerializeField] bool repeatableDialog;
        [SerializeField] TextMeshProUGUI startConversation;
        [SerializeField] string conversantName;
        private bool playerInRange;
        private bool isDialogStarted;



        private void Update()
        {
            CheckDialog();

            if (gameManager.instance.playerScript.GetComponent<PlayerConversant>().IsActive()) 
            {
                gameManager.instance.cursorLockPause();
            }
            else if(!gameManager.instance.isPaused && !gameManager.instance.playerScript.GetComponent<PlayerConversant>().IsActive())
            {
                gameManager.instance.cursorUnlockUnpause();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player")) 
            {
                playerInRange = false;
            }
        }

        public void CheckDialog()
        {
            if(repeatableDialog)
            {
                StartDialog();
            }
            else if(!repeatableDialog && !isDialogStarted) 
            {
                StartDialog();
            }
        }

        public string GetName() 
        {
            return conversantName;
        }

        private void StartDialog()
        {
            if (playerInRange && useButtonToStartDialog && Input.GetKeyDown(KeyCode.E) && !gameManager.instance.playerScript.GetComponent<PlayerConversant>().IsActive())
            {
                gameManager.instance.player.GetComponent<PlayerConversant>().StartDialog(this,dialog);
                isDialogStarted = true;
            }
            else if (playerInRange && !useButtonToStartDialog && !gameManager.instance.playerScript.GetComponent<PlayerConversant>().IsActive())
            {
                gameManager.instance.player.GetComponent<PlayerConversant>().StartDialog(this,dialog);
                isDialogStarted = true;
            }

            if(startConversation != null) 
            {
                if (gameManager.instance.isPaused || !playerInRange)
                {
                    startConversation.gameObject.SetActive(false);
                }
                else if(playerInRange && useButtonToStartDialog)
                {
                    startConversation.gameObject.SetActive(true);
                }
            }
        }
    }
}
