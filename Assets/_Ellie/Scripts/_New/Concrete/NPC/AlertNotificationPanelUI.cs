using Demo.Perception;
using Horsepower.NPC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CarGame
{
    public class AlertNotificationPanelUI : MonoBehaviour
    {
        [SerializeField] private AlertNotificationIndicatorUI[] indicators;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private float fadeInDuration = 0.5f;
        [SerializeField] private float fadeOutDuration = 0.5f;

        List<AlertNotificationIndicatorUI> activeIndicators = new();

        Stack<AlertNotificationIndicatorUI> freeIndicators = new();

        Dictionary<Enemy, AlertNotificationIndicatorUI> trackedEnemies = new();

        bool isPlaying;

        private void Awake()
        {
            freeIndicators = new Stack<AlertNotificationIndicatorUI>();

            foreach (AlertNotificationIndicatorUI indicator in indicators)
            {
                freeIndicators.Push(indicator);
            }


        }

        private void OnEnable()
        {
            SuspicionIndicator.OnSuspicionChanged += OnSuspicionChanged;
            // old
            // EnemyController.OnAggroIncrease += OnSuspicionChanged;
            // EnemyController.OnDeathEvent += EnemyController_OnDeathEvent;
        }



        private void OnDisable()
        {
            SuspicionIndicator.OnSuspicionChanged -= OnSuspicionChanged;
            // old
            // EnemyController.OnAggroIncrease -= OnSuspicionChanged;
            // EnemyController.OnDeathEvent -= EnemyController_OnDeathEvent;
        }

        private void OnSuspicionChanged(Enemy enemy, float suspicion)
        {
            if (suspicion == 0)
            {
                TryRemove(enemy);
            }
            else
            {
                TryActivate(enemy, suspicion);
            }
        }

        private void Update()
        {
            if (trackedEnemies.Count > 0 && !isPlaying)
            {
                isPlaying = true;
            }
            else if (trackedEnemies.Count == 0 && isPlaying)
            {
                isPlaying = false;
            }

            float targetVolume = isPlaying ? 1f : 0f;
            float duration = isPlaying ? fadeInDuration : fadeOutDuration;

            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, Time.deltaTime / duration);

            if (!isPlaying && audioSource.volume == 0f && audioSource.isPlaying)
            {
                //audioSource.Stop();
            }
        }

        public void RemoveDeadEnemies()
        {
            var deadKeys = trackedEnemies.Keys.Where(k => k == null).ToList();
            foreach (var key in deadKeys)
            {
                trackedEnemies[key].Deactivate(Return);
                trackedEnemies.Remove(key);
            }
        }
        public void TryActivate(Enemy target, float vision)
        {
            if (trackedEnemies.ContainsKey(target))
            {
                trackedEnemies[target].UpdateVision(vision);
                return;
            }

            if (freeIndicators.TryPop(out AlertNotificationIndicatorUI indicator))
            {
                indicator.Activate(target);
                indicator.UpdateVision(vision);
                activeIndicators.Add(indicator);
                trackedEnemies.Add(target, indicator);
            }
        }

        public void TryRemove(Enemy enemy)
        {


            if (trackedEnemies.ContainsKey(enemy))
            {
                trackedEnemies[enemy].Deactivate(Return);
                trackedEnemies.Remove(enemy);
                //activeIndicators.Remove()
            }
        }

        private void Return(AlertNotificationIndicatorUI enemy)
        {
            freeIndicators.Push(enemy);
        }
    }
}
