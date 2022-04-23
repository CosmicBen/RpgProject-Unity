using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rpg.Quests
{
    [Serializable]
    public class QuestStatus
    {
        [SerializeField] private Quest quest;
        [SerializeField] private List<string> completedObjectives = new List<string>();

        [Serializable]
        class QuestStatusRecord
        {
            public string questName;
            public List<string> completedObjectives;
        }

        public QuestStatus(Quest quest)
        {
            this.quest = quest;
        }

        public QuestStatus(object statusObject)
        {
            QuestStatusRecord state = statusObject as QuestStatusRecord;
            if (state != null)
            {
                quest = Quest.GetByName(state.questName);
                completedObjectives = state.completedObjectives;
            }
        }

        public object CaptureState()
        {
            QuestStatusRecord state = new QuestStatusRecord();
            state.questName = quest.name;
            state.completedObjectives = completedObjectives;
            return state;
        }

        public Quest GetQuest()
        {
            return quest;
        }

        public int GetCompletedCount()
        {
            return completedObjectives.Count;
        }

        public bool IsObjectiveComplete(string objective)
        {
            return completedObjectives.Contains(objective);
        }

        public void CompleteObjective(string objective)
        {
            if (quest.HasObjective(objective))
            {
                completedObjectives.Add(objective);
            }
        }

        public bool IsComplete()
        {
            foreach (Quest.Objective objective in quest.GetObjectives())
            {
                if (!completedObjectives.Contains(objective.refrence))
                {
                    return false;
                }
            }

            return true;
        }
    }
}