using System;
using System.Collections.Generic;

namespace Ellie.Core.StateMachine
{
    public class StateMachine<TContext>
    {
        public TContext Context { get; private set; }

        public State<TContext> State => currentState;
        private State<TContext> currentState;

        private Dictionary<State<TContext>, List<Transition<TContext>>> transitions = new();
        List<Transition<TContext>> currentTransitions = new();
        List<Transition<TContext>> fromAnyTransitions = new();

        private bool started;

        public StateMachine(TContext context)
        {
            Context = context;
        }

        public void Start(State<TContext> state)
        {
            started = true;
            ChangeState(state);
        }

        public void Update(float dt)
        {
            if (!started)
            {
                return;
            }

            var transition = GetTransition();

            if (transition != null)
            {
                ChangeState(transition.To);
            }
            else
            {
                currentState.Update(dt);
            }
        }

        public void ChangeState(State<TContext> to)
        {
            if (to == null || currentState == to)
            {
                return;
            }

            currentState?.Exit();
            currentState = to;

            transitions.TryGetValue(to, out currentTransitions);

            currentState.Enter();
        }

        public void AddTransition(State<TContext> from, State<TContext> to, Func<bool> condition)
        {
            if (!transitions.TryGetValue(from, out var t))
            {
                t = new List<Transition<TContext>>();
                transitions[from] = t;
            }

            t.Add(new Transition<TContext>(to, condition));
        }

        public void AddFromAnyTransition(State<TContext> to, Func<bool> condition)
        {
            fromAnyTransitions.Add(new Transition<TContext>(to, condition));
        }

        private Transition<TContext> GetTransition()
        {
            foreach (var transition in fromAnyTransitions)
            {
                if (currentState == transition.To)
                {
                    continue;
                }

                if (transition.Condition())
                {
                    return transition;
                }
            }

            if (currentTransitions != null)
            {
                foreach (var transition in currentTransitions)
                {
                    if (currentState == transition.To)
                    {
                        continue;
                    }

                    if (transition.Condition())
                    {
                        return transition;
                    }
                }
            }

            return null;
        }

    }

}