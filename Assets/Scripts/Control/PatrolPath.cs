using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rpg.Control
{
    public class PatrolPath : MonoBehaviour
    {
        private const float WAYPOINT_GIZMO_RADIUS = 0.3f;

        private void OnDrawGizmos()
        {
            for (int childId = 0; childId < transform.childCount; childId++)
            {
                Vector3 childPosition = GetWaypoint(childId);
                Vector3 nextPosition = GetWaypoint(GetNextIndex(childId));

                Gizmos.DrawSphere(childPosition, WAYPOINT_GIZMO_RADIUS);
                Gizmos.DrawLine(childPosition, nextPosition);
            }
        }

        public int GetNextIndex(int i)
        {
            return (i + 1) % transform.childCount;
        }
        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }
    }
}