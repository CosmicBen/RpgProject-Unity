using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rpg.Ui.DamageText
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] private Text damageText = null;

        public void DestroyText()
        {
            Destroy(gameObject, 3.0f);
        }

        public void SetValue(float damage)
        {
            damageText.text = damage.ToString();
        }
    }
}