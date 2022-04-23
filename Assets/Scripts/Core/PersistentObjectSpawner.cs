using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rpg.Core
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject persistentObjectPrefab = null;

        private static bool hasSpawned = false;

        private void Awake()
        {
            if (hasSpawned) { return; }

            SpawnPersistenObjects();

            hasSpawned = true;
        }

        private void SpawnPersistenObjects()
        {
            GameObject persistentObject = Instantiate(persistentObjectPrefab);
            DontDestroyOnLoad(persistentObject);
        }
    }
}