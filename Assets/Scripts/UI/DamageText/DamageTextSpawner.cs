using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rpg.Ui.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] private DamageText damageTextPrefab = null;

        public void Spawn(float damage)
        {
            DamageText instance = Instantiate(damageTextPrefab, transform);
            instance.SetValue(damage);
        }
    }
}