using System;

namespace Ellie.Core.StateMachine
{
    public class Transition<T>
    {
        public State<T> To { get; private set; }
        public Func<bool> Condition { get; private set; }

        public Transition(State<T> to, Func<bool> condition)
        {
            To = to;
            Condition = condition;
        }
    }

}