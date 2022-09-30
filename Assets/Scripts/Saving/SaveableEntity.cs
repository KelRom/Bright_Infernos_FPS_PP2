using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Saving 
{
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] string uniqueIdentifier = "";
        public string GetUniqueIdentifier() 
        {
            return uniqueIdentifier;
        }

        public object CaptureState() 
        {
            return new SerializableVector3( transform.position);
        }

        public void RestoreState(object state) 
        {
            SerializableVector3 position = (SerializableVector3)state;

            gameManager.instance.playerScript.controller.enabled = false;
            //GetComponent<NavMeshAgent>().enabled = false;
            transform.position = position.toVector();
            gameManager.instance.playerScript.controller.enabled = true;
            //GetComponent<NavMeshAgent>().enabled = true;

        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.IsPlaying(gameObject)) 
                return;

            if (string.IsNullOrEmpty(gameObject.scene.path))
                return;


            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier");

            if ( string.IsNullOrEmpty(property.stringValue)) 
            {
                property.stringValue = System.Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            print("Editing");
        }
#endif
    }
}

