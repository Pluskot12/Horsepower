using System;

namespace Ellie.Core.StateMachine
{
    [Serializable]
    public abstract class State<TContext>
    {
        public StateMachine<TContext> Machine { get; private set; }
        public TContext Context => Machine.Context;

        public void Init(StateMachine<TContext> machine)
        {
            Machine = machine;
        }

        internal virtual void Enter()
        {
            OnEnter();
        }

        internal virtual void Update(float dt)
        {
            OnUpdate(dt);
        }

        internal virtual void Exit()
        {
            OnExit();
        }

        protected virtual void OnEnter() { }
        protected virtual void OnUpdate(float dt) { }
        protected virtual void OnExit() { }
    }

}