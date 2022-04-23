using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rpg.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private Text healthText = null;

        private Health health = null;

        private Health PlayerHeath
        {
            get
            {
                if (health == null) { health = GameObject.FindWithTag("Player").GetComponent<Health>(); }
                return health;
            }
        }
        
        private void Update()
        {
            healthText.text = string.Format("{0:0}/{1:0}", PlayerHeath.GetHealthPoints(), PlayerHeath.GetMaxHealthPoints());
        }
    }
}