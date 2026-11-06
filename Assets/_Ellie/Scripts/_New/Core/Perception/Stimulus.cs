using UnityEngine;

namespace Ellie.Core.Perception
{
    public readonly struct Stimulus
    {
        public readonly Transform Source;
        public readonly Vector3 Position;
        public readonly float Amount;

        public Stimulus(Transform source, Vector3 position, float amount)
        {
            Source = source;
            Position = position;
            Amount = amount;
        }
    }
}
