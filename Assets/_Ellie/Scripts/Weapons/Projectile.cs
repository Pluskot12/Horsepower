using Ellie.Audio;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CarGame
{
    public class Projectile : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform visuals;
        [SerializeField] private SpriteRenderer spriteRenderer;


        [Header("Settings")]
        [SerializeField] private float bulletDrop = 0;
        [SerializeField] private float rotationSpeed = 5f;


        [Header("Triggers")]
        [SerializeField] private bool triggerOnHitEffects = true;
        [SerializeField] private bool destroyOnImpact = true;
        [SerializeField] private bool destroyOnImpactGround = true;

        [Space(10)]
        [Header("Audio")]
        [SerializeField] private AudioClip[] hitSounds;

        [Header("Events")]
        [SerializeField] private UnityEvent OnSpawn;
        [SerializeField] private UnityEvent OnImpact;

        HashSet<IDamageable> targets = new HashSet<IDamageable>();

        private LayerMask hitLayer;
        private int damage;

        private float time;

        private Vector3 initialPosition;
        private Vector3 initialVelocity;

        private Vector3 gravity;
        private Vector3 start;
        private Vector3 end;
        private Vector3 direction;

        private RaycastHit2D hit;
        private bool stopped;

        private float rotationOffset;

        public void Setup(Vector3 position, float rotationOffset, Vector3 velocity, int damage, float lifeTime, LayerMask mask)
        {
            this.rotationOffset = rotationOffset;

            Setup(position, velocity, damage, lifeTime, mask);
        }

        public void Setup(Vector3 position, Vector3 velocity, int damage, float lifeTime, LayerMask mask)
        {
            this.damage = damage;

            initialPosition = position;
            initialVelocity = velocity;
            time = 0;

            start = initialPosition;

            hitLayer = mask;

            OnSpawn?.Invoke();

            Destroy(gameObject, lifeTime);
        }

        public void SetDrawOrder(int order)
        {
            spriteRenderer.sortingOrder = order;
        }

        private void Update()
        {
            if (stopped)
            {
                return;
            }

            start = GetPosition();
            time += Time.deltaTime;
            end = GetPosition();

            Vector3 dir = end - start;

            if (bulletDrop != 0 && dir.sqrMagnitude > 0.0001f)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                Quaternion targetRot = Quaternion.Euler(0f, 0f, angle + rotationOffset);
                visuals.transform.rotation = Quaternion.Lerp(visuals.transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }

            RaycastSegment(start, end);
        }

        void RaycastSegment(Vector3 start, Vector3 end)
        {
            direction = end - start;
            float distance = direction.magnitude;

            hit = Physics2D.Raycast(start, direction, distance, hitLayer);
            if (hit)
            {
                if (CanBeDamaged(hit, out IDamageable target))
                {
                    target.TryDamage(damage, gameObject, triggerOnHitEffects);
                    targets.Add(target);
                }
                else if (destroyOnImpactGround)
                {
                    stopped = true;

                    Destroy(gameObject);
                }

                OnImpact?.Invoke();

                if (hitSounds.Length > 0)
                {
                    SoundManager.PlayRandomSFX(hitSounds, transform.position);
                }

                transform.position = hit.point;


                if (destroyOnImpact)
                {
                    stopped = true;
                    Destroy(gameObject);
                }
            }
            else
            {
                transform.position = end;
            }
        }

        private bool CanBeDamaged(RaycastHit2D hit, out IDamageable target)
        {
            if (hit.rigidbody != null)
            {
                if (hit.rigidbody.gameObject.TryGetComponent(out IDamageable t))
                {
                    if (!targets.Contains(t))
                    {
                        target = t;
                        return true;
                    }
                }
            }

            target = null;
            return false;
        }

        Vector3 GetPosition()
        {
            if (bulletDrop == 0)
                return initialPosition + (initialVelocity * time);

            gravity = Vector3.down * bulletDrop;
            return initialPosition + (initialVelocity * time) + (0.5f * time * time * gravity);
        }

    }
}

