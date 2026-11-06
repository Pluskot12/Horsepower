using CarGame;
using Ellie.Audio;
using Ellie.Core;
using Ellie.UI;
using Horsepower.NPC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Demo.Perception
{

    public class Enemy : Actor<EnemyContext>, IAttacker, IDamageable, IVisionTarget
    {

        [Serializable]
        public struct Stats
        {
            public int Health;
            public float IdleMoveSpeed;
            public float MaxMoveSpeed;
        }

        [SerializeField] private Stats stats;
        public Stats Stat => stats;

        [SerializeField] private CarGame.EnemyData data;
        [SerializeReference] private Ellie.Core.Perception.Vision vision;
        [SerializeReference] private Hearing hearing;
        [SerializeReference] private EnemyTargeting targeting;
        [SerializeReference] private MovementController movement;
        [SerializeReference] private Weapons weapons;

        [Header("Misc")]
        [SerializeField] private List<EnemyComponent> components;
        [SerializeField] private DamageSystem damageSystem;
        [SerializeField] private HitEffect hitEffect; // TODO: Move this to DamageSystem
        [SerializeField] private EnemyHoverUI hoverUI;
        [SerializeField] private Rigidbody2D body;
        public Rigidbody2D Rigidbody => body;
        [SerializeField] private BoxCollider2D visionCollider;

        [Header("Drops")]
        [SerializeField] private List<DropTable> dropTables;
        [SerializeField] private Faction faction;

        public bool IsIdle => context.IsIdle;
        public float MeterMax => 100f;

        public int MaxHealth { get => context.Stats.Health; }
        public int CurrentHealth { get => context.Health; set => context.Health = value; }
        public bool IsDead { get; private set; }

        public Transform Transform => transform;

        public BoxCollider2D VisionCollider => visionCollider;

        public bool IsVisible => true;

        public Faction Faction => faction;

        public Vector3 Velocity => body.linearVelocity;

        public event Action<Enemy, IAttacker> OnHitEvent;

        protected override void Awake()
        {
            context = new EnemyContext
            {
                Transform = transform,
                Faction = Faction,
                Attacker = this, // TODO: Change this
                Stats = stats,
                Slider = slider,
                Health = stats.Health
            };

            hearing?.Init(context);
            weapons?.Init(context);

            foreach (var component in components)
            {
                component.Init(context);
            }

            hoverUI.Init(context.Health);

            base.Awake();

            sortingGroup.sortingOrder = GetSortingOrder();
            controller.SetEnginePitch(UnityEngine.Random.Range(0f, 0.075f));
        }

        private Coroutine idleSoundsCoroutine;
        private void OnEnable()
        {
            TargetRegistry.Instance.Register(this);

            idleSoundsCoroutine = StartCoroutine(PlayIdleSound());

            hearing?.AddToRegistery();
        }

        private void OnDisable()
        {
            TargetRegistry.Instance.Unregister(this);

            StopCoroutine(idleSoundsCoroutine);

            hearing?.Dispose();
            targeting?.Dispose(context);

            foreach (var component in components)
            {
                component.Dispose(context);
            }

        }

        private void OnDestroy()
        {
            hoverUI?.Dispose();
            slider?.Dispose();
        }

        private void Update()
        {
            if (context.Disabled)
            {
                return;
            }

            float dt = Time.deltaTime;

            vision?.Tick(context, dt);
            hearing?.Tick(context, dt);
            targeting?.Tick(context, dt);
            brain?.Tick(context, dt);
            movement?.Tick(context, dt);
            weapons?.Tick(context, dt);

            foreach (var component in components)
            {
                component.Tick(context, dt);
            }

            slider.SetPercentage(context.AlertMeter / 100f);
        }

        public float GetSuspicion(Transform target)
        {
            return targeting.TargetData.TryGetValue(target, out TargetInfo info) ? info.suspicion : 0f;
        }

        public bool IsFullyAlerted(Transform target)
        {
            return targeting.AlertedTargets.Contains(target);
        }

        private void OnDrawGizmos()
        {
            vision?.DrawGizmos(context);
        }



        public void TryDamage(int damage, IAttacker attacker = null, bool triggerEffects = true)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);

            if (attacker == null)
            {
                Debug.LogWarning("No attacker");
            }
            else
            {
                context.SuspicionStimuli.Add(new Ellie.Core.Perception.Stimulus(attacker.Transform, attacker.Transform.position, float.MaxValue));
            }

            // Debug.Log("Damage " + damage + " target " + attacker);
            if (CurrentHealth <= 0)
            {
                OnDeath();
            }
            else
            {
                // Alert(attacker.transform.position, 1);
                OnHit(damage, attacker, triggerEffects);
            }
        }

        public void TryDamagePercentage(float percent, IAttacker attacker = null, bool triggerEffects = true)
        {
            int damage = Mathf.RoundToInt(MaxHealth * percent);
            TryDamage(damage, attacker, triggerEffects);
        }

        public void OnHit(int damage, IAttacker attacker, bool triggerEffects)
        {
            float percentage = (float)CurrentHealth / MaxHealth * 100f;
            damageSystem.UpdateSprite(percentage, triggerEffects);

            if (triggerEffects)
            {
                HitEffect effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            hoverUI.SetHpText(CurrentHealth);

            OnHitEvent?.Invoke(this, attacker);
        }
        private bool infected;

        public void SetInfected(bool infected)
        {
            this.infected = infected;
        }

        public void OnDeath()
        {
            if (!IsDead)
            {
                CurrentHealth = 0;

                IsDead = true;
                body.simulated = false;

                hoverUI?.Dispose();
                slider?.Dispose();

                damageSystem.OnDeath(infected);

                if (!infected)
                {
                    ItemSpawner.Instance.SpawnLoot(transform, dropTables);
                }

                StopAllCoroutines();
                Destroy(gameObject, 5f);

                RaiseOnDespawnEvent(this);
            }
        }

        internal void SetEnraged(Player player, float duration)
        {
            context.EnragedTimer = duration;
            context.SuspicionStimuli.Add(new Ellie.Core.Perception.Stimulus(player.transform, player.transform.position, float.MaxValue));
        }

        internal void DeAggro()
        {
            Debug.Log("Boink");

            targeting.Reset();

            context.SuspicionStimuli.Clear();
            context.AlertMeter = 0;
        }


        [SerializeField] private CarController controller;
        public CarController Controller => controller;
        internal void AlignToGround()
        {
            controller.AlignToGround(gameObject, 0f);
        }

        public EnemySaveData GetSaveData()
        {
            return new EnemySaveData
            {
                Id = data.Id,
                Health = CurrentHealth,
                Position = transform.position,
                Rotation = transform.rotation,
                Velocity = controller.Body.linearVelocity,
                AngularVelocity = controller.Body.angularVelocity,
            };

        }

        public void LoadData(EnemySaveData data)
        {
            CurrentHealth = data.Health;

            controller.Body.linearVelocity = data.Velocity;
            controller.Body.angularVelocity = data.AngularVelocity;
        }



        public void Knockback(Vector2 value) => controller.Knockback(value);

        [Header("Debugging")]
        public Slider slider;


        private IEnumerator PlayIdleSound()
        {
            while (!IsDead)
            {
                float random = UnityEngine.Random.Range(minIdleNoise, maxIdleNoise);

                yield return new WaitForSeconds(random);

                if (!IsDead/* && inIdleMode*/)
                {
                    SoundManager.PlayRandomSFX(idleSounds, transform.position);

                    if (faction != Faction.Animal)
                    {
                        EnemySpawnManager.Instance.OnNoiseGenerated(0.1f);
                    }

                    OnIdleSound?.Invoke();
                    OnIdleSoundEvent?.Invoke();
                }
            }
        }

        [Header("Idle")]
        [SerializeField] private AudioClip[] idleSounds;
        [SerializeField] private float minIdleNoise = 3;
        [SerializeField] private float maxIdleNoise = 5;
        public UnityEvent OnIdleSound;
        public event Action OnIdleSoundEvent;
        public static int sortingOrder = 0;

        [SerializeField] private SortingGroup sortingGroup;
        private int GetSortingOrder()
        {
            sortingOrder++;

            if (sortingOrder > 1000)
            {
                sortingOrder = 0;
            }

            return sortingOrder;
        }

        public void SetSortingOrder(int order)
        {
            sortingGroup.sortingOrder = sortingOrder - order;
        }

    }
}
