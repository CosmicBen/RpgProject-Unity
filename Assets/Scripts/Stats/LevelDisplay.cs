using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rpg.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        [SerializeField] private Text levelText = null;

        private BaseStats baseStats = null;

        private BaseStats BaseStats
        {
            get
            {
                if (baseStats == null) { baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>(); }
                return baseStats;
            }
        }

        private void Update()
        {
            levelText.text = string.Format("{0:0}", BaseStats.GetLevel());
        }
    }
}