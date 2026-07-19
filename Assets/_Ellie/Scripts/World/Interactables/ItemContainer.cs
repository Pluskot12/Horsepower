using Ellie.Audio;
using System.Collections.Generic;
using UnityEngine;
using static CarGame.DropTable;
using static CarGame.TerrainChunk;

namespace CarGame
{
    public class ItemContainer : MonoBehaviour, Interactable, IRespawnable
    {
        [SerializeField] private WorldObjectData data;

        [Header("References")]
        [SerializeField] private BoxCollider2D boxCollider;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Effect")]
        [SerializeField] private GameObject explosion;

        [Header("Audio")]
        [SerializeField] private AudioClip[] onDestroyAudio;

        [Header("Drops")]
        [SerializeField] private int minItems = 1;
        [SerializeField] private int maxItems = 3;
        [SerializeField] private DropTable dropTable;
        [SerializeField] private TerrainChunk chunk;

        int variant;

        float respawnTime => TimeManager.Instance.DayLength * 0.5f;

        public void Respawn()
        {
            SetActive(true);
        }

        public void Init(int variant, TerrainChunk chunk)
        {
            this.variant = variant;
            this.chunk = chunk;
        }

        public void TryInteract()
        {
            SpawnLoot();

            if (explosion)
            {
                var explosionInstance = Instantiate(explosion, explosion.transform.position, explosion.transform.rotation);
                explosionInstance.transform.SetParent(null);
                explosionInstance.SetActive(true);
                Destroy(explosionInstance.gameObject, 5f);
            }


            if (onDestroyAudio.Length > 0)
            {
                SoundManager.PlayRandomSFX(onDestroyAudio, transform.position);
            }
            else
            {
                Debug.LogWarning("No audioclips set for " + name);
            }


            if (chunk)
            {
                chunk.OnHarvest(this, respawnTime);
            }
            else
            {
                Debug.Log("No respawning for " + gameObject.name);
            }

            SetActive(false);
        }
        /*[ReadOnly]*/
        public string Id;
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = System.Guid.NewGuid().ToString();
            }
        }
        private void SetActive(bool active)
        {
            spriteRenderer.enabled = active;
            boxCollider.enabled = active;
        }


        private void SpawnLoot()
        {
            List<DroppedItem> drops = new List<DroppedItem>();

            drops.AddRange(Roll(minItems, maxItems));

            ItemSpawner.Instance.SpawnLoot(transform, drops);
        }

        private IEnumerable<DroppedItem> Roll(int minItems, int maxItems, float multiplier = 1f)
        {
            List<DropTable.Item> shuffledItems = new List<DropTable.Item>(dropTable.items);
            for (int i = shuffledItems.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                var temp = shuffledItems[i];
                shuffledItems[i] = shuffledItems[j];
                shuffledItems[j] = temp;
            }

            List<DroppedItem> drops = new List<DroppedItem>();
            HashSet<ItemData> droppedItemTypes = new HashSet<ItemData>();

            foreach (DropTable.Item item in shuffledItems)
            {
                if (drops.Count >= maxItems) break;

                float random = Random.Range(0f, 100f);
                if (random <= item.dropChance * multiplier)
                {
                    DroppedItem droppedItem = new DroppedItem();
                    droppedItem.item = item.item;
                    droppedItem.quantity = Random.Range(item.minDrop, item.maxDrop);
                    drops.Add(droppedItem);
                    droppedItemTypes.Add(item.item);
                }
            }

            while (drops.Count < minItems && droppedItemTypes.Count < shuffledItems.Count)
            {
                foreach (DropTable.Item item in shuffledItems)
                {
                    if (drops.Count >= minItems)
                    {
                        break;
                    }

                    if (droppedItemTypes.Contains(item.item))
                    {
                        continue;
                    }

                    float random = Random.Range(0f, 100f);
                    if (random <= item.dropChance * multiplier)
                    {
                        DroppedItem droppedItem = new DroppedItem();
                        droppedItem.item = item.item;
                        droppedItem.quantity = Random.Range(item.minDrop, item.maxDrop);
                        drops.Add(droppedItem);
                        droppedItemTypes.Add(item.item);
                    }
                }
            }

            return drops;
        }

        public InteractableSaveData GetSaveData()
        {
            return new InteractableSaveData()
            {
                Id = data.Id,
                Variant = variant,
                Position = transform.position,
                Rotation = transform.rotation,
                RespawnTime = 0 // TODO
            };
        }


        #region Helper
#if UNITY_EDITOR
        [ContextMenu("Update Collider")]
        public void UpdateCollider()
        {
            boxCollider.size = spriteRenderer.sprite.bounds.size;
            boxCollider.offset = spriteRenderer.sprite.bounds.center;

            UnityEditor.EditorUtility.SetDirty(boxCollider);
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
        #endregion

    }
}
