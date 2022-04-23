using Rpg.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rpg.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 1.0f;
        [SerializeField] private bool isHoming = false;
        [SerializeField] private GameObject hitEffect = null;
        [SerializeField] private float maxLifeTime = 10.0f;
        [SerializeField] private GameObject[] destroyOnHit = new GameObject[0];
        [SerializeField] private float lifeAfterImpact = 2.0f;
        [SerializeField] private UnityEvent onHit = null;

        private Health target = null;
        private float damage = 0.0f;
        private GameObject instigator = null;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        private void Update()
        {
            if (target == null) { return; }
            
            if (isHoming && !target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }

            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget(Health newTarget, GameObject instigator, float weaponDamage)
        {
            this.instigator = instigator;

            target = newTarget;
            damage = weaponDamage;

            Destroy(gameObject, maxLifeTime);
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();

            if (targetCapsule != null)
            {
                return target.transform.position + targetCapsule.center;
            }
            else
            {
                return target.transform.position;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform != target.transform) { return; }
            if (target.IsDead()) { return; }

            onHit?.Invoke();

            if (hitEffect != null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            target.TakeDamage(instigator, damage);
            speed = 0.0f;

            foreach (GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }

            Destroy(gameObject, lifeAfterImpact);
        }
    }
}