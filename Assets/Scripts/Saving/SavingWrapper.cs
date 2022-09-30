using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saving 
{
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "save";

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                GetComponent<SavingSystem>().Save(defaultSaveFile);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                GetComponent<SavingSystem>().Load(defaultSaveFile);
            }
        }
    }

}
