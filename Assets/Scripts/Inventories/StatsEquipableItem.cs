using GameDevTV.Inventories;
using Rpg.Stats;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rpg.Inventories
{
    [CreateAssetMenu(menuName = "RPG Project/Inventory/Equipable Item")]
    public class StatsEquipableItem : EquipableItem, IModifierProvider
    {
        [Serializable]
        private struct Modifier
        {
            public Stat stat;
            public float value;
        }

        [SerializeField] private Modifier[] additiveModifiers;
        [SerializeField] private Modifier[] percentabgeModifiers;

        public IEnumerable<float> GetAdditiveModifier(Stat stat)
        {
            foreach (Modifier modifier in additiveModifiers)
            {
                if (modifier.stat == stat)
                {
                    yield return modifier.value;
                }
            }
        }

        public IEnumerable<float> GetPercentageModifier(Stat stat)
        {
            foreach (Modifier modifier in percentabgeModifiers)
            {
                if (modifier.stat == stat)
                {
                    yield return modifier.value;
                }
            }
        }
    }
}