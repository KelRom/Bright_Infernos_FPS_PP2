using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Dialog
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] Dialog testDialog;
        Dialog currentDialog;
        DialogNode currentNode = null;
        bool isChoosing = false;

        public event Action onConverstationUpdated;

        IEnumerator Start() 
        {
            yield return new WaitForSeconds(2);
            StartDialog(testDialog);
        }

        public void StartDialog(Dialog newDialog) 
        {
            currentDialog = newDialog;
            currentNode = currentDialog.GetRootNode();
            onConverstationUpdated();
        }

        public void Quit()
        {
            currentDialog = null;
            currentNode = null;
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

        public void SelectChoice(DialogNode chosenNode) 
        {
            currentNode = chosenNode;
            isChoosing = false;
            Next();
        }

        public void Next() 
        {
            int numPlayerResponse = currentDialog.GetPlayerChildren(currentNode).Count();

            if(numPlayerResponse > 0) 
            {
                isChoosing = true;
                onConverstationUpdated();
                return;
            }

            DialogNode[] children = currentDialog.GetAIChildren(currentNode).ToArray();
            currentNode = children[UnityEngine.Random.Range(0, children.Count())];
            onConverstationUpdated();
        }

        public bool HasNext()
        {
            return currentDialog.GetAllChildren(currentNode).Count() > 0;
        }
    }
}