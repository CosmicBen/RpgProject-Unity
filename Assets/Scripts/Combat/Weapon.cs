﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rpg.Combat
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private UnityEvent onHit = null;

        public void OnHit()
        {
            onHit?.Invoke();
        }
    }
}