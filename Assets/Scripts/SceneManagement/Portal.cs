using Rpg.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace Rpg.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        private enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        [SerializeField] private int sceneToLoad = -1;
        [SerializeField] private Transform spawnPoint = null;
        [SerializeField] private DestinationIdentifier destination = DestinationIdentifier.A;
        [SerializeField] private float fadeOutTime = 1.0f;
        [SerializeField] private float fadeInTime = 1.0f;
        [SerializeField] private float fadeWaitTime = 0.5f;

        private Fader fader = null;
        private SavingWrapper saveWrapper = null;

        private Fader Fader
        {
            get
            {
                if (fader == null) { fader = FindObjectOfType<Fader>(); }
                return fader;
            }
        }

        private SavingWrapper SaveWrapper
        {
            get
            {
                if (saveWrapper == null) { saveWrapper = FindObjectOfType<SavingWrapper>(); }
                return saveWrapper;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) { return; }

            StartCoroutine(Transition());
        }

        private IEnumerator Transition()
        {
            if (sceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set.");
                yield break;
            }

            PlayerController oldPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            oldPlayerController.enabled = false;

            DontDestroyOnLoad(gameObject);
            SaveWrapper.Save();

            Fader.FadeOut(fadeOutTime);
            yield return new WaitForSeconds(fadeOutTime);

            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            PlayerController newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;

            SaveWrapper.Load();

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            yield return new WaitForSeconds(fadeWaitTime);
            Fader.FadeIn(fadeInTime);

            newPlayerController.enabled = true;

            SaveWrapper.Save();

            Destroy(gameObject);
        }

        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) { continue; }
                if (portal.destination != destination) { continue; }

                return portal;
            }

            return null;
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            if (otherPortal == null) { return; }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            player.transform.rotation = spawnPoint.rotation;
        }
    }
}