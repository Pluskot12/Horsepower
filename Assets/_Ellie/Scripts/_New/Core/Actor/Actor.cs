using System;
using UnityEngine;

namespace Ellie.Core
{
    public abstract class Actor<TContext> : MonoBehaviour
    {
        protected TContext context;

        public event Action<Actor<TContext>> OnDespawnEvent;

        [SerializeReference] protected Brain<TContext> brain;

        protected virtual void Awake()
        {
            brain?.Setup(context);
        }

        protected void RaiseOnDespawnEvent(Actor<TContext> actor)
        {
            OnDespawnEvent?.Invoke(actor);
        }
    }
}
