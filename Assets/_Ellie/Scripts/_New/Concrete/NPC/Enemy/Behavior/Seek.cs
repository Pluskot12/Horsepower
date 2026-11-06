using Demo.Perception;
using Ellie.Core.Behavior;
using System;
using UnityEngine;

namespace Horsepower.NPC
{
    [Serializable]
    public class Seek : Behavior<EnemyContext>
    {
        Vector3 lastPosition;

        protected override void OnEnter(EnemyContext context)
        {
            lastPosition = context.LastKnownPosition;
            float dirToTarget = context.LastKnownPosition.x - context.Transform.position.x;
            float moveDir = Mathf.Sign(dirToTarget);
            context.MoveTarget = Vector2.right * -moveDir;
        }

        protected override void OnUpdate(EnemyContext context, float dt)
        {
            if (lastPosition == context.LastKnownPosition)
            {
                return;
            }

            lastPosition = context.LastKnownPosition;
            float dirToTarget = context.LastKnownPosition.x - context.Transform.position.x;
            float moveDir = Mathf.Sign(dirToTarget);
            context.MoveTarget = Vector2.right * -moveDir;
        }
    }
}
