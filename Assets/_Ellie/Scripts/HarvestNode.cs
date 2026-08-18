
using Ellie.Audio;
using UnityEngine;
using static CarGame.TerrainChunk;

namespace CarGame
{
    public interface IRespawnable
    {
        public void Respawn();
        public void Init(int variant, TerrainChunk chunk, Vector3 position);
        public Vector3 Position { get; set; }
    }



    public class HarvestNode : MonoBehaviour, IRespawnable
    {
        [SerializeField] private WorldObjectData data;

        [Header("References")]
        [SerializeField] private GameObject visuals;
        [SerializeField] private Collider2D hitbox;

        [Header("Node Settings")]
        [SerializeField] private HarvestType type;
        [SerializeField] private ItemData resource;
        [SerializeField] private int minDrop;
        [SerializeField] private int maxDrop;
        [SerializeField] private int health;

        [Header("Hit Effect")]
        [SerializeField] private HitEffect hitEffect;
        [SerializeField] private Transform hitEffectSpawnPoint;
        [SerializeField] private Transform onDeathPartsParent;

        [Header("Sounds")]
        [SerializeField] private AudioClip[] hitSounds;
        [SerializeField] private AudioClip deathSound;

        [Header("Chunk")]
        [SerializeField] private TerrainChunk chunk;
        private int maxHealth;

        public Vector3 Position { get; set; }

        public enum HarvestType
        {
            None,
            Mining,
            Woodcutting
        }

        int variant;

        public HarvestType Type => type;


        public void Respawn()
        {
            health = maxHealth;
            SetActive(true);
        }
        public bool debug;


        private void Start()
        {
            maxHealth = health;
        }

        public void Init(int variant, TerrainChunk chunk, Vector3 position)
        {
            this.variant = variant;
            this.chunk = chunk;

            Position = position;
        }

        public void SetData(WorldObjectData data)
        {
            this.data = data;
        }

        public void Damage(int damage)
        {
            health = Mathf.Clamp(health - damage, 0, maxHealth);

            OnHit(damage);

            if (health <= 0)
            {
                OnDeath();
            }

        }

        public void OnHit(int damage)
        {
            SoundManager.PlayRandomSFX(hitSounds, transform.position);

            int random = Random.Range(1, 3);
            for (int i = 0; i < random; i++)
            {
                HitEffect effect = Instantiate(hitEffect, hitEffectSpawnPoint.position, Quaternion.identity);
            }
        }

        public void OnDeath()
        {
            SpawnResources();

            var partsInstance = Instantiate(onDeathPartsParent, onDeathPartsParent.transform.position, onDeathPartsParent.rotation);
            partsInstance.SetParent(null);
            partsInstance.gameObject.SetActive(true);

            SoundManager.PlaySFX(deathSound, transform.position);
            //audioSource.transform.parent = null;
            //Destroy(audioSource.gameObject, deathSound.length * 2f);
            //Destroy(gameObject);

            //float respawnTime = TimeManager.Instance.DayLength * 2f;

            if (chunk)
            {
                chunk.OnHarvest(this, TimeManager.Instance.ResourceRespawnTime);
            }
            else
            {
                Debug.Log("No respawning for " + gameObject.name);
            }

            SetActive(false);
        }

        private void SetActive(bool active)
        {
            visuals.SetActive(active);
            hitbox.enabled = active;
        }

        private void SpawnResources()
        {
            if (resource == null)
            {
                Debug.LogWarning("No resource set");
                return;
            }

            int quantity = Random.Range(minDrop, maxDrop);
            Vector3 position = transform.position + Vector3.up;
            Vector3 force = Vector3.up * 150f;

            ItemSpawner.Instance.SpawnItem(resource, quantity, -1, position, force);
        }

        public HarvestNodeSaveData GetSaveData()
        {
            return new HarvestNodeSaveData()
            {
                Id = data.Id,
                Variant = variant,
                Position = transform.position,
                Rotation = transform.rotation,
                Health = health,
                RespawnTime = 0 // TODO
            };
        }

        public void Setup(HarvestNodeSaveData loadedData)
        {
            health = loadedData.Health;

            if (health <= 0)
            {
                SetActive(false);
                chunk.OnHarvest(this, TimeManager.Instance.ResourceRespawnTime);
            }
        }
    }
}
