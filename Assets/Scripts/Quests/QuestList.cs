﻿using GameDevTV.Inventories;
using Rpg.Core;
using Rpg.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rpg.Quests
{
    public class QuestList : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        private List<QuestStatus> statuses = new List<QuestStatus>();
        public event Action onUpdate;

        public IEnumerable<QuestStatus> GetStatuses()
        {
            return statuses;
        }

        public void AddQuest(Quest quest)
        {
            if (HasQuest(quest)) { return; }

            QuestStatus newStatus = new QuestStatus(quest);
            statuses.Add(newStatus);

            onUpdate?.Invoke();
        }

        public bool HasQuest(Quest quest)
        {
            return GetQuestStatus(quest) != null;
        }

        public void CompleteObjective(Quest quest, string objective)
        {
            QuestStatus status = GetQuestStatus(quest);

            if (status != null)
            {
                status.CompleteObjective(objective);
            }

            if (status.IsComplete())
            {
                GiveReward(quest);
            }
                
            onUpdate?.Invoke();
        }

        private void GiveReward(Quest quest)
        {
            foreach (Quest.Reward reward in quest.GetRewards())
            {
                if (!reward.item.IsStackable())
                {
                    int given = 0;

                    for (int i = 0; i < reward.number; ++i)
                    {
                        bool isGiven = GetComponent<Inventory>().AddToFirstEmptySlot(reward.item, 1);
                        if (!isGiven) { break; }
                        given++;
                    }

                    if (given == reward.number) { continue; }

                    for (int i = given; i < reward.number; ++i)
                    {
                        GetComponent<ItemDropper>().DropItem(reward.item, 1);
                    }
                }
                else
                {
                    bool isGiven = GetComponent<Inventory>().AddToFirstEmptySlot(reward.item, reward.number);
                    if (!isGiven)
                    {
                        GetComponent<ItemDropper>().DropItem(reward.item, reward.number);
                    }
                }
            }
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

        public object CaptureState()
        {
            List<object> state = new List<object>();
            foreach (QuestStatus status in statuses)
            {
                state.Add(status.CaptureState());
            }
            return state;
        }

        public void RestoreState(object state)
        {
            List<object> stateList = state as List<object>;
            if (stateList == null) { return; }

            statuses.Clear();

            foreach (object objectState in stateList)
            {
                statuses.Add(new QuestStatus(objectState));
            }
        }

        public bool? Evaluate(string predicate, string[] parameters)
        {
            switch (predicate)
            {
                case "HasQuest":
                    return HasQuest(Quest.GetByName(parameters[0]));
                case "CompletedQuest":
                    return GetQuestStatus(Quest.GetByName(parameters[0])).IsComplete();
            }

            return null;
        }
    }
}