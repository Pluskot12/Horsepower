using System;
using System.Collections.Generic;
using UnityEngine;

namespace Horsepower.NPC
{
    [Serializable]
    public class Weapons
    {
        [SerializeReference] private List<Weapon> weapons;

        public void Init(IWeaponContext context)
        {
            foreach (var weapon in weapons)
            {
                weapon.Init(context);
            }
        }

        public void Tick(IWeaponContext context, float dt)
        {
            foreach (var weapon in weapons)
            {
                weapon.Tick(context, dt);
            }
        }
    }
}
