using Ellie.Core.Behavior;
using System.Collections.Generic;
using UnityEngine;

namespace Ellie.Core.StateMachine
{
    public abstract class BehaviorState<TContext> : State<TContext>
    {
        [SerializeReference] private List<Behavior<TContext>> behavoirs;

        public void SetBehavior(List<Behavior<TContext>> behavoirs)
        {
            this.behavoirs = behavoirs;
        }

        internal override void Enter()
        {
            base.Enter();

            // Context.StateLifeTime = 0;

            if (behavoirs == null)
            {
                return;
            }

            foreach (var behavior in behavoirs)
            {
                behavior.Enter(Context);
            }
        }

        internal override void Exit()
        {
            base.Exit();

            if (behavoirs == null)
            {
                return;
            }

            foreach (var behavior in behavoirs)
            {
                behavior.Exit(Context);
            }
        }

        internal override void Update(float dt)
        {
            base.Update(dt);

            //Context.StateLifeTime += dt;

            if (behavoirs == null)
            {
                return;
            }

            foreach (var behavior in behavoirs)
            {
                behavior.Update(Context, dt);
            }
        }
    }
}
