using GameDevTV.Utils;
using Rpg.Combat;
using Rpg.Core;
using Rpg.Movement;
using Rpg.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rpg.Control
{
    [RequireComponent(typeof(Fighter))]
    public class AiController : MonoBehaviour
    {
        private const string PLAYER_TAG = "Player";

        [SerializeField] private float chaseDistance = 5.0f;
        [SerializeField] private float suspicionTime = 2.0f;
        [SerializeField] private float agroCooldownTime = 5.0f;
        [SerializeField] private PatrolPath patrolPath = null;
        [SerializeField] private float waypointTolerance = 1.0f;
        [SerializeField] private float waypointDwellTime = 3.0f;

        [Range(0.0f, 1.0f)]
        [SerializeField] private float patrolSpeedFraction = 0.2f;
        [SerializeField] private float shoutDistance = 5.0f;

        private GameObject player;
        private ActionScheduler myActionScheduler;
        private Fighter myFighter;
        private Mover myMover;
        private Health myHealth;

        private LazyValue<Vector3> guardPosition;
        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private int currentWaypointIndex = 0;
        private float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        private float timeSinceAggrevated = Mathf.Infinity;

        private GameObject Player
        {
            get
            {
                if (player == null) { player = GameObject.FindGameObjectWithTag(PLAYER_TAG); }
                return player;
            }
        }

        private ActionScheduler MyActionScheduler
        {
            get
            {
                if (myActionScheduler == null) { myActionScheduler = GetComponent<ActionScheduler>(); }
                return myActionScheduler;
            }
        }

        private Fighter MyFighter
        {
            get
            {
                if (myFighter == null) { myFighter = GetComponent<Fighter>(); }
                return myFighter;
            }
        }

        private Mover MyMover
        {
            get
            {
                if (myMover == null) { myMover = GetComponent<Mover>(); }
                return myMover;
            }
        }

        private Health MyHealth
        {
            get
            {
                if (myHealth == null) { myHealth = GetComponent<Health>(); }
                return myHealth;
            }
        }

        private void Awake()
        {
            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }
        private void Start()
        {
            guardPosition.ForceInit();
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void Update()
        {
            if (MyHealth.IsDead()) { return; }
            if (Player == null) { return; }

            if (IsAggrevated() && MyFighter.CanAttack(Player))
            {
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        public void Aggrevate()
        {
            timeSinceAggrevated = 0.0f;
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition.value;

            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0.0f;
                    CycleWaypoint();
                }

                nextPosition = GetCurrentWaypoint();
            }

            if (timeSinceArrivedAtWaypoint > waypointDwellTime)
            {
                MyMover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
        }

        private bool AtWaypoint()
        {
            float waypointDistance = Vector3.Distance(GetCurrentWaypoint(), transform.position);
            return waypointDistance < waypointTolerance;
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void SuspicionBehaviour()
        {
            MyActionScheduler.CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0.0f;
            MyFighter.Attack(Player);

            AggrevateNearbyEnemies();
        }

        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0.0f);

            foreach (RaycastHit hit in hits)
            {
                AiController ai = hit.transform.GetComponent<AiController>();
                if (ai == null) { continue; }

                ai.Aggrevate();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }

        private bool IsAggrevated()
        {
            if (Player == null) { return false; }
            
            float distanceToPlayer = Vector3.Distance(Player.transform.position, transform.position);

            return distanceToPlayer <= chaseDistance || timeSinceAggrevated < agroCooldownTime;
        }
    }
}