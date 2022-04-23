using GameDevTV.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rpg.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
        [SerializeField] private int startingLevel = 1;
        [SerializeField] private CharacterClass characterClass = CharacterClass.Grunt;
        [SerializeField] private Progression progression = null;
        [SerializeField] private GameObject levelUpParticleEffect = null;
        [SerializeField] private bool shouldUseModifier = false;

        private Experience myExperience = null;
        private LazyValue<int> currentlevel;

        public event Action onLevelUp;

        private Experience MyExperience
        {
            get
            {
                if (myExperience == null) { myExperience = GetComponent<Experience>(); }
                return myExperience;
            }
        }

        private void OnEnable()
        {
            if (MyExperience != null) { MyExperience.onExperienceGained += UpdateLevel; }
        }

        private void OnDisable()
        {
            if (MyExperience != null) { MyExperience.onExperienceGained -= UpdateLevel; }
        }

        private void Awake()
        {
            currentlevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            currentlevel.ForceInit();
        }

        private void UpdateLevel()
        {
            int newlevel = CalculateLevel();

            if (newlevel > currentlevel.value)
            {
                currentlevel.value = newlevel;
                onLevelUp?.Invoke();
                LevelUpEffect();
            }
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1.0f + GetPercentageModifier(stat) / 100.0f);
        }

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        private float GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseModifier) { return 0.0f; }

            float totalModifier = 0.0f;

            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifier(stat))
                {
                    totalModifier += modifier;
                }
            }

            return totalModifier;
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifier) { return 0.0f; }

            float totalModifier = 0.0f;

            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifier(stat))
                {
                    totalModifier += modifier;
                }
            }

            return totalModifier;
        }

        public int GetLevel()
        {
            return currentlevel.value;
        }

        public int CalculateLevel()
        {
            if (MyExperience == null) { return startingLevel; }

            float currentXp = MyExperience.GetPoints();
            int maxLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);

            for (int level = 1; level <= maxLevel; ++level)
            {
                float xpToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);

                if (xpToLevelUp > currentXp)
                {
                    return level;
                }
            }
            
            return maxLevel + 1;
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }
    }
}