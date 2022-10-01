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
                print("Save needs to be configured");
                //Save();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                print("Load needs to be configured");
                //Load();
            }
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }
    }

}
