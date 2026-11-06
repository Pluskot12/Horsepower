using System.Collections.Generic;
using UnityEngine;

namespace Ellie.Core.Perception
{
    public interface IPerceptionContext
    {
        public Transform Transform { get; set; }
        public Faction Faction { get; set; }
        public Transform Target { get; set; }
        public Vector3 LastKnownPosition { get; set; }
        public float AlertMeter { get; set; } // Rename to SuspisionMeter
        public float AlertMeter2 { get; set; } // Rename to SuspisionMeter
        public List<Stimulus> SuspicionStimuli { get; set; }
        public List<InvestigationPoint> InvestigationPoints { get; set; }
        public bool IsTargetInView { get; set; }
        bool IsAggro { get; set; }
        // public LayerMask ObstructionMask { get; set; }
    }
}
