using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Dialog
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] string playerName;

        Dialog currentDialog = null;
        DialogNode currentNode = null;
        AIConversant currentConversant = null;
        bool isChoosing = false;

        public event Action onConverstationUpdated;

        public void StartDialog(AIConversant newConversant, Dialog newDialog) 
        {
            currentDialog = newDialog;
            currentNode = currentDialog.GetRootNode();
            currentConversant = newConversant;
            TriggerEnterAction();
            onConverstationUpdated();
            gameManager.instance.onRestart += Quit;
        }

        public bool IsCurrentDialogSkipable() 
        {
            return currentDialog.IsSkipable();
        }

        public void Quit()
        {
            currentDialog = null;
            TriggerExitAction();
            currentNode = null;
            currentConversant = null;
            isChoosing = false;
            onConverstationUpdated();
        }

        public bool IsActive() 
        {
            return currentDialog != null;
        }

        public bool IsChoosing() 
        {
            return isChoosing;
        }

        public string GetText() 
        {
           if(currentNode == null) 
           {
                return "";
           }

            return currentNode.GetText();
        }

        public IEnumerable<DialogNode> GetChoices() 
        {
            return currentDialog.GetPlayerChildren(currentNode);
        }

        public string GetCurrentConversantName()
        {
            if (isChoosing) 
            {
                return playerName;
            }
            else 
            {
                return currentConversant.GetName();
            }
        }

        public void SelectChoice(DialogNode chosenNode) 
        {
            currentNode = chosenNode;
            TriggerEnterAction();
            isChoosing = false;

            if (HasNext()) 
            {
                Next();
            }
            else 
            {
                Quit();
            }
        }

        public void Next() 
        {
            int numPlayerResponse = currentDialog.GetPlayerChildren(currentNode).Count();

            if(numPlayerResponse > 0) 
            {
                isChoosing = true;
                TriggerExitAction();
                onConverstationUpdated();
                return;
            }

            DialogNode[] children = currentDialog.GetAIChildren(currentNode).ToArray();
            TriggerExitAction();
            currentNode = children[UnityEngine.Random.Range(0, children.Count())];
            TriggerEnterAction();
            onConverstationUpdated();
        }

        public bool HasNext()
        {
            return currentDialog.GetAllChildren(currentNode).Count() > 0;
        }

        private void TriggerEnterAction()
        {
            TriggerAction(currentNode.GetOnEnterAction());
        }
        private void TriggerExitAction()
        {
            TriggerAction(currentNode.GetOnExitAction());   
        }

        private void TriggerAction(string action) 
        {
            if(action == "") 
            {
                return;
            }

            foreach(DialogTrigger trigger in currentConversant.GetComponents<DialogTrigger>()) 
            {
                trigger.Trigger(action);
            }
        }
    }
}