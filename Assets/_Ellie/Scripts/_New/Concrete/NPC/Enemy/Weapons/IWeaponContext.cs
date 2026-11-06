using CarGame;
using UnityEngine;

namespace Horsepower.NPC
{
    public interface IWeaponContext
    {
        public bool Disabled { get; }
        public bool WeaponsEnabled { get; set; }
        public Transform Transform { get; set; }
        public IAttacker Attacker { get; set; }
        public Transform Target { get; set; }
    }
}
