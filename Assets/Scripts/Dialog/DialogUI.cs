using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialog;
using TMPro;
using UnityEngine.UI;

namespace UI
{


    public class DialogUI : MonoBehaviour
    {
        PlayerConversant playerConversant;
        [SerializeField] TextMeshProUGUI aiText;
        [SerializeField] Button nextButton;

        void Start()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            nextButton.onClick.AddListener(Next);
            UpdateUI();        
        }

        void Next() 
        {
            playerConversant.Next();
            UpdateUI();


        }

        void UpdateUI()
        {
            aiText.text = playerConversant.GetText();
            nextButton.gameObject.SetActive(playerConversant.HasNext());
        }
    }
}
