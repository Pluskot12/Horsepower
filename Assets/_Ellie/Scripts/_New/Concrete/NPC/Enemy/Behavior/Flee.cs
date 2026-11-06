using Demo.Perception;
using Ellie.Core.Behavior;
using System;
using UnityEngine;

namespace Horsepower.NPC
{
    [Serializable]
    public class Flee : Behavior<EnemyContext>
    {
        //[SerializeField] private float stopDistance = 2.5f;
        private float randomStopDistance;
        private bool updateDistance;

        protected override void OnUpdate(EnemyContext context, float dt)
        {
            float dirToPlayer = context.LastKnownPosition.x - context.Transform.position.x;
            //float absDist = Mathf.Abs(dirToPlayer);

            // if (absDist > randomStopDistance)
            {
                float moveDir = Mathf.Sign(dirToPlayer);
                context.MoveTarget = Vector2.right * moveDir;
                //updateDistance = true;
            }

        }
    }
}
