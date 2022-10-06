using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Dialog
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] Dialog currentDialog;
        DialogNode currentNode = null;

        private void Awake()
        {
            currentNode = currentDialog.GetRootNode();
        }

        public string GetText() 
        {
           if(currentNode == null) 
           {
                return "";
           }

            return currentNode.GetText();
        }

        public void Next() 
        {
            DialogNode[] children = currentDialog.GetAllChildren(currentNode).ToArray();
            currentNode = children[Random.Range(0, children.Count())];
        }

        public bool HasNext()
        {
            return currentDialog.GetAllChildren(currentNode).Count() > 0;
        }
    }
}