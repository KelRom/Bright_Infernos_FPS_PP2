using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Dialog 
{
    public class DialogTrigger : MonoBehaviour
    {
        [SerializeField] string action;
        [SerializeField] UnityEvent onTrigger; 
        
        public void Trigger(string actionToTrigger)
        {
            if (actionToTrigger == action)
            {
                onTrigger.Invoke();
            }
        }
    }


}