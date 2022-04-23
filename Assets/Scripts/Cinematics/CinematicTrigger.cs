using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Rpg.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        private PlayableDirector myDirector = null;
        private bool alreadyTriggered = false;

        private PlayableDirector MyDirector
        {
            get
            {
                if (myDirector == null) { myDirector = GetComponent<PlayableDirector>(); }
                return myDirector;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (MyDirector == null || alreadyTriggered || !other.CompareTag("Player")) { return; }

            alreadyTriggered = true;
            MyDirector.Play();
        }
    }
}