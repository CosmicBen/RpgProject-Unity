using Rpg.Control;
using Rpg.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Rpg.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        private PlayableDirector myDirector = null;
        private GameObject player = null;

        private PlayableDirector MyDirector
        {
            get
            {
                if (myDirector == null) { myDirector = GetComponent<PlayableDirector>(); }
                return myDirector;
            }
        }

        private GameObject Player
        {
            get
            {
                if (player == null) { player = GameObject.FindGameObjectWithTag("Player"); }
                return player;
            }
        }

        private void OnEnable()
        {
            MyDirector.played += DisableControl;
            MyDirector.stopped += EnableControl;   
        }

        private void OnDisable()
        {
            MyDirector.played -= DisableControl;
            MyDirector.stopped -= EnableControl;
        }

        private void DisableControl(PlayableDirector director)
        {
            Player.GetComponent<ActionScheduler>().CancelCurrentAction();
            Player.GetComponent<PlayerController>().enabled = false;
        }

        private void EnableControl(PlayableDirector director)
        {
            Player.GetComponent<PlayerController>().enabled = true;
        }
    }
}