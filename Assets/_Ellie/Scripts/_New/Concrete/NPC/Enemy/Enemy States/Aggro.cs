using System;
using UnityEngine;

namespace Demo.Perception
{
    [Serializable]
    public class Aggro : EnemyState
    {
        protected override void OnEnter()
        {
            Context.IsAggro = true;

            Context.Slider.SetText("Aggro");
            Context.Slider.SetColor(Color.red);
        }

        protected override void OnExit()
        {
            Context.IsAggro = false;
        }
    }
}
