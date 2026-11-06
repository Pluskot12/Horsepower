using Ellie.Core;
using UnityEngine;

namespace CarGame
{
    public class NoiseGenerator : MonoBehaviour
    {
        [SerializeField] private float radius;
        [SerializeField] private LayerMask targetLayers;
        [SerializeField] private bool fireEvents;

        public void GenerateNoise(float multiplier, Transform owner)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, targetLayers);

            float dist;
            float depth;
            float alertLevel;

            NoiseManager.EmitNoise(transform.position, 1f, owner);

            // Old
            // EnemySpawnManager.Instance.OnNoiseGenerated(multiplier);

            foreach (var hit in hits)
            {
                if (!hit.attachedRigidbody)
                {
                    continue;
                }

                if (hit.attachedRigidbody.TryGetComponent<EnemyController>(out EnemyController enemy))
                {
                    dist = Vector2.Distance(hit.transform.position, transform.position);
                    depth = 1f - (dist / radius);
                    alertLevel = Mathf.Clamp01(depth);

                    enemy.Alert(transform.position, alertLevel);
                }
            }


        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
