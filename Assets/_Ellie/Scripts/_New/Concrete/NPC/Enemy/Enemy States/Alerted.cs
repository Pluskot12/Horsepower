using System;
using UnityEngine;

namespace Demo.Perception
{
    [Serializable]
    public class Alerted : EnemyState
    {/*
        public Alerted(StateMachine<EnemyContext> machine) : base(machine)
        {
        }*/

        protected override void OnEnter()
        {
            Context.Slider.SetText("Alerted");
            Context.Slider.SetColor(Color.yellow);
        }

        protected override void OnUpdate(float dt)
        {
        }
    }
}
