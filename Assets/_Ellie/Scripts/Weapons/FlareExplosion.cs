using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace CarGame
{
    public class FlareExplosion : MonoBehaviour
    {
        [Header("Light")]
        [SerializeField] private Light2D flareLight;
        [SerializeField] private float fadeInDuration = 0.5f;
        [SerializeField] private float fadeOutDuration = 0.5f;

        [Header("Settings")]
        [SerializeField] private float minDistance = 10f;
        [SerializeField] private float maxDistance = 50f;

        private Transform playerTransform;

        private FlareGun.LootDrop lootDrop;

        bool destroy;

        bool init;
        float noise;
        float lifetime;
        float tickrate;
        float tick;

        float elapsedTime;

        private bool canSpawnLoot;
        private float lootTimer;
        private float lootTimerInterval = 10;


        private void Awake()
        {
            flareLight.intensity = 0;
        }

        public void Setup(float noise, float tickrate, float lifetime, FlareGun.LootDrop lootDrop)
        {
            playerTransform = GameManager.Instance.Player.transform;

            this.noise = noise;
            this.tickrate = tickrate;
            this.lifetime = lifetime;

            EnemySpawnManager.Instance.OnNoiseGenerated(noise);

            this.lootDrop = lootDrop;

            canSpawnLoot = true;

            init = true;
        }

        private void Update()
        {
            if (!init)
            {
                return;
            }

            tick += Time.deltaTime;
            elapsedTime += Time.deltaTime;

            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (tick >= tickrate)
            {
                float distanceMultiplier = GetDistanceMultiplier(distanceToPlayer);

                if (distanceMultiplier > 0f)
                {
                    EnemySpawnManager.Instance.OnNoiseGenerated(noise * distanceMultiplier);
                }

                tick = 0;
            }

            lifetime -= Time.deltaTime;

            if (lifetime <= 0)
            {
                destroy = true;
            }

            UpdateLightIntensity();

            TrySpawnLoot(distanceToPlayer);

        }

        private float GetDistanceMultiplier(float distance)
        {
            if (playerTransform == null)
            {
                return 1f;
            }

            if (distance <= minDistance)
            {
                return 1f;
            }

            if (distance >= maxDistance)
            {
                return 0f;
            }

            return 1f - ((distance - minDistance) / (maxDistance - minDistance));
        }

        public void OnNoise()
        {

        }

        public void OnAnimationFinished()
        {
            if (destroy)
            {
                Destroy(gameObject);
            }
        }

        private void UpdateLightIntensity()
        {
            if (flareLight == null)
            {
                return;
            }

            float totalLifetime = lifetime + Time.deltaTime;
            float elapsed = totalLifetime - lifetime;

            if (elapsedTime <= fadeInDuration)
            {
                flareLight.intensity = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            }
            else if (lifetime <= fadeOutDuration)
            {
                flareLight.intensity = Mathf.Lerp(0f, 1f, lifetime / fadeOutDuration);
            }
            else
            {
                flareLight.intensity = 1f;
            }
        }

        private void TrySpawnLoot(float distanceToPlayer)
        {
            if (canSpawnLoot)
            {
                lootTimer += Time.deltaTime;

                if (lootTimer >= lootTimerInterval)
                {
                    if (distanceToPlayer <= maxDistance)
                    {
                        float random = Random.value;
                        float chance = GetLootDropChance();

                        if (random <= chance)
                        {
                            canSpawnLoot = false;
                            SpawnLootDrop();
                        }
                    }

                    lootTimer = 0;
                }
            }
        }

        private void SpawnLootDrop()
        {
            Vector3 position = transform.position;
            position.x += Random.Range(-5f, 5f);
            position.y += 2f;

            Instantiate(lootDrop.prefab, position, Quaternion.identity);
        }

        private float GetLootDropChance()
        {
            switch (TimeManager.Instance.GetTimeOfDay())
            {
                case TimeManager.TimeOfDay.Day: return lootDrop.dropChanceDay;
                case TimeManager.TimeOfDay.Dusk: return lootDrop.dropChanceDusk;
                case TimeManager.TimeOfDay.Night: return lootDrop.dropChanceNight;
                default: return 0f;
            }
        }
    }
}
