using Ellie.Core.Perception;
using System;
using UnityEngine;

namespace Ellie.Core
{
    [Serializable]
    public abstract class Hearing
    {
        protected IPerceptionContext context;

        public abstract void Tick(IPerceptionContext context, float dt);

        public virtual void Init(IPerceptionContext context)
        {
            this.context = context;
        }

        protected abstract void OnNoiseEmitted(Vector3 position, float loudness, Transform owner);

        public virtual void AddToRegistery()
        {
            NoiseManager.OnNoiseEmitted += OnNoiseEmitted;
        }

        public virtual void Dispose()
        {
            NoiseManager.OnNoiseEmitted -= OnNoiseEmitted;
        }
    }
}
