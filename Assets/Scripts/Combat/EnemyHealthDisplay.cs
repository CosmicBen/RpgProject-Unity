using Rpg.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rpg.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        [SerializeField] private Text healthText = null;

        private Fighter playerFighter = null;

        private Fighter PlayerFighter
        {
            get
            {
                if (playerFighter == null) { playerFighter = GameObject.FindWithTag("Player").GetComponent<Fighter>(); }
                return playerFighter;
            }
        }

        private void Update()
        {
            Health target = PlayerFighter.GetTarget();

            if (target == null)
            {
                healthText.text = "N/A";
            }
            else
            {
                healthText.text = string.Format("{0:0}/{1:0}", target.GetHealthPoints(), target.GetMaxHealthPoints());
            }
        }
    }
}