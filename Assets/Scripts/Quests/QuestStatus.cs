using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quests
{
    public class QuestStatus
    {
        Quest quest;
        List<string> completeObjectives = new List<string>();


        [System.Serializable]
        class QuestStatusRecord 
        {
            public string questName;
            public List<string> completeObjectives;
        }

        public QuestStatus(Quest quest)
        {
            this.quest = quest;
        }
        public QuestStatus(object objectState)
        {
            QuestStatusRecord state = objectState as QuestStatusRecord;

            quest = Quest.GetByName(state.questName);
            completeObjectives = state.completeObjectives;
        }

        public Quest GetQuest() 
        {
            return quest;
        }

        public int GetCompletedCount() 
        {
            return completeObjectives.Count;
        }

        public bool IsObjectiveCompleted(string objective) 
        {
            return completeObjectives.Contains(objective);
        }

        public void CompleteObjective(string objective)
        {
            if(quest.HasObjective(objective))
            {
                completeObjectives.Add(objective);
            }
        }

        internal object CaptureState()
        {
            QuestStatusRecord state = new QuestStatusRecord();

            state.questName = quest.name;
            state.completeObjectives = completeObjectives;

            return state;
        }
    }
}
