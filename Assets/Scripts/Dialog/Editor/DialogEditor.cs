using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Dialog.Editor
{
    public class DialogEditor : EditorWindow
    {
        Dialog selectedDialog = null;
        [NonSerialized] GUIStyle nodeStyle;
        [NonSerialized] GUIStyle playerNodeStyle;
        [NonSerialized] DialogNode draggingNode = null;
        [NonSerialized] Vector2 draggingOffset;
        [NonSerialized] DialogNode creatingNode = null;
        [NonSerialized] DialogNode deletingNode = null;
        [NonSerialized] DialogNode linkingParentNode = null;
        Vector2 scrollPos;
        [NonSerialized] bool draggingCanvas = false;
        [NonSerialized] Vector2 draggingCanvasOffset;
        const float canvasSize = 4000;
        const float backgroundSize = 50;


        [MenuItem("Window/Dialog Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogEditor), false, "Dialog Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) as Dialog != null)
            {
                ShowEditorWindow();
                return true;
            }
            return false;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChange;

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.normal.textColor = Color.white;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);

            playerNodeStyle = new GUIStyle();
            playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            playerNodeStyle.normal.textColor = Color.white;
            playerNodeStyle.padding = new RectOffset(20, 20, 20, 20);
            playerNodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void OnSelectionChange()
        {
            Dialog currentSelection = Selection.activeObject as Dialog;
            if (currentSelection != null) 
            {
                selectedDialog = currentSelection;
                Repaint();
            }
        }

        private void OnGUI()
        {
            if(selectedDialog == null) 
            {
                EditorGUILayout.LabelField("No Dialog Selected.");
            }
            else 
            {
                ProcessEvents();

                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

                Rect canvas = GUILayoutUtility.GetRect(canvasSize, canvasSize);
                Texture2D backgroundTex = Resources.Load("background") as Texture2D;
                Rect texCoords = new Rect(0, 0, canvasSize / backgroundSize, canvasSize / backgroundSize);
                GUI.DrawTextureWithTexCoords(canvas, backgroundTex, texCoords);
                

                foreach (DialogNode node in selectedDialog.GetAllNodes())
                {
                    DrawConnections(node);
                }

                foreach (DialogNode node in selectedDialog.GetAllNodes())
                {
                    DrawNode(node);
                }

                if (creatingNode != null)
                {
                    selectedDialog.CreateNode(creatingNode);
                    creatingNode = null;
                }

                EditorGUILayout.EndScrollView();

                if(deletingNode != null) 
                {
                    selectedDialog.DeleteNode(deletingNode);
                    deletingNode = null;
                }


            }
        }


        private void ProcessEvents()
        {
            if(Event.current.type == EventType.MouseDown && draggingNode == null) 
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPos);
                if(draggingNode != null) 
                {
                    draggingOffset = draggingNode.GetRect().position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;
                }
                else 
                {
                    draggingCanvas = true;
                    draggingCanvasOffset = Event.current.mousePosition + scrollPos;
                    Selection.activeObject = selectedDialog;
                } 
            }
            else if(Event.current.type == EventType.MouseDrag && draggingNode != null) 
            {
                draggingNode.setPos( Event.current.mousePosition + draggingOffset);
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseDrag && draggingCanvas) 
            {
                scrollPos = draggingCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggingNode != null) 
            {
                draggingNode = null;
            }
            else if (Event.current.type == EventType.MouseUp && draggingCanvas)
            {
                draggingCanvas = false;
            }

        }

        private void DrawNode(DialogNode node)
        {
            GUIStyle style = nodeStyle;
            if (node.IsPlayerSpeaking()) 
            {
                style = playerNodeStyle;
            }

            GUILayout.BeginArea(node.GetRect(), style);

            node.SetText(EditorGUILayout.TextField(node.GetText()));

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("+"))
            {
                creatingNode = node;
            }

            DrawLinkButtons(node);

            if (GUILayout.Button("x"))
            {
                deletingNode = node;
            }

            GUILayout.EndHorizontal();

            foreach (DialogNode childNode in selectedDialog.GetAllChildren(node))
            {
                EditorGUILayout.LabelField(childNode.GetText());
            }

            GUILayout.EndArea();
        }

        private void DrawLinkButtons(DialogNode node)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("Link"))
                {
                    linkingParentNode = node;
                }
            }
            else if(linkingParentNode == node) 
            {
                if(GUILayout.Button("Cancel"))
                {
                    linkingParentNode = null;
                }
            }
            else if(linkingParentNode.GetChildren().Contains(node.name)) 
            {
                if (GUILayout.Button("Unlink"))
                {
                    linkingParentNode.RemoveChild(node.name);
                    linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("Child"))
                {
                    linkingParentNode.AddChild(node.name);
                    linkingParentNode = null;
                }
            }
        }

        private void DrawConnections(DialogNode node)
        {
            Vector3 startPos = new Vector2(node.GetRect().xMax, node.GetRect().center.y);

            foreach (DialogNode childNode in selectedDialog.GetAllChildren(node)) 
            {
                Vector3 endPos = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y);
                Vector3 controlpointOffset = endPos - startPos;
                controlpointOffset.y = 0;
                controlpointOffset.x *= 0.8f;
                Handles.DrawBezier(
                    startPos, endPos, 
                    startPos + controlpointOffset, endPos - controlpointOffset, 
                    Color.white, null, 5f);
            }
        }

        private DialogNode GetNodeAtPoint(Vector2 point)
        {
            DialogNode foundNode = null;
            foreach(DialogNode node in selectedDialog.GetAllNodes()) 
            {
                if (node.GetRect().Contains(point)) 
                {
                    foundNode = node;
                }
            }
            return foundNode;
        }
    }
}
