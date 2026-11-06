using Ellie.Core.Perception;
using System;
using UnityEngine;

namespace Ellie.Core
{
    [Serializable]
    public class SimpleVision : Vision
    {



        public override void Tick(IPerceptionContext context, float dt)
        {
            base.Tick(context, dt);

            foreach (var candidate in TargetRegistry.Instance.All)
            {
                if (CanSeeTarget(context, candidate))
                {
                    context.SuspicionStimuli.Add(new Stimulus(candidate.Transform, candidate.Transform.position, 1 * dt));
                }
            }
        }

        public override bool CanSeeTarget(IPerceptionContext context, IVisionTarget target)
        {
            return RaycastToTarget(context, target.Transform);
        }

        private bool RaycastToTarget(IPerceptionContext context, Transform target)
        {
            Vector3 direction = target.position - context.Transform.position;

            var hit = Physics2D.Raycast(context.Transform.position, direction.normalized, direction.magnitude, obstructionMask);

            Debug.DrawRay(context.Transform.position, direction.normalized * direction.magnitude, hit ? Color.red : Color.green);

            return !hit;
        }


    }
}
