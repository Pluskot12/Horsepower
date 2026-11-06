using System;
using UnityEngine;

namespace Horsepower.NPC
{
    [Serializable]
    public abstract class Weapon
    {
        [Serializable]
        public struct Stats
        {
            public int damage;
            public float min;
            public float max;
        }

        [SerializeField] private WeaponController controller;
        [SerializeField] private Stats stats;

        protected IWeaponContext context;

        protected bool CanAttack => context.WeaponsEnabled && !context.Disabled;

        public virtual void Init(IWeaponContext context)
        {
            this.context = context;

            controller.Init(context);
        }

        public virtual void Tick(IWeaponContext context, float dt)
        {
            controller?.Tick(context, dt);
        }

    }
}
