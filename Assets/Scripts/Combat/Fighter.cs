using GameDevTV.Utils;
using Rpg.Core;
using Rpg.Movement;
using Rpg.Attributes;
using Rpg.Saving;
using Rpg.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Inventories;

namespace Rpg.Combat
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Mover))]
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        private const string ATTACK_PARAM = "Attack";
        private const string CANCEL_ATTACK_PARAM = "CancelAttack";

        [SerializeField] public float timeBetweenAttacks = 1f;
        [SerializeField] private Transform rightHandTransform = null;
        [SerializeField] private Transform leftHandTransform = null;
        [SerializeField] private WeaponConfig defaultWeapon = null;

        private Animator myAnimator = null;
        private ActionScheduler myActionScheduler = null;
        private BaseStats myStats = null;
        private Mover myMovement = null;
        private Equipment myEquipment = null;

        private WeaponConfig currentWeaponConfig = null;
        private LazyValue<Weapon> currentWeapon;
        private Health target = null;
        private float timeSinceLastAttack = Mathf.Infinity;

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

        private Mover MyMovement
        {
            get
            {
                if (myMovement == null) { myMovement = GetComponent<Mover>(); }
                return myMovement;
            }
        }

        private BaseStats MyStats
        {
            get
            {
                if (myStats == null) { myStats = GetComponent<BaseStats>(); }
                return myStats;
            }
        }

        private Equipment MyEquipment
        {
            get
            {
                if (myEquipment == null) { myEquipment = GetComponent<Equipment>(); }
                return myEquipment;
            }
        }

        private void Awake()
        {
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);

            if (MyEquipment != null)
            {
                MyEquipment.equipmentUpdated += UpdateWeapon;
            }
        }

        private void Start()
        {
            currentWeapon.ForceInit();
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }

        private void EquipWeapon(string weaponName)
        {
            if (string.IsNullOrEmpty(weaponName)) { return; }
            
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }

        private void UpdateWeapon()
        {
            WeaponConfig weapon = MyEquipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;

            if (weapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
            else
            {
                EquipWeapon(weapon);
            }
        }

        public Health GetTarget()
        {
            return target;
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            if (weapon == null) { return null; }
            return weapon.Spawn(rightHandTransform, leftHandTransform, MyAnimator);
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null) { return; }
            if (target.IsDead()) { return; }

            if (!GetIsInRange(target.transform))
            {
                MyMovement.MoveTo(target.transform.position, 1.0f);
            }
            else
            {
                MyMovement.Cancel();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);

            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                // This will trigger the Hit() event.
                TriggerAttack();
                timeSinceLastAttack = 0.0f;
            }
        }

        private void TriggerAttack()
        {
            MyAnimator.ResetTrigger(CANCEL_ATTACK_PARAM);
            MyAnimator.SetTrigger(ATTACK_PARAM);
        }

        private bool GetIsInRange(Transform target)
        {
            if (currentWeaponConfig == null) { return false; }

            return Vector3.Distance(transform.position, target.position) < currentWeaponConfig.GetRange();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) { return false; }
            if (!MyMovement.CanMoveTo(combatTarget.transform.position) && !GetIsInRange(combatTarget.transform)) { return false; }

            Health targetHealth = combatTarget.GetComponent<Health>();
            return targetHealth != null && !targetHealth.IsDead();
        }

        public void Attack(GameObject combatTarget)
        {
            MyActionScheduler.StartAction(this);   
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            MyMovement.Cancel();

            StopAttack();

            target = null;
        }

        private void StopAttack()
        {
            MyAnimator.ResetTrigger(ATTACK_PARAM);
            MyAnimator.SetTrigger(CANCEL_ATTACK_PARAM);
        }

        // Animation Events
        private void Hit()
        {
            if (target == null) { return; }

            float damage = MyStats.GetStat(Stat.Damage);

            if (currentWeapon.value != null)
            {
                currentWeapon.value.OnHit();
            }

            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, gameObject, target, damage);
            }
            else
            {
                target.TakeDamage(gameObject, damage);
            }
        }

        private void Shoot() { Hit(); }

        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = state as string;
            EquipWeapon(weaponName);
        }
    }
}