using System;
using UnityEngine;

namespace Demo.Perception
{
    [Serializable]
    public class Disabled : EnemyState
    {
        protected override void OnEnter()
        {
            Debug.Log("Disabled");
            Context.Disabled = true;
        }

        protected override void OnExit()
        {
            Debug.Log("Enabled");
            Context.Disabled = false;
        }
    }
}
