using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Rpg.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "RPG Project/Stats/New Progression")]
    public class Progression : ScriptableObject
    {
        [SerializeField] private ProgressionCharacterClass[] characterClasses = new ProgressionCharacterClass[0];

        [Serializable]
        private class ProgressionStat
        {
            [SerializeField] public Stat stat = Stat.Health;
            [SerializeField] public float[] levels = Array.Empty<float>();
        }

        [Serializable]
        private class ProgressionCharacterClass
        {
            [SerializeField] public CharacterClass characterClass = CharacterClass.Grunt;
            [SerializeField] public ProgressionStat[] stats = Array.Empty<ProgressionStat>();
        }

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookup();

            if (lookupTable.ContainsKey(characterClass) && 
                lookupTable[characterClass].ContainsKey(stat) &&
                level > 0 && level <= lookupTable[characterClass][stat].Length)
            {
                return lookupTable[characterClass][stat][level - 1];
            }

            return 0.0f;
        }

        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookup();

            if (lookupTable.ContainsKey(characterClass) && lookupTable[characterClass].ContainsKey(stat))
            {
                float[] levels = lookupTable[characterClass][stat];
                return levels.Length;
            }

            return 0;
        }

        private void BuildLookup()
        {
            if (lookupTable != null) return;

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass progressionClass in characterClasses)
            {
                if (lookupTable.ContainsKey(progressionClass.characterClass)) { continue; }

                Dictionary<Stat, float[]> statLookupTable = new Dictionary<Stat, float[]>();

                foreach (ProgressionStat progressionStat in progressionClass.stats)
                {
                    if (statLookupTable.ContainsKey(progressionStat.stat)) { continue; }

                    statLookupTable.Add(progressionStat.stat, progressionStat.levels);
                }

                lookupTable.Add(progressionClass.characterClass, statLookupTable);
            }
        }
    }
}