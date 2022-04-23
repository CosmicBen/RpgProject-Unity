using Rpg.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Rpg.Saving
{
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] private string uniqueIdentifier = "";

#if UNITY_EDITOR

        static Dictionary<string, SaveableEntity> globalLookup = new Dictionary<string, SaveableEntity>();

        private void Update()
        {
            if (Application.IsPlaying(gameObject)) { return; }
            if (string.IsNullOrEmpty(gameObject.scene.path)) { return; }

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty(nameof(uniqueIdentifier));
            
            if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
            {
                property.stringValue = Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            if (!globalLookup.ContainsKey(property.stringValue))
            {
                globalLookup.Add(property.stringValue, this);
            }
        }

        private bool IsUnique(string candidate)
        {
            if (!globalLookup.ContainsKey(candidate) || globalLookup[candidate] == this) { return true; }

            if (globalLookup[candidate] == null || globalLookup[candidate].GetUniqueIdentifier() != candidate)
            {
                globalLookup.Remove(candidate);
                return true;
            }

            return false;
        }

#endif

        public string GetUniqueIdentifier()
        {
            return uniqueIdentifier;
        }

        public Dictionary<string, object> CaptureState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();

            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                string typeString = saveable.GetType().ToString();

                if (state.ContainsKey(typeString))
                {
                    Debug.LogError($"Can only save one {typeString} component per savable entity.");
                }
                else
                {
                    state.Add(typeString, saveable.CaptureState());
                }
            }

            return state;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> stateDictionary = (Dictionary<string, object>)state;

            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                string typeString = saveable.GetType().ToString();

                if (stateDictionary.ContainsKey(typeString))
                {
                    saveable.RestoreState(stateDictionary[typeString]);
                }
            }
        }
    }
}