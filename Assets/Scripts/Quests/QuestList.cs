using Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Quests {

    public class QuestList : MonoBehaviour, ISaveable
    {
        List<QuestStatus> statuses = new List<QuestStatus>();
        public event Action addedQuest;

        public void AddQuest(Quest quest)
        {
            if (HasQuest(quest)) 
            {
                return;
            }
            QuestStatus newStatus = new QuestStatus(quest);
            statuses.Add(newStatus); 
            if (addedQuest != null)
            {
                addedQuest();
            }
        }

        public void CompleteObjective(Quest quest, string objective)
        {
            QuestStatus status = GetQuestStatus(quest);

            if (status != null)
            {
                status.CompleteObjective(objective);
                if (addedQuest != null)
                {
                    addedQuest();
                }
                if (quest.name == "Kill the wizard king" && quest.GetObjectiveCount() == GetQuestStatus(quest).GetCompletedCount())
                {
                    gameManager.instance.Win();
                }
            }
        }

        public bool HasQuest(Quest quest)
        {
            return GetQuestStatus(quest) != null;
        }

        private QuestStatus GetQuestStatus(Quest quest) 
        {
            foreach (QuestStatus status in statuses)
            {
                if (status.GetQuest() == quest)
                {
                    return status;
                }
            }

            return null;
        }

        public IEnumerable<QuestStatus> GetStatuses()
        {
            return statuses;
        }

        public object CaptureState()
        {
            print("saving list");
            List<object> state = new List<object>();
            foreach(QuestStatus status in statuses) 
            {
                state.Add(status.CaptureState());
            }

            return state;
        }

        public void RestoreState(object state)
        {
            print("loading list");
            List<object> stateList = state as List<object>;

            statuses.Clear();

            foreach(object objectState in stateList) 
            {
                statuses.Add(new QuestStatus(objectState));
            }

            if (addedQuest != null)
            {
                addedQuest();
            }
        }
    } 
}
