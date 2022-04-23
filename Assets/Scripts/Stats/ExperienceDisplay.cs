using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rpg.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        [SerializeField] private Text experienceText = null;

        private Experience experience = null;

        private Experience PlayerExperience
        {
            get
            {
                if (experience == null) { experience = GameObject.FindWithTag("Player").GetComponent<Experience>(); }
                return experience;
            }
        }

        private void Update()
        {
            experienceText.text = string.Format("{0:0}", PlayerExperience.GetPoints());
        }
    }
}