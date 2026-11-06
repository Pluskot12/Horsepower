using System;
using UnityEngine;

namespace Demo.Perception
{
    [Serializable]
    public class Idle : EnemyState
    {

        protected override void OnEnter()
        {
            Context.IsIdle = true;
            Context.MoveSpeed = Context.Stats.IdleMoveSpeed;
            Context.Slider.SetText("Idle");
            Context.Slider.SetColor(Color.green);
        }

        protected override void OnUpdate(float dt)
        {
            //  Debug.Log("Idling");
        }

        protected override void OnExit()
        {
            Context.IsIdle = false;
            Context.MoveSpeed = Context.Stats.MaxMoveSpeed;
        }
    }
}
