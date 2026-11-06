
using CarGame;
using Demo.Perception;
using System;
using UnityEngine;

namespace Horsepower.NPC
{
    public class SuspicionIndicator : EnemyComponent
    {
        public static event Action<Enemy, float> OnSuspicionChanged;

        [SerializeField] private Enemy self;

        private Transform player;
        public override void Init(EnemyContext context)
        {
            player = GameManager.Instance.Player.transform;

            // canvas.transform.SetParent(null);
        }
        public override void Tick(EnemyContext context, float dt)
        {
            if (self == null || player == null)
            {
                return;
            }

            float suspicion = self.GetSuspicion(player);
            bool fullyAlerted = self.IsFullyAlerted(player);
            bool shouldShow = self.IsIdle && suspicion > 0f && !fullyAlerted;

            OnSuspicionChanged?.Invoke(self, shouldShow ? suspicion : 0f);
        }

        public override void Dispose(EnemyContext context)
        {
            OnSuspicionChanged?.Invoke(self, 0f);
            //Destroy(canvas.gameObject);
        }
    }
}
