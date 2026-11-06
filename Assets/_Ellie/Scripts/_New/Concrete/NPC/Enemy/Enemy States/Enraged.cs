using CarGame;
using System;
using UnityEngine;

namespace Demo.Perception
{
    [Serializable]
    public class Enraged : EnemyState
    {
        protected override void OnEnter()
        {
            Context.IsAggro = true;

            Context.Slider.SetText("Enraged");
            Context.Slider.SetColor(Color.purple);
        }

        protected override void OnUpdate(float dt)
        {
            Context.EnragedTimer -= Time.deltaTime;

            if (GameManager.Instance.Player.IsDead)
            {
                Context.EnragedTimer *= 0.5f;
            }
        }

        protected override void OnExit()
        {
            Context.IsAggro = false;
        }
    }
}
