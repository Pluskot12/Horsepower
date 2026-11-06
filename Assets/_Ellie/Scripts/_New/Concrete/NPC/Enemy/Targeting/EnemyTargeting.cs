using Ellie.Core;
using Ellie.Core.Perception;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Demo.Perception
{
    [Serializable]
    public abstract class EnemyTargeting : Targeting<IPerceptionContext>
    {
        public readonly Dictionary<Transform, TargetInfo> TargetData = new();
        public readonly List<Transform> AlertedTargets = new();

        public override void Reset()
        {
            TargetData.Clear();
            AlertedTargets.Clear();
        }
    }
}
