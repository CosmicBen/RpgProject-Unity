using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rpg.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Canvas canvas = null;
        [SerializeField] private RectTransform foreground = null;
        [SerializeField] private Health health = null;

        private void Update()
        {
            float healthFraction = health.GetFraction();

            if (Mathf.Approximately(healthFraction, 0.0f) || Mathf.Approximately(healthFraction, 1.0f))
            {
                canvas.enabled = false;
                return;
            }

            canvas.enabled = true;
            foreground.localScale = new Vector3(healthFraction, 1.0f, 1.0f);
        }
    }
}