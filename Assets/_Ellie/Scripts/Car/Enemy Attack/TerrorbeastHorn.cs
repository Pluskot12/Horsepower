using Demo.Perception;
using Ellie.Audio;
using UnityEngine;

namespace CarGame
{
    public class TerrorbeastHorn : MonoBehaviour
    {
        [SerializeField] private Enemy enemy;
        [SerializeField] private AudioClip hornSound;
        [SerializeField] private float noiseLevel = 10f;
        [SerializeField, Range(0.0f, 1.0f)] private float triggerChance = 0.5f;

        public void OnIdleSound()
        {
            if (enemy.IsIdle)
            {
                return;
            }

            float roll = Random.value;

            if (roll >= triggerChance)
            {
                Beep();
            }
        }

        private void Beep()
        {
            SoundManager.PlaySFX(hornSound, transform.position);
            EnemySpawnManager.Instance.OnNoiseGenerated(noiseLevel);
        }
    }
}
