using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dialog
{
    [CreateAssetMenu(fileName = " New Dialog", menuName = "Dialog", order = 0)]
    public class Dialog : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] List<DialogNode> nodes = new List<DialogNode>();
        [SerializeField] Vector2 newNodeOffset = new Vector2(250, 0);

        Dictionary<string, DialogNode> nodeLookup = new Dictionary<string, DialogNode>();

        private void OnValidate()
        {
            nodeLookup.Clear();
            foreach(DialogNode node in GetAllNodes()) 
            {
                if (node != null)
                {
                    nodeLookup[node.name] = node;
                }
            }
        }

        public IEnumerable<DialogNode> GetAllNodes() 
        {
            return nodes;
        }
        public IEnumerable<DialogNode> GetAllChildren(DialogNode parentNode)
        {
            foreach (string childID in parentNode.GetChildren())
            {
                if (nodeLookup.ContainsKey(childID))
                {
                    yield return nodeLookup[childID];
                }
            }
        }

        public IEnumerable<DialogNode> GetAIChildren(DialogNode currentNode)
        {
            foreach (DialogNode node in GetAllChildren(currentNode))
            {
                if (!node.IsPlayerSpeaking())
                {
                    yield return node;
                }
            }
        }

        public IEnumerable<DialogNode> GetPlayerChildren(DialogNode currentNode)
        {
            foreach(DialogNode node in GetAllChildren(currentNode)) 
            {
                if (node.IsPlayerSpeaking()) 
                {
                    yield return node;
                }
            }
        }


        public DialogNode GetRootNode()
        {
            return nodes[0];
        }

#if UNITY_EDITOR


        public void CreateNode(DialogNode parent)
        {
            DialogNode newNode = MakeNode(parent);
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialog Node");
            Undo.RecordObject(this, "Added New Dialog Node");
            AddNode(newNode);
        }

        public void DeleteNode(DialogNode nodeToDelete)
        {
            Undo.RecordObject(this, "Deleted Dialog Node");
            nodes.Remove(nodeToDelete);
            OnValidate();
            DeleteChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);

        }

        private void DeleteChildren(DialogNode nodeToDelete)
        {
            foreach (DialogNode node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete.name);
            }
        }
        private DialogNode MakeNode(DialogNode parent)
        {
            DialogNode newNode = CreateInstance<DialogNode>();
            newNode.name = Guid.NewGuid().ToString();
            if (parent != null)
            {
                parent.AddChild(newNode.name);
                newNode.SetPlayerSpeaking(!parent.IsPlayerSpeaking());
                newNode.setPos(parent.GetRect().position + newNodeOffset);
            }

            return newNode;
        }

        private void AddNode(DialogNode newNode)
        {
            nodes.Add(newNode);
            OnValidate();
        }
#endif
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (nodes.Count == 0)
            {
                DialogNode startingNode = MakeNode(null);
                AddNode(startingNode);
            }

            if (AssetDatabase.GetAssetPath(this) != "") 
          { 
                foreach(DialogNode node in GetAllNodes()) 
                {
                    if(AssetDatabase.GetAssetPath(node) == "") 
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
          }
#endif

        }

        public void OnAfterDeserialize()
        {
        }
    }
}