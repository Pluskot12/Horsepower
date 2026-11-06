using Demo.Perception;
using Ellie.Core.Behavior;
using System;
using UnityEngine;

namespace Horsepower.NPC
{
    [Serializable]
    public class IncreaseSpeed : Behavior<EnemyContext>
    {
        [SerializeField] private float maxSpeed = 9999f;
        [SerializeField] private float increasePerTick = 1f;
        [SerializeField] private float increaseTickTime = 1f;

        private float timer;

        protected override void OnUpdate(EnemyContext context, float dt)
        {
            timer += Time.deltaTime;

            if (timer >= increaseTickTime && context.MoveSpeed < maxSpeed)
            {
                context.MoveSpeed += increasePerTick;
                timer -= increaseTickTime;
            }

        }
    }
}
