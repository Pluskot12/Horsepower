using System.Collections.Generic;
using UnityEngine;

namespace CarGame
{
    public class BiomeDeadEnd : Biome
    {
        private float startingDamagePercent = 0.2f;
        private float tickRate = 1;

        private class BurnData
        {
            public float timer;
            public float damagePercent;
        }

        private Dictionary<IDamageable, BurnData> targets = new Dictionary<IDamageable, BurnData>();

        private void Update()
        {
            foreach (var target in new List<IDamageable>(targets.Keys))
            {
                if (!targets.ContainsKey(target))
                {
                    continue;
                }

                targets[target].timer += Time.deltaTime;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.isTrigger)
            {
                return;
            }

            if (collision.transform.root.TryGetComponent<IDamageable>(out IDamageable target))
            {
                targets.TryAdd(target, new BurnData { timer = tickRate, damagePercent = startingDamagePercent });
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.collider.transform.root.TryGetComponent<IDamageable>(out IDamageable target))
            {
                if (targets.TryGetValue(target, out BurnData data) && data.timer >= tickRate)
                {
                    target.TryDamagePercentage(data.damagePercent, gameObject);
                    data.damagePercent *= 2;
                    data.timer = 0;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.isTrigger)
            {
                return;
            }

            if (collision.transform.root.TryGetComponent<IDamageable>(out IDamageable target))
            {
                targets.Remove(target);
            }
        }
    }
}
