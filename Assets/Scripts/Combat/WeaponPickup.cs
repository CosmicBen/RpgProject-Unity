using Rpg.Attributes;
using Rpg.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rpg.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] private WeaponConfig weapon = null;
        [SerializeField] private float healthToRestore = 0.0f; // health hack
        [SerializeField] private float respawnTime = 5.0f;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) { return; }

            Pickup(other.gameObject);
        }

        private void Pickup(GameObject subject)
        {
            if (weapon != null)
            {
                Fighter fighter = subject.GetComponent<Fighter>();
                Pickup(fighter);
            }

            if (healthToRestore > 0)
            {
                Health health = subject.GetComponent<Health>();
                health.Heal(healthToRestore);
            }

            StartCoroutine(HideForSeconds(respawnTime));
        }

        private void Pickup(Fighter fighter)
        {
            if (fighter == null) { return; }

            fighter.EquipWeapon(weapon);

            //StartCoroutine(HideForSeconds(respawnTime));
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        private void ShowPickup(bool shouldShow)
        {
            GetComponent<Collider>().enabled = shouldShow;
            
            for (int childId = 0; childId < transform.childCount; ++childId)
            {
                transform.GetChild(childId).gameObject.SetActive(shouldShow);
            }
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Pickup(callingController.gameObject);
            }

            return true;
        }
    }
}