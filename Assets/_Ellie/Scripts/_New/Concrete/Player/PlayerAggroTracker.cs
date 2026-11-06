using Demo.Perception;
using Ellie.Core;
using System.Collections.Generic;
using UnityEngine;

namespace CarGame
{
    public class PlayerAggroTracker : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private float chaseDisapearTime = 10;

        private List<Transform> aggroedEnemies = new();

        private bool timerActive;
        private float timer;

        private void OnEnable()
        {
            AggroTracker.OnAggroChanged += OnAggroChanged;
        }

        private void OnDisable()
        {
            AggroTracker.OnAggroChanged -= OnAggroChanged;
        }

        private void Update()
        {
            if (timerActive)
            {
                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    MusicManager.Instance.StopChaseMusic();

                    timerActive = false;
                }
            }
        }

        private void OnAggroChanged(Transform enemy, Transform target)
        {
            if (player == null)
            {
                Debug.LogWarning("No player object set");
                return;
            }

            if (enemy.TryGetComponent(out Enemy e))
            {
                if (e.Faction == Faction.Animal)
                {
                    return;
                }
            }

            if (target == player.transform)
            {
                aggroedEnemies.Add(enemy);

                PlayChaseMusic();
            }
            else if (aggroedEnemies.Contains(enemy))
            {
                aggroedEnemies.Remove(enemy);

                if (aggroedEnemies.Count == 0)
                {
                    StartTimer();
                }
            }

            // Debug.Log("Aggro on Player: " + aggroedEnemies.Count);
        }

        private void PlayChaseMusic()
        {
            CancelTimer();

            MusicManager.Instance.PlayChaseMusic();
        }

        private void StartTimer()
        {
            timer = chaseDisapearTime;

            timerActive = true;
        }

        private void CancelTimer()
        {
            timerActive = false;
        }
    }
}
