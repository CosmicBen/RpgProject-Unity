using GameDevTV.Inventories;
using Rpg.Attributes;
using Rpg.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rpg.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "RPG Project/Weapons/Make New Weapon")]
    public class WeaponConfig : EquipableItem, IModifierProvider
    {
        [SerializeField] private Weapon equippedPrefab = null;
        [SerializeField] private AnimatorOverrideController animatorOverride = null;

        [SerializeField] private float damage = 5.0f;
        [SerializeField] private float percentageBonus = 0.0f;
        [SerializeField] private float range = 2.0f;
        [SerializeField] private bool isRightHanded = true;
        [SerializeField] private Projectile projectile = null;

        private const string weaponName = "Weapon";
        
        public Weapon Spawn(Transform rightHandTransform, Transform leftHandTransform, Animator animator)
        {
            DestroyOldWeapon(rightHandTransform, leftHandTransform);
            Weapon weapon = null;

            if (equippedPrefab != null)
            {
                Transform handTransform = GetTransform(rightHandTransform, leftHandTransform);
                weapon = Instantiate(equippedPrefab, handTransform);
                weapon.gameObject.name = weaponName;
            }

            AnimatorOverrideController overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }

            if (oldWeapon == null) { return; }

            oldWeapon.name = "Destroyed Weapon";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetTransform(Transform rightHandTransform, Transform leftHandTransform)
        {
            return isRightHanded ? rightHandTransform : leftHandTransform;
        }

        public float GetRange()
        {
            return range;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, GameObject instigator, Health target, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public IEnumerable<float> GetAdditiveModifier(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return damage;
            }
        }

        public IEnumerable<float> GetPercentageModifier(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return percentageBonus;
            }
        }
    }
}