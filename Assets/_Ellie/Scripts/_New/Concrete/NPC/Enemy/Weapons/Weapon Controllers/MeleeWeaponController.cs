using CarGame;
using Ellie.Audio;
using UnityEngine;

namespace Horsepower.NPC
{
    public class MeleeWeaponController : WeaponController
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Transform origin;
        [SerializeField] private float attackRadius = 0.75f;
        [SerializeField] private AudioClip[] attackSounds;

        protected override void Attack()
        {
            animator.Play("Attack");
        }

        public void AttackTrigger()
        {
            Vector2 bitePos = (Vector2)origin.position;
            Collider2D hit = Physics2D.OverlapCircle(bitePos, attackRadius, hitLayer);

            if (hit != null)
            {
                if (hit.attachedRigidbody)
                {
                    if (hit.attachedRigidbody.TryGetComponent(out IDamageable damageable))
                    {
                        damageable.TryDamage(damage, attacker);

                        SoundManager.PlayRandomSFX(attackSounds, transform.position);
                    }
                }
                else
                {
                    if (hit.TryGetComponent(out IDamageable damageable))
                    {
                        damageable.TryDamage(damage, attacker);

                        SoundManager.PlayRandomSFX(attackSounds, transform.position);
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (origin == null)
            {
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(origin.position, attackRadius);
        }
    }
}
