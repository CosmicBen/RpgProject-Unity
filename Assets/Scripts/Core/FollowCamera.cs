using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rpg.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;

        private void LateUpdate()
        {
            transform.position = target.position;
        }
    }
}