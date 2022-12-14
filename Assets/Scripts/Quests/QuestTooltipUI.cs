using Quests;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestTooltipUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] Transform objectiveContainer;
    [SerializeField] GameObject objectivePrefab;
    [SerializeField] GameObject objectiveIncomplete;
    public void Setup(QuestStatus status) 
    {
        Quest quest = status.GetQuest();
        title.text = quest.GetTitle();

        foreach(Transform item in objectiveContainer) 
        {
            Destroy(item.gameObject);
        }

        foreach (string objective in quest.GetObjectives()) 
        {
            GameObject prefab = objectiveIncomplete;

            if (status.IsObjectiveCompleted(objective)) 
            {
                prefab = objectivePrefab;
            }

            GameObject objectiveInstance = Instantiate(prefab, objectiveContainer);
            TextMeshProUGUI objectiveText = objectiveInstance.GetComponentInChildren<TextMeshProUGUI>();
            objectiveText.text = objective;
        }
    }
}
