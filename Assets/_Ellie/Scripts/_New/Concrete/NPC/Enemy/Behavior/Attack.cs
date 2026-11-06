using Demo.Perception;
using Ellie.Core.Behavior;
using System;

namespace Horsepower.NPC
{
    [Serializable]
    public class Attack : Behavior<EnemyContext>
    {
        protected override void OnEnter(EnemyContext context)
        {
            context.WeaponsEnabled = true;
        }

        protected override void OnExit(EnemyContext context)
        {
            context.WeaponsEnabled = false;
        }
    }
}
