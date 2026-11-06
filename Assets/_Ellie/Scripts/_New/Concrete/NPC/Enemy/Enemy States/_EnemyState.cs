using Ellie.Core.StateMachine;
using System;

namespace Demo.Perception
{
    [Serializable]
    public abstract class EnemyState : BehaviorState<EnemyContext>
    {
        //[SerializeReference] private List<Behavior<EnemyContext>> behavoirs;
    }
}
