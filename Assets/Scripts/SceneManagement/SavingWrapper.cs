using Rpg.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rpg.SceneManagement
{
    [RequireComponent(typeof(SavingSystem))]
    public class SavingWrapper : MonoBehaviour
    {
        private const string defaultSaveFile = "save";

        [SerializeField] private bool clearLastSaveFile = false;
        [SerializeField] private float fadeInTime = 1.0f;

        private SavingSystem savingSystem = null;

        private SavingSystem SavingSystem
        {
            get
            {
                if (savingSystem == null) { savingSystem = GetComponent<SavingSystem>(); }
                return savingSystem;
            }
        }

        private void Awake()
        {
            if (clearLastSaveFile)
            {
                SavingSystem.Delete(defaultSaveFile);
            }

            StartCoroutine(LoadLastScene());
        }

        private IEnumerator LoadLastScene()
        {
            yield return SavingSystem.LoadLastScene(defaultSaveFile);
            
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();

            yield return fader.FadeIn(fadeInTime);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
        }

        public void Load()
        {
            SavingSystem.Load(defaultSaveFile);
        }

        public void Save()
        {
            SavingSystem.Save(defaultSaveFile);
        }
    }
}