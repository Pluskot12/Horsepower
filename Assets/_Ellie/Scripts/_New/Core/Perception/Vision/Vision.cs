using System;
using UnityEngine;

namespace Ellie.Core.Perception
{
    [Serializable]
    public abstract class Vision
    {
        [SerializeField] protected LayerMask normalObstructionMask;
        [SerializeField] protected LayerMask aggroObstructionMask;

        protected LayerMask obstructionMask;

        public virtual void Tick(IPerceptionContext context, float dt)
        {
            obstructionMask = context.IsAggro ? aggroObstructionMask : normalObstructionMask;
        }

        public abstract bool CanSeeTarget(IPerceptionContext context, IVisionTarget target);
        public virtual void DrawGizmos(IPerceptionContext context) { }
    }


}
