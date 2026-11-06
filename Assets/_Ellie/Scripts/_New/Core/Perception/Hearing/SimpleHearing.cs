using Ellie.Core.Perception;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ellie.Core
{
    [Serializable]
    public class SimpleHearing : Hearing
    {
        [SerializeField] private float hearingRange = 15;



        private struct Noise
        {
            public Vector3 position;
            public float noiseLevel;
            public Transform owner;

            public Noise(Vector3 position, float loudness, Transform owner = null)
            {
                this.position = position;
                this.noiseLevel = loudness;
                this.owner = owner;
            }
        }

        private readonly List<Noise> pending = new();

        public override void Tick(IPerceptionContext context, float dt)
        {
            foreach (var noise in pending)
            {
                if (noise.owner == null)
                {
                    context.InvestigationPoints.Add(new InvestigationPoint(noise.position, noise.noiseLevel));
                }
                else
                {
                    context.SuspicionStimuli.Add(new Stimulus(noise.owner, noise.position, noise.noiseLevel));
                }
            }

            pending.Clear();
        }

        protected override void OnNoiseEmitted(Vector3 position, float loudness, Transform owner)
        {
            if (owner != null && owner == context.Transform)
            {
                return;
            }

            float distance = Vector3.Distance(context.Transform.position, position);

            if (distance <= hearingRange)
            {
                float stimilus = loudness * (1f - distance / hearingRange);
                pending.Add(new Noise(position, stimilus, owner));
            }
        }
    }
}
