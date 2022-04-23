using GameDevTV.Utils;
using Rpg.Core;
using Rpg.Saving;
using Rpg.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rpg.Attributes
{
    [RequireComponent(typeof(Animator))]
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private float regenerationPercentage = 70.0f;
        [SerializeField] private UnityEvent<float> onTakeDamage = null;
        [SerializeField] private UnityEvent onDie = null;

        private const string DIE_PARAM = "Die";

        private BaseStats myStats = null;
        private ActionScheduler myActionScheduler = null;
        private Animator myAnimator = null;
        private LazyValue<float> healthPoints;
        private bool isDead = false;

        private BaseStats MyStats
        {
            get
            {
                if (myStats == null) { myStats = GetComponent<BaseStats>(); }
                return myStats;
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

        private Animator MyAnimator
        {
            get
            {
                if (myAnimator == null) myAnimator = GetComponent<Animator>();
                return myAnimator;
            }
        }

        public bool IsDead() { return isDead; }

        private void Awake()
        {
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private void Start()
        {
            healthPoints.ForceInit();
        }

        private float GetInitialHealth()
        {
            return MyStats.GetStat(Stat.Health);
        }

        private void OnEnable()
        {
            if (MyStats != null) { MyStats.onLevelUp += RegenerateHealth; }
        }

        private void OnDisable()
        {
            if (MyStats != null) { MyStats.onLevelUp -= RegenerateHealth; }
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0.0f);

            if (healthPoints.value <= Mathf.Epsilon && !isDead)
            {
                onDie?.Invoke();
                Die();
                AwardExperience(instigator);
            }
            else
            {
                onTakeDamage?.Invoke(damage);
            }
        }

        public void Heal(float healthToRestore)
        {
            healthPoints.value = Mathf.Min(healthPoints.value + healthToRestore, GetMaxHealthPoints());
        }

        public float GetPercentage()
        {
            return 100.0f * (healthPoints.value / MyStats.GetStat(Stat.Health));
        }

        public float GetHealthPoints()
        {
            return healthPoints.value;
        }

        public float GetMaxHealthPoints()
        {
            return MyStats.GetStat(Stat.Health);
        }

        public float GetFraction()
        {
            return healthPoints.value / MyStats.GetStat(Stat.Health);
        }

        private void Die()
        {
            if (isDead) { return; }

            isDead = true;
            MyAnimator.SetTrigger(DIE_PARAM);
            MyActionScheduler.CancelCurrentAction();
        }

        private void AwardExperience(GameObject instigator)
        {
            if (instigator == null) { return; }

            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) { return; }

            experience.GainExperience(MyStats.GetStat(Stat.ExperienceReward));
        }

        private void RegenerateHealth()
        {
            float regeneratedHealthPoints = MyStats.GetStat(Stat.Health) * regenerationPercentage / 100;
            healthPoints.value = Mathf.Max(healthPoints.value, regeneratedHealthPoints);
        }

        public object CaptureState()
        {
            return healthPoints.value;
        }

        public void RestoreState(object state)
        {
            float health = (float)state;
            healthPoints.value = health;

            if (healthPoints.value <= Mathf.Epsilon)
            {
                Die();
            }
        }
    }
}