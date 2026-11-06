using CarGame;
using Ellie.Core;
using Ellie.Core.Perception;
using Ellie.UI;
using Horsepower.NPC;
using System.Collections.Generic;
using UnityEngine;

namespace Demo.Perception
{
    public class EnemyContext : IMovementContext, IPerceptionContext, IWeaponContext
    {
        public Transform Transform { get; set; }
        public Faction Faction { get; set; }
        public IAttacker Attacker { get; set; }
        public bool Disabled { get; set; }
        public Enemy.Stats Stats { get; set; }
        public bool IsIdle { get; set; }

        // Health (TODO: Add these to an interface)
        public int Health { get; set; } = 100;

        // Perception
        public Transform Target { get; set; }
        public Vector3 LastKnownPosition { get; set; }
        public float AlertMeter { get; set; }
        public float AlertMeter2 { get; set; }
        public List<Stimulus> SuspicionStimuli { get; set; } = new();
        public List<InvestigationPoint> InvestigationPoints { get; set; } = new();
        public bool IsTargetInView { get; set; }
        public bool IsAggro { get; set; }
        //public LayerMask ObstructionMask { get; set; } // Maybe not needed

        public bool WeaponsEnabled { get; set; }

        // Movement
        public Vector2 MoveTarget { get; set; }
        public float MoveSpeed { get; set; }
        public bool StopMove { get; set; }
        public float EnragedTimer { get; internal set; }

        // Debugging
        public Slider Slider;
    }
}
