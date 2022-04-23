using Rpg.Control;
using Rpg.Core;
using Rpg.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rpg.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {

        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (!enabled) { return false; }

            Fighter playerFighter = callingController.GetComponent<Fighter>();
            if (!playerFighter.CanAttack(gameObject)) { return false; }

            if (Input.GetMouseButton(0))
            {
                playerFighter.Attack(gameObject);
            }

            return true;
        }
    }
}