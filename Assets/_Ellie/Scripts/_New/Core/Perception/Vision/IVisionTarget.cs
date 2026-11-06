using UnityEngine;

namespace Ellie.Core
{
    public interface IVisionTarget
    {
        public Faction Faction { get; }
        Transform Transform { get; }
        BoxCollider2D VisionCollider { get; }
        bool IsVisible { get; }
    }

    public enum Faction
    {
        Player,
        Infected,
        Animal

    }
}
