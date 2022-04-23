using Rpg.Combat;
using Rpg.Core;
using Rpg.Attributes;
using Rpg.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Rpg.Movement
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Fighter))]
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] private float maxSpeed = 6.0f;
        [SerializeField] private float maxNavPathLength = 40.0f;

        private const string FORWARD_SPEED_PARAM = "ForwardSpeed";

        private ActionScheduler myActionScheduler = null;
        private NavMeshAgent myAgent = null;
        private Animator myAnimator = null;
        private Health myHealth = null;

        private NavMeshAgent MyAgent
        {
            get
            {
                if (myAgent == null) { myAgent = GetComponent<NavMeshAgent>(); }
                return myAgent;
            }
        }

        private Animator MyAnimator
        {
            get
            {
                if (myAnimator == null) { myAnimator = GetComponent<Animator>(); }
                return myAnimator;
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

        private Health MyHealth
        {
            get
            {
                if (myHealth == null) { myHealth = GetComponent<Health>(); }
                return myHealth;
            }
        }

        private void Update()
        {
            MyAgent.enabled = !MyHealth.IsDead();

            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            MyActionScheduler.StartAction(this);

            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            MyAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            MyAgent.destination = destination;
            MyAgent.isStopped = false;
        }

        public void Cancel()
        {
            MyAgent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = MyAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float forwardSpeed = localVelocity.z;

            MyAnimator.SetFloat(FORWARD_SPEED_PARAM, forwardSpeed);
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath) { return false; }
            if (path.status != NavMeshPathStatus.PathComplete) { return false; } // filters out nav mesh islands
            if (GetPathLength(path) > maxNavPathLength) { return false; }

            return true;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float length = 0.0f;

            for (int i = 0; i < path.corners.Length - 1; ++i)
            {
                length += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return length;
        }

        [Serializable]
        private struct MoverSaveData
        {
            public SerializableVector3 position;
            public SerializableVector3 eulerAngles;
        }

        public object CaptureState()
        {
            MoverSaveData data = new MoverSaveData();
            data.position = new SerializableVector3(transform.position);
            data.eulerAngles = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        public void RestoreState(object state)
        {
            MoverSaveData data = (MoverSaveData)state;
            transform.eulerAngles = data.eulerAngles.ToVector();
            MyAgent.Warp(data.position.ToVector());
        }
    }
}