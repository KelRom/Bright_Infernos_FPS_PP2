using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialog;
using TMPro;
using UnityEngine.UI;
using System;

namespace UI
{


    public class DialogUI : MonoBehaviour
    {
        PlayerConversant playerConversant;
        [SerializeField] TextMeshProUGUI aiText;
        [SerializeField] Button nextButton;
        [SerializeField] GameObject AIResponse;
        [SerializeField] Transform choiceRoot;
        [SerializeField] GameObject choicePrefab;
        [SerializeField] Button quitButton;

        void Start()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            playerConversant.onConverstationUpdated += UpdateUI;
            gameManager.instance.onPauseToggle += UpdateUIPause;
            nextButton.onClick.AddListener(() => playerConversant.Next());
            quitButton.onClick.AddListener(() => playerConversant.Quit());

            UpdateUI();
        }

        void UpdateUIPause() 
        {
            if (gameManager.instance.isPaused)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(playerConversant.IsActive());
            }
        }
        void UpdateUI()
        {

            gameObject.SetActive(playerConversant.IsActive());

            if (!playerConversant.IsActive()) 
            {
                return;
            }


            AIResponse.SetActive(!playerConversant.IsChoosing());
            choiceRoot.gameObject.SetActive(playerConversant.IsChoosing());

            if (playerConversant.IsChoosing())
            {
                BuildChoiceList();
            }
            else 
            {
                aiText.text = playerConversant.GetText();
                nextButton.gameObject.SetActive(playerConversant.HasNext());
            }

            if(playerConversant.HasNext() == false) 
            {
                quitButton.gameObject.SetActive(true);
            }
            else if (!playerConversant.IsCurrentDialogSkipable()) 
            {
                quitButton.gameObject.SetActive(false);
            }
        }


        private void BuildChoiceList()
        {
            foreach (Transform item in choiceRoot)
            {
                Destroy(item.gameObject);
            }

            foreach (DialogNode choice in playerConversant.GetChoices())
            {
                GameObject choiceInstance = Instantiate(choicePrefab, choiceRoot);
                var textComp = choiceInstance.GetComponentInChildren<TextMeshProUGUI>();
                textComp.text = choice.GetText();
                Button button = choiceInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() => 
                {
                    playerConversant.SelectChoice(choice);
                });
            }
        }
    }
}
