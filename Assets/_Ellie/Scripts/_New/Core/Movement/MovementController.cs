using System;
using UnityEngine;

namespace Ellie.Core
{
    [Serializable]
    public class MovementController
    {
        public virtual void Tick(IMovementContext context, float dt) { }
        public virtual void FixedTick(IMovementContext context) { }
    }

    public interface IMovementContext
    {
        public Vector2 MoveTarget { get; set; }
        public float MoveSpeed { get; set; }
        public bool StopMove { get; set; }
    }
}
