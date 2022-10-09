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
            if (Input.GetKeyDown(KeyCode.V))
            {
                print("Saving...");
                Save();
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                print("loading...");
                Load();
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
