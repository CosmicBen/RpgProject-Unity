using Rpg.Quests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rpg.Ui.Quests
{
    public class QuestListUi : MonoBehaviour
    {
        [SerializeField] private QuestItemUi questPrefab;
        private QuestList questList;

        private void Start()
        {
            questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            questList.onUpdate += Redraw;
            Redraw();
        }

        private void Redraw()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            foreach (QuestStatus status in questList.GetStatuses())
            {
                QuestItemUi uiInstance = Instantiate(questPrefab, transform);
                uiInstance.Setup(status);
            }
        }
    }
}