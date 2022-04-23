using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rpg.Movement;
using Rpg.Combat;
using Rpg.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using GameDevTV.Inventories;

namespace Rpg.Control
{
    [RequireComponent(typeof(Mover))]
    public class PlayerController : MonoBehaviour
    {
        [Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] private CursorMapping[] cursorMappings = Array.Empty<CursorMapping>();
        [SerializeField] private float maxNavMeshProjectionDistance = 1.0f;
        [SerializeField] private float raycastRadius = 1.0f;

        private Camera mainCamera = null;
        private bool isDragging = false;

        private Mover myMover = null;
        private Fighter myFighter = null;
        private Health myHealth = null;
        private ActionStore myActionStore = null;

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

        private ActionStore MyActionStore
        {
            get
            {
                if (myActionStore == null) { myActionStore = GetComponent<ActionStore>(); }
                return myActionStore;
            }
        }

        private Camera MainCamera
        {
            get
            {
                if (mainCamera == null) { mainCamera = Camera.main; }
                return mainCamera;
            }
        }

        private void Update()
        {
            CheckSpecialAbilityKeys();

            if (InteractWithUi()) { return; }
            if (MyHealth.IsDead()) { SetCursor(CursorType.None); return; }
            if (InteractWithComponent()) { return; }
            if (InteractWithMovement()) { return; }

            SetCursor(CursorType.None);
        }

        private void CheckSpecialAbilityKeys()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) { MyActionStore.Use(0, gameObject); }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { MyActionStore.Use(1, gameObject); }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { MyActionStore.Use(2, gameObject); }
            if (Input.GetKeyDown(KeyCode.Alpha4)) { MyActionStore.Use(3, gameObject); }
            if (Input.GetKeyDown(KeyCode.Alpha5)) { MyActionStore.Use(4, gameObject); }
            if (Input.GetKeyDown(KeyCode.Alpha6)) { MyActionStore.Use(5, gameObject); }
        }

        private bool InteractWithUi()
        {
            bool overUi = EventSystem.current.IsPointerOverGameObject();

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
            {
                isDragging = overUi;
            }

            if (overUi)
            {
                SetCursor(CursorType.UI);
            }

            return overUi || isDragging;
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }

            return false;
        }

        private RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            float[] distances = new float[hits.Length];

            for (int i = 0; i < hits.Length; ++i)
            {
                distances[i] = Vector3.Distance(MainCamera.transform.position, hits[i].point);
            }

            Array.Sort(distances, hits);

            return hits;
        }

        // could probably make an iraycastable script on the terrain
        private bool InteractWithMovement()
        {
            if (RaycastNavMesh(out Vector3 target))
            {
                if (!MyMover.CanMoveTo(target)) { return false; }

                if (Input.GetMouseButton(0))
                {
                    MyMover.StartMoveAction(target, 1.0f);
                }

                SetCursor(CursorType.Movement);
                return true;
            }

            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = Vector3.zero;
            bool hasHit = Physics.Raycast(GetMouseRay(), out RaycastHit hit, float.MaxValue);
            if (!hasHit) { return false; }

            bool hasHitNavMesh = NavMesh.SamplePosition(hit.point, out NavMeshHit navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if (!hasHitNavMesh) { return false; }

            target = navMeshHit.position;

            return true;
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping mapping in cursorMappings)
            {
                if (mapping.type == type)
                {
                    return mapping;
                }
            }

            return cursorMappings[0];
        }

        private Ray GetMouseRay()
        {
            return MainCamera.ScreenPointToRay(Input.mousePosition);
        }
    }
}