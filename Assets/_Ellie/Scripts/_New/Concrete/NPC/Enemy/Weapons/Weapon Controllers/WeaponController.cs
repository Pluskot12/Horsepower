using CarGame;
using UnityEngine;

namespace Horsepower.NPC
{
    public abstract class WeaponController : MonoBehaviour
    {
        [Header("Base")]
        [SerializeField] protected LayerMask hitLayer;
        [SerializeField] protected int damage = 10;
        [SerializeField] protected float attackRateMin = 0.5f;
        [SerializeField] protected float attackRateMax = 1f;

        protected IAttacker attacker;
        protected IWeaponContext context;
        private float cooldown;

        public virtual void Init(IWeaponContext context)
        {
            this.attacker = context.Attacker;
            this.context = context;
        }

        public virtual void Tick(IWeaponContext context, float dt)
        {
            if (cooldown > 0)
            {
                cooldown -= dt;
            }

            if (cooldown <= 0 && context.WeaponsEnabled)
            {
                Attack();
                ResetCooldown();
            }
        }

        protected abstract void Attack();

        private void ResetCooldown()
        {
            cooldown = Random.Range(attackRateMin, attackRateMax);
        }
    }
}
