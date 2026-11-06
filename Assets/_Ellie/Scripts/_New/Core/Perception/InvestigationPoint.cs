using UnityEngine;

namespace Ellie.Core.Perception
{
    public readonly struct InvestigationPoint
    {
        public readonly Vector3 Position;
        public readonly float Priority;

        public InvestigationPoint(Vector3 position, float priority)
        {
            Position = position;
            Priority = priority;
        }
    }
}
