using Ellie.Core;
using UnityEngine;

namespace CarGame
{
    public interface IDamageable
    {
        public int MaxHealth { get; }
        public int CurrentHealth { get; set; }

        public void TryDamage(int damage, IAttacker attacker, bool triggerEffects = true);
        public void TryDamagePercentage(float percent, IAttacker attacker, bool triggerEffects = true);

        public void OnHit(int damage, IAttacker attacker, bool triggerEffects);
        public void OnDeath();
    }

    public interface IAttacker
    {
        Transform Transform { get; }
        Faction Faction { get; }
    }
}