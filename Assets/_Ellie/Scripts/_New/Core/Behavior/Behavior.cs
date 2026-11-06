using System;

namespace Ellie.Core.Behavior
{
    [Serializable]
    public abstract class Behavior<T>
    {
        internal virtual void Enter(T context)
        {
            OnEnter(context);
        }

        internal virtual void Exit(T context)
        {
            OnExit(context);
        }

        internal virtual void Update(T context, float dt)
        {
            OnUpdate(context, dt);
        }

        protected virtual void OnEnter(T context) { }
        protected virtual void OnExit(T context) { }
        protected virtual void OnUpdate(T context, float dt) { }
    }
}
