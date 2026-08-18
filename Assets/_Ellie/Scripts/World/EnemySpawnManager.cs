using Ellie.Audio;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace CarGame
{
    public class EnemySpawnManager : MonoBehaviour
    {
        public static EnemySpawnManager Instance { get; private set; }

        [Header("Database")]
        [SerializeField] private EnemyDatabase database;

        [Header("References")]
        [SerializeField] private Camera cam;
        [SerializeField] private EnemyData enemyData;
        [SerializeField] private LayerMask groundLayer;

        [Header("Settings")]
        [SerializeField] private int maxRandomEnemies = 5;
        [SerializeField] private int randomEnemyMaxDistnace = 50;
        [SerializeField] private float baseSpawnInterval = 2;
        [SerializeField, Range(0, 100f)] private float baseSpawnChance = 100;

        public List<EnemyController> spawnedEnemies = new List<EnemyController>();

        //private Vector3 left = new Vector2(-1, 0);
        //private Vector3 right = new Vector2(2, 0);

        [SerializeField] float buildingDistance = 5f;
        [SerializeField] float distancePerIteration = 1f;
        private Vector3 left = new Vector2(-0.8f, 0);
        private Vector3 right = new Vector2(1.8f, 0);


        [SerializeField] private bool canSpawn;

        private float time;

        #region Player Noise Aggro

        [Header("Player Aggro")]
        [SerializeField] private ProgressBarUI aggroBar;
        [SerializeField] private TextMeshProUGUI spawnChanceText;
        [SerializeField] private AnimationCurve raidSpawnCurve;
        [SerializeField] private AudioClip[] aggroTriggerSounds;
        [SerializeField] private float baseAggro = 100.6f;
        [SerializeField] private float deAggroMulti = 2f;
        [SerializeField] private bool aggroDebug;
        private float currentAggro;
        private Coroutine deAggro;

        private void UpdateAggroBar(float p)
        {
            aggroBar.Set(p);

            float spawnChance = raidSpawnCurve.Evaluate(Mathf.Clamp01(p)) * 100f;
            spawnChanceText.text = "" + spawnChance.ToString("n2") + "%";
        }

        public void OnNoiseGenerated(float multi)
        {
            currentAggro += baseAggro * multi;

            float progress = currentAggro / 100f;

            UpdateAggroBar(progress);

            if (deAggro != null)
            {
                StopCoroutine(deAggro);
            }

            deAggro = StartCoroutine(AggroDecrease());



            UpdateAggroBar(progress);


        }

        public bool TrySpawnRaid()
        {
            if (RollRaid())
            {
                SpawnRaid();

                return true;
            }

            return false;
        }

        private bool RollRaid()
        {
            float progress = currentAggro / 100f;
            float spawnChance = raidSpawnCurve.Evaluate(Mathf.Clamp01(progress)) * 100f;
            float randomRoll = Random.value * 100f;

            return randomRoll <= spawnChance;
        }

        private void SpawnRaid()
        {
            if (deAggro != null)
            {
                StopCoroutine(deAggro);
            }

            currentAggro = 0;
            UpdateAggroBar(0);

            int minEnemy = 1;
            int maxEnemy = 4;
            int random = Random.Range(minEnemy, maxEnemy);

            //Vector3 position = GameManager.Instance.Player.transform.position + Vector3.right * 5f;
            Vector3 position = GetOffScreenPosition();

            Side side = GetRandomSide();

            for (int i = 0; i < random; i++)
            {
                position.x += 1f + i * Random.value;

                //var prefab = TryGetEnemy(position, true);
                //var instance = SpawnEnemy(prefab, position);
                var instance = SpawnRandomEnemyOffScreen(side, true); //SpawnEnemy(prefab, position);

                instance.SetAggro(GameManager.Instance.Player);
            }

            SoundManager.PlayRandomSFX(aggroTriggerSounds, GameManager.Instance.Player.transform.position);
        }

        private IEnumerator AggroDecrease()
        {
            float delay = 1f;
            float progress = currentAggro / 100f;
            yield return new WaitForSeconds(delay);

            while (currentAggro > 0)
            {
                currentAggro -= Time.deltaTime * deAggroMulti;
                progress = currentAggro / 100f;
                UpdateAggroBar(progress);
                yield return null;
            }
        }

        #endregion

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
#if UNITY_EDITOR
            canSpawn = false;
#endif
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                // canSpawn = !canSpawn;
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                /*
                foreach (var enemy in randomSpawnedEnemies) 
                {
                    enemy.OnDeath();
                }

                randomSpawnedEnemies.Clear();
                */
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                return;
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                worldPosition.z = 0;
                var e = SpawnEnemy(enemyData.Prefab, worldPosition, Quaternion.identity, true);
                // spawnedEnemies.Add(e);
                //SpawnRandomEnemyOffScreen();

            }

            if (!canSpawn || GameManager.Instance.Player.IsDead)
            {
                return;
            }

            if (time >= GetSpawnInterval())
            {
                if (TrySpawnRaid() == false)
                {
                    TrySpawnRandomEnemy();
                }

                time = 0;
            }

            time += Time.deltaTime;
        }

        private void TrySpawnRandomEnemy(Side side = Side.Random)
        {
            spawnedEnemies.RemoveAll(enemy => enemy == null);

            if (spawnedEnemies.Count >= maxRandomEnemies)
            {
                foreach (var enemy in spawnedEnemies)
                {
                    if (Vector2.Distance(GameManager.Instance.Player.transform.position, enemy.transform.position) > randomEnemyMaxDistnace)
                    {
                        Destroy(enemy.gameObject);
                    }
                }

                spawnedEnemies.RemoveAll(enemy => enemy == null);

                if (spawnedEnemies.Count >= maxRandomEnemies)
                {
                    return;
                }
            }


            float chance = Random.Range(0f, 100f);
            if (chance <= baseSpawnChance)
            {
                SpawnRandomEnemyOffScreen(side);
            }

        }

        private bool BlockedByBuilding(Vector3 position)
        {
            foreach (var building in BuildingManager.Instance.PlacedBuildings)
            {
                if (Vector3.Distance(building.transform.position, position) <= buildingDistance)
                {
                    return true;
                }
            }

            return false;
        }

        private Vector3 GetOffScreenPosition()
        {
            Vector3 position = GameManager.Instance.Player.transform.position;
            position.x = cam.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x;

            return position;
        }

        private enum Side
        {
            Random,
            Left,
            Right
        }

        private Side GetRandomSide()
        {
            return Random.value >= 0.5f ? Side.Left : Side.Right;
        }

        private EnemyController SpawnRandomEnemyOffScreen(Side side, bool ignoreChance = false)
        {
            Vector3 position = GameManager.Instance.Player.transform.position;

            Vector3 offset;

            if (side == Side.Random)
            {
                side = GetRandomSide();
            }

            if (side == Side.Left)
            {
                offset = left;
            }
            else
            {
                offset = right;
            }


            position.x = cam.ViewportToWorldPoint(offset).x;
            int i = 0;

            while (BlockedByBuilding(position) && i < 3)
            {
                if (side == Side.Right)
                {
                    position.x += (distancePerIteration * (i + 1));
                }
                else
                {
                    position.x -= (distancePerIteration * (i + 1));
                }

                i++;
            }

            if (BlockedByBuilding(position))
            {
                return null;
            }

            RaycastHit2D hit = Physics2D.Raycast(position + Vector3.up * 100f, Vector2.down, 99999, groundLayer);

            // Dont spawn on DeadEnd
            if (hit.transform.gameObject.TryGetComponent<Biome>(out Biome biome))
            {
                if (biome.Type == BiomeType.DeadEnd)
                {
                    return null;
                }
            }

            var enemy = TryGetEnemy(position, ignoreChance);

            if (enemy != null)
            {
                var instance = SpawnEnemy(enemy, position, Quaternion.identity, true);

                return instance;
            }

            return null;
        }

        private EnemyController SpawnEnemy(EnemyController enemy, Vector2 position, Quaternion rotation, bool alignWithGround)
        {
            if (enemy == null)
            {
                return null;
            }

            EnemyController e = Instantiate(enemy, position, rotation);

            if (alignWithGround)
            {
                e.AlignToGround();
            }

            spawnedEnemies.Add(e);

            return e;
        }

        private EnemyController TryGetEnemy(Vector3 position, bool ignoreSpawnChance = false)
        {
            BiomeData biome = BiomeManager.Instance.CurrentBiome;

            position.y = 100;
            RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, 999, groundLayer);

            if (hit && hit.collider.TryGetComponent<TerrainChunk>(out TerrainChunk b))
            {
                biome = b.biomeData;
            }

            if (ignoreSpawnChance)
            {
                return biome.GetEnemy();
            }

            return biome.TryGetEnemy();
        }



        private float GetSpawnChance()
        {
            return baseSpawnChance;
        }

        private float GetSpawnInterval()
        {
            return baseSpawnInterval;
        }

        public List<EnemySaveData> GetSaveData()
        {
            return spawnedEnemies.Where(enemy => enemy != null).Select(enemy => enemy.GetSaveData()).ToList();
        }

        public void LoadData(List<EnemySaveData> data)
        {
            foreach (EnemySaveData enemy in data)
            {
                if (enemy.Health <= 0)
                {
                    continue;
                }

                var prefab = GetEnemyWithId(enemy.Id);
                var instance = SpawnEnemy(prefab, enemy.Position, enemy.Rotation, false);
                instance.LoadData(enemy);
            }
        }

        public EnemyController GetEnemyWithId(string id)
        {
            return database.GetById(id).Prefab;
        }

        public List<EnemyController> GetEnemiesWithinRadius(Transform center, float radius)
        {
            List<EnemyController> enemies = new List<EnemyController>();

            foreach (var enemy in spawnedEnemies)
            {
                if (Vector2.Distance(enemy.transform.position, center.position) <= radius)
                {
                    enemies.Add(enemy);
                }
            }

            return enemies;
        }
    }
}
