using UnityEngine;

namespace CarGame
{
    public class GatheringTool : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform hitPoint;
        [SerializeField] private LayerMask targetLayers;
        [SerializeField] private float hitRadius = 0.25f;

        [Header("Tool Settings")]
        [SerializeField] private HarvestNode.HarvestType type;
        [SerializeField] private int harvestDamage;
        [SerializeField] private int combatDamage;
        [SerializeField] private float swingCooldown = 0.75f;

        public void OnUse(Tool tool)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(hitPoint.position, hitRadius, targetLayers);

            int connectedHits = 0;

            foreach (var hit in hits)
            {
                HarvestNode harvestable = hit.GetComponent<HarvestNode>();

                if (harvestable != null)
                {
                    if (type == harvestable.Type)
                    {
                        harvestable.Damage(harvestDamage);
                        connectedHits++;
                        continue;
                    }
                }

                if (hit.attachedRigidbody)
                {
                    IDamageable damageable = hit.attachedRigidbody.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        Debug.Log("TODO: Set attacker source");
                        damageable.TryDamage(combatDamage, null);
                        connectedHits++;
                        continue;
                    }
                }

                if (connectedHits > 0)
                {
                    tool.UpdateDurability();
                }
            }

        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(hitPoint.position, hitRadius);
        }
    }
}
