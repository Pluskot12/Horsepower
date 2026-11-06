using CarGame;
using Demo.Perception;
using UnityEngine;

namespace Horsepower.NPC
{
    public class SelfDestructWeaponController : WeaponController
    {

        [SerializeField] private Enemy self;
        [SerializeField] private Vector3 offset;
        [SerializeField] private float rayDistance = 0.3f;

        public override void Tick(IWeaponContext context, float dt)
        {
            if (context.WeaponsEnabled)
            {
                Attack();
            }
        }

        protected override void Attack()
        {
            if (CheckTargets())
            {
                self.OnDeath();
            }
        }

        private bool CheckTargets()
        {
            Transform t = self.transform;

            Vector3 origin = t.TransformPoint(offset);
            Vector3 facingDirection = t.TransformVector(Vector3.right).normalized;

            RaycastHit2D hit = Physics2D.Raycast(origin, facingDirection, rayDistance, hitLayer);
            Debug.DrawRay(origin, facingDirection * rayDistance);

            if (hit && hit.collider.attachedRigidbody)
            {
                if (hit.collider.attachedRigidbody.TryGetComponent(out Player player))
                {
                    return !player.Dash.IsDashing;
                }
            }
            return hit;
            /*
            if (hit.collider != null)
            {
                return true;
            }

            // Debug.DrawRay(transform.position, facingDirection * rayDistance, Color.red);

            return false;*/
        }

        private void OnDrawGizmos()
        {
            Transform t = self.transform;

            Vector3 origin = t.TransformPoint(offset);
            Vector3 facingDirection = t.TransformVector(Vector3.right).normalized;

            Gizmos.DrawRay(origin, facingDirection * rayDistance);
        }

    }
}
