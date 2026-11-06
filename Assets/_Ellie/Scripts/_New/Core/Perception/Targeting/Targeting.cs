using Ellie.Core.Perception;
using System;
using UnityEngine;

namespace Ellie.Core
{
    [Serializable]
    public struct TargetInfo
    {
        public float suspicion;
        public float lastStimulusTime;
        public Vector3 lastKnownPosition;
    }

    [Serializable]
    public abstract class Targeting<TContext>
    {
        public abstract void Tick(TContext context, float dt);
        public virtual void Dispose(IPerceptionContext context) { }
        public abstract void Reset();
    }
}
