using Ellie.Core;
using Ellie.Core.Behavior;
using Ellie.Core.StateMachine;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Demo.Perception
{
    [Serializable]
    public class DefaultEnemyBrain : Brain<EnemyContext>
    {
        private StateMachine<EnemyContext> machine;

        [SerializeReference] private List<Behavior<EnemyContext>> idleBehavoirs;
        [SerializeReference] private List<Behavior<EnemyContext>> alertBehavoirs;
        [SerializeReference] private List<Behavior<EnemyContext>> aggroBehavoirs;
        [SerializeReference] private List<Behavior<EnemyContext>> enragedBehavoirs;

        private float alertThreshold = 100f;

        public State<EnemyContext> CurrentState => machine.State;

        private Idle idleState = new();
        private Alerted alertState = new();
        private Aggro aggroState = new();
        private Enraged enragedState = new();
        private Disabled disabledState = new();

        public override void Setup(EnemyContext context)
        {
            SetupStateMachine(context);
        }

        public override void Tick(EnemyContext context, float dt)
        {
            machine.Update(dt);
        }

        private void SetupStateMachine(EnemyContext context)
        {
            machine = new StateMachine<EnemyContext>(context);

            idleState.Init(machine);
            alertState.Init(machine);
            aggroState.Init(machine);
            enragedState.Init(machine);
            disabledState.Init(machine);

            idleState.SetBehavior(idleBehavoirs);
            alertState.SetBehavior(alertBehavoirs);
            aggroState.SetBehavior(aggroBehavoirs);
            enragedState.SetBehavior(enragedBehavoirs);

            machine.AddTransition(idleState, alertState, () => context.AlertMeter >= alertThreshold);
            machine.AddTransition(alertState, idleState, () => context.AlertMeter <= 0);

            machine.AddTransition(alertState, aggroState, () => context.IsTargetInView);
            machine.AddTransition(aggroState, alertState, () => !context.IsTargetInView);

            // Enraged
            machine.AddTransition(enragedState, aggroState, () => context.EnragedTimer <= 0);
            machine.AddFromAnyTransition(enragedState, () => context.EnragedTimer > 0);

            machine.AddTransition(disabledState, idleState, () => context.Health > 0);
            machine.AddFromAnyTransition(disabledState, () => context.Health <= 0);


            machine.Start(idleState);
        }
    }
}
