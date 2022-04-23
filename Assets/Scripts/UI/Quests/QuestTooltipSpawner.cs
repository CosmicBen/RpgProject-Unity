using GameDevTV.Core.UI.Tooltips;
using Rpg.Quests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rpg.Ui.Quests
{
    public class QuestTooltipSpawner : TooltipSpawner
    {
        public override bool CanCreateTooltip()
        {
            return true;
        }

        public override void UpdateTooltip(GameObject tooltip)
        {
            QuestStatus status = GetComponent<QuestItemUi>().GetQuestStatus();
            tooltip.GetComponent<QuestTooltipUi>().Setup(status);
        }
    }
}