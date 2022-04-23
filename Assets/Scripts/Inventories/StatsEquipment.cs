using GameDevTV.Inventories;
using Rpg.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rpg.Inventories
{
    public class StatsEquipment : Equipment, IModifierProvider
    {
        public IEnumerable<float> GetAdditiveModifier(Stat stat)
        {
            foreach (EquipLocation slot in GetAllPopulatedSlots())
            {
                IModifierProvider item = GetItemInSlot(slot) as IModifierProvider;
                if (item == null) continue;

                foreach (float modifier in item.GetAdditiveModifier(stat))
                {
                    yield return modifier;
                }
            }
        }

        public IEnumerable<float> GetPercentageModifier(Stat stat)
        {
            foreach (EquipLocation slot in GetAllPopulatedSlots())
            {
                IModifierProvider item = GetItemInSlot(slot) as IModifierProvider;
                if (item == null) continue;

                foreach (float modifier in item.GetPercentageModifier(stat))
                {
                    yield return modifier;
                }
            }
        }
    }
}