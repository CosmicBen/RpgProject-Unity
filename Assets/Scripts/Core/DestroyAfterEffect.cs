using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rpg.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        [SerializeField] GameObject targetToDestroy = null;

        private ParticleSystem mySystem = null;

        private ParticleSystem MySystem
        {
            get
            {
                if (mySystem == null) { mySystem = GetComponent<ParticleSystem>(); }
                return mySystem;
            }
        }
        
        private void Update()
        {
            if (!MySystem.IsAlive())
            {
                if (targetToDestroy != null)
                {
                    Destroy(targetToDestroy);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}