using Demo.Perception;
using Ellie.Core.Behavior;
using System;
using UnityEngine;

namespace Horsepower.NPC
{
    [Serializable]
    public class Chase : Behavior<EnemyContext>
    {
        [SerializeField] private float stopDistance = 2.5f;
        private float randomStopDistance;
        private bool updateDistance;

        protected override void OnUpdate(EnemyContext context, float dt)
        {
            float dirToPlayer = context.LastKnownPosition.x - context.Transform.position.x;
            float absDist = Mathf.Abs(dirToPlayer);

            if (absDist > randomStopDistance)
            {
                float moveDir = Mathf.Sign(dirToPlayer);
                context.MoveTarget = Vector2.right * -moveDir;
                updateDistance = true;
            }

            if (updateDistance && absDist <= stopDistance * 0.25f)
            {
                randomStopDistance = stopDistance * UnityEngine.Random.Range(0.75f, 1.25f);
                updateDistance = false;
            }
        }
    }
}
