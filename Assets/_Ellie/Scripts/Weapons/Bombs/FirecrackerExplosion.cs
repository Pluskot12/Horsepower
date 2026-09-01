using UnityEngine;

namespace CarGame
{
    public class FirecrackerExplosion : BombExplosion
    {
        [SerializeField] private float slowFactor = 0.1f;
        [SerializeField] private float slowDuration = 0.5f;

        protected override void ApplyEffect(Collider2D hit, IDamageable target, int damage)
        {
            base.ApplyEffect(hit, target, damage);

            ApplySlow(hit);
        }

        private void ApplySlow(Collider2D hit)
        {
            if (hit.attachedRigidbody.TryGetComponent<ISlowable>(out ISlowable target))
            {
                target.ApplySlow(slowFactor, slowDuration);
            }
        }
    }
}
