using Demo.Perception;
using Ellie.Core.Behavior;
using System;
using UnityEngine;

namespace Horsepower.NPC
{
    [Serializable]
    public class Wander : Behavior<EnemyContext>
    {
        [SerializeField] private float maxWanderDistance = 5;
        [SerializeField] private float moveDuration = 5;
        [SerializeField] private float moveRandom = 5;

        private Vector3 idlePoint;
        private float timer;
        private float pauseTimer;

        protected override void OnEnter(EnemyContext context)
        {
            idlePoint = context.Transform.position;
        }

        protected override void OnUpdate(EnemyContext context, float dt)
        {
            timer -= dt;

            if (timer <= 0)
            {
                if (pauseTimer > 0)
                {
                    context.MoveTarget = Vector2.zero;
                    context.StopMove = true;

                    pauseTimer -= dt;
                    return;
                }

                float direction = (UnityEngine.Random.value > 0.5f) ? 1f : -1f;

                float distFromStart = context.Transform.position.x - idlePoint.x;

                if (distFromStart > maxWanderDistance)
                {
                    direction = 1f;
                }
                else if (distFromStart < -maxWanderDistance)
                {
                    direction = -1f;
                }

                context.MoveTarget = Vector2.right * direction;

                timer = moveDuration + UnityEngine.Random.Range(0, moveRandom);
                pauseTimer = UnityEngine.Random.Range(1, 5);
            }
        }

        protected override void OnExit(EnemyContext context)
        {
            context.MoveTarget = Vector2.zero;
        }
    }
}