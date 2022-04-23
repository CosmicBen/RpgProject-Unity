using GameDevTV.Inventories;
using Rpg.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Rpg.Inventories
{
    public class RandomDropper : ItemDropper
    {
        [SerializeField] private float scatterDistance = 1.0f;
        [SerializeField] private DropLibrary dropLibrary;
        
        private const int ATTEMPTS = 30;
        private BaseStats myBaseStats = null;

        private BaseStats MyBaseStats
        {
            get
            {
                if (myBaseStats == null) { myBaseStats = GetComponent<BaseStats>(); }
                return myBaseStats;
            }
        }

        public void RandomDrop()
        {
            IEnumerable<DropLibrary.Dropped> drops = dropLibrary.GetRandomDrops(MyBaseStats.GetLevel());

            foreach (DropLibrary.Dropped drop in drops)
            {
                DropItem(drop.item, drop.number);
            }
        }

        protected override Vector3 GetDropLocation()
        {
            for (int i = 0; i < ATTEMPTS; ++i)
            {
                Vector3 randomPoint = transform.position + scatterDistance * Random.insideUnitSphere;

                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 0.1f, NavMesh.AllAreas))
                {
                    return randomPoint;
                }
            }

            return transform.position;
        }
    }
}