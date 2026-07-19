using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D;

namespace CarGame
{
    [System.Serializable]
    public class FlatArea
    {
        public int startPoint;
        public int endPoint;

        public float startX;
        public float endX;
        public float y;

        public float Width => endX - startX;

        public BiomeData biome;

        public bool containsStructure;

        public FlatArea(int start, int end, float startX, float endX, float y, BiomeData biome)
        {
            this.startPoint = start;
            this.endPoint = end;
            this.startX = startX;
            this.endX = endX;
            this.y = y;
            this.biome = biome;
        }
    }

    [System.Serializable]
    public class SpawnableArea
    {
        public int startPoint;
        public int endPoint;

        public float startX;
        public float endX;
        public float y;

        public float Width => endX - startX;

        public bool isOccupied;

        public bool containsStructure;

        public SpawnableArea(int start, int end, float startX, float endX, float y)
        {
            this.startPoint = start;
            this.endPoint = end;
            this.startX = startX;
            this.endX = endX;
            this.y = y;

        }
    }



    public class TerrainChunk : MonoBehaviour
    {
        public SpriteShapeController shape;

        [Header("Chunk")]
        public Biome biome;
        public float chunkWidth = 50f;
        public int pointsPerChunk = 16;
        public float bottomHeight = -20f;

        [Header("Generation")]
        public float startHeight;
        public float globalXOffset;
        public BiomeData biomeData;

        public bool CanBuild = true;

        public float EndHeight { get; private set; }
        public Vector3 EndTangent { get; private set; }

        public List<FlatArea> flatAreas = new List<FlatArea>();

        List<Structure> structures = new List<Structure>();

        [SerializeField] private LayerMask structureMask;
        [SerializeField] private bool debug;


        #region Chunk Culling

        public Vector2 Position => _cachedPos;

        private SpriteShapeRenderer _renderer;
        private Vector2 _cachedPos;

        private Renderer[] _renderers;
        private ShadowCaster2D[] _shadowCasters;
        void Awake()
        {
            // Grabs SpriteShapeRenderer, SpriteRenderer, MeshRenderer — everything
            _renderers = GetComponentsInChildren<Renderer>();
            _shadowCasters = GetComponentsInChildren<ShadowCaster2D>();
            _cachedPos = transform.position;

        }
        /*
        void OnEnable() => ChunkCullingManager.Instance?.Register(this);
        void OnDisable() => ChunkCullingManager.Instance?.Unregister(this);
        */
        public void SetVisible(bool visible)
        {

            /*
            foreach (var r in _renderers)
                if (r.enabled != visible) r.enabled = visible;
            */
            foreach (var s in _shadowCasters)
                if (s.enabled != visible) s.enabled = visible;
        }

        #endregion


        public void Generate(Biome biome)
        {
            //biome.SetData(biomeData);

            this.biome = biome;

            shape.spline.Clear();
            flatAreas.Clear();

            float step = chunkWidth / (pointsPerChunk - 1);
            int index = 0;

            shape.spline.InsertPointAt(index++, new Vector3(0, bottomHeight, 0));

            float currentFlatHeight = 0f;
            int flatPointsRemaining = 0;
            int flatStartIndex = -1;

            for (int i = 0; i < pointsPerChunk; i++)
            {
                float x = i * step;
                float y;

                if (i == 0)
                {
                    y = startHeight;
                }
                else if (flatPointsRemaining > 0)
                {
                    y = currentFlatHeight;
                    flatPointsRemaining--;

                    // Start flat
                    if (flatStartIndex == -1)
                        flatStartIndex = i;
                }
                else
                {
                    // End flat section
                    if (flatStartIndex != -1)
                    {
                        RegisterFlatArea(flatStartIndex, i - 1, step, currentFlatHeight);
                        flatStartIndex = -1;
                    }

                    if (Random.value < biomeData.flatChance)
                    {
                        flatPointsRemaining = Random.Range(
                            biomeData.flatMinPoints,
                            biomeData.flatMaxPoints
                        );

                        float noise = Mathf.PerlinNoise(
                            (globalXOffset + x) * biomeData.noiseScale,
                            biomeData.noiseSeed
                        );

                        currentFlatHeight =
                            startHeight + (noise - 0.5f) * biomeData.heightAmplitude;

                        y = currentFlatHeight;
                        flatPointsRemaining--;

                        flatStartIndex = i;
                    }
                    else
                    {
                        float noise = Mathf.PerlinNoise(
                            (globalXOffset + x) * biomeData.noiseScale,
                            biomeData.noiseSeed
                        );

                        y = startHeight + (noise - 0.5f) * biomeData.heightAmplitude;
                    }
                }

                shape.spline.InsertPointAt(index++, new Vector3(x, y, 0));
            }

            // Flat reaches chunk end
            if (flatStartIndex != -1)
            {
                RegisterFlatArea(flatStartIndex, pointsPerChunk - 1, step, currentFlatHeight);
            }

            EndHeight = shape.spline.GetPosition(index - 1).y;

            shape.spline.InsertPointAt(index++, new Vector3(chunkWidth, bottomHeight, 0));

            for (int i = 0; i < shape.spline.GetPointCount(); i++)
            {
                biomeData.ApplyTangent(shape.spline, i, flatAreas);
            }

            shape.spriteShape = biomeData.profile;

            shape.RefreshSpriteShape();
            shape.BakeMesh();
            shape.BakeCollider();
        }

        void RegisterFlatArea(int startIndex, int endIndex, float step, float y)
        {
            float startX = startIndex * step;
            float endX = endIndex * step;

            // Ignore if too small
            if (endX - startX < step * 1.5f)
                return;

            flatAreas.Add(new FlatArea(
                startIndex + 1,
                endIndex + 1,
                startX,
                endX,
                y,
                biomeData
            ));
        }

        public bool TryPlaceBiomeStructure(Structure structure, FlatArea area)
        {
            return Place(area, structure);
        }

        private bool Place(FlatArea flat, Structure building)
        {
            if (flat.containsStructure == true)
            {
                Debug.Log("This chunk already contains a structure");
                return false;
            }

            float halfWidth = building.SpriteRenderer.bounds.size.x / 2f;

            float minX = flat.startX + halfWidth;
            float maxX = flat.endX - halfWidth;

            if (minX >= maxX)
            {
                return false;
            }

            flat.containsStructure = true;

            float x = Random.Range(minX, maxX);
            float y = flat.y;

            Vector3 worldPos = transform.TransformPoint(
                new Vector3(x, y, 0)
            );

            Structure s = Instantiate(building, worldPos, Quaternion.identity, transform);
            s.SetBiome(biome, this);
            structures.Add(s);

            return true;
        }

        public List<GameObject> spawnedObjects = new List<GameObject>();

        public void SpawnBiomeObjects(TerrainManager manager, List<BiomeSpawnable> objects, Transform parent, bool alignWithGround, float multiplier = 1f)
        {
            if (objects == null || objects.Count == 0)
            {
                return;
            }

            float stepSize = 0.5f;
            float x = 0f;

            while (x < chunkWidth)
            {
                List<BiomeSpawnable> candidates = null;

                foreach (var spawnable in objects)
                {
                    if (Random.value <= (spawnable.spawnChance * multiplier) * stepSize)
                    {
                        if (candidates == null)
                        {
                            candidates = new List<BiomeSpawnable>();
                        }

                        candidates.Add(spawnable);
                    }
                }

                if (candidates == null || candidates.Count == 0)
                {
                    x += stepSize;
                    continue;
                }

                BiomeSpawnable chosen = candidates[Random.Range(0, candidates.Count)];

                float halfWidth = chosen.minSpacing * 0.5f;
                float slopeAngle = GetFootprintSlopeAngle(x, halfWidth);

                if (slopeAngle > chosen.allowedAngle)
                {
                    x += 0.5f;
                    continue;
                }

                RaycastHit2D hit = manager.RaycastGroundAt(new Vector3(transform.position.x + x, 0, 0));
                if (!hit)
                {
                    continue;
                }

                float y = hit.point.y;
                float worldX = hit.point.x;

                if (IsBlockedByStructure(worldX, halfWidth))
                {
                    x += 0.5f;
                    continue;
                }

                Vector3 position = new Vector3(worldX, y, 0f);

                int variant = Random.Range(0, chosen.objectData.variants.Length);
                var prefab = chosen.objectData.variants[variant];
                var spawnedObject = Instantiate(prefab, position, Quaternion.identity, parent);
                if (spawnedObject.TryGetComponent(out IRespawnable s))
                {
                    s.Init(variant, this);
                }
                else
                {
                    Debug.LogWarning("Object is not respawnable");
                }
                if (alignWithGround)
                {
                    spawnedObject.transform.up = hit.normal;
                }

                spawnedObjects.Add(spawnedObject);

                float spacing = Random.Range(chosen.minSpacing, chosen.maxSpacing);

                x += spacing;
            }
        }

        float GetFootprintSlopeAngle(float localX, float halfWidth)
        {
            float leftX = Mathf.Max(0f, localX - halfWidth);
            float rightX = Mathf.Min(chunkWidth, localX + halfWidth);

            float leftY = GetHeightAtX(leftX);
            float rightY = GetHeightAtX(rightX);

            Vector2 dir = new Vector2(rightX - leftX, rightY - leftY).normalized;

            return Vector2.Angle(dir, Vector2.right);
        }

        public float GetHeightAtX(float localX)
        {
            var spline = shape.spline;
            int count = spline.GetPointCount();

            for (int i = 1; i < count - 1; i++)
            {
                float x0 = spline.GetPosition(i).x;
                float x1 = spline.GetPosition(i + 1).x;

                if (localX >= x0 && localX <= x1)
                {
                    float t = Mathf.InverseLerp(x0, x1, localX);
                    return Mathf.Lerp(
                        spline.GetPosition(i).y,
                        spline.GetPosition(i + 1).y,
                        t
                    );
                }
            }

            return spline.GetPosition(1).y;
        }

        bool IsBlockedByStructure(float worldX, float radius)
        {
            foreach (var sr in structures)
            {
                if (sr == null) continue;

                Bounds b = sr.SpriteRenderer.bounds;

                if (worldX + radius > b.min.x &&
                    worldX - radius < b.max.x)
                {
                    return true;
                }
            }

            return false;
        }
        private List<FlatArea> spawnableFlats;
        [ContextMenu("Split Flats")]
        private void SplitFlatArea()
        {
            SplitFlatAreas(flatAreas, 1.5f);
            /*
            spawnableFlats = new List<FlatArea>();

            foreach (var flat in flatAreas)
            {
                Vector3 startPosition = transform.position + new Vector3(flat.startX + flat.Width / 2f, flat.y);
                Vector3 size = new Vector3(flat.Width, 2, 1f);


                Gizmos.DrawCube(startPosition, size);
            }*/
        }
        [SerializeField] List<SpawnableArea> slicedAreas = new List<SpawnableArea>();

        private SpawnableArea GetRandomUnoccupiedFlatArea(List<SpawnableArea> flatAreas)
        {
            List<SpawnableArea> unoccupied = flatAreas.Where(a => !a.isOccupied && !a.containsStructure).ToList();

            if (unoccupied.Count == 0)
                return null;

            return unoccupied[Random.Range(0, unoccupied.Count)];
        }


        private List<SpawnableArea> SplitFlatAreas(List<FlatArea> flatAreas, float sliceSize = 2f, float slicePadding = 0.1f)
        {
            slicedAreas = new List<SpawnableArea>();

            foreach (FlatArea area in flatAreas)
            {
                int sliceCount = Mathf.FloorToInt(area.Width / sliceSize);

                if (sliceCount == 0)
                    continue;

                float evenSliceSize = area.Width / sliceCount;

                for (int i = 0; i < sliceCount; i++)
                {
                    float sliceStartX = area.startX + i * evenSliceSize + slicePadding;
                    float sliceEndX = area.startX + (i + 1) * evenSliceSize - slicePadding;

                    // Skip if padding collapses the slice
                    if (sliceStartX >= sliceEndX)
                        continue;

                    float t1 = (sliceStartX - area.startX) / area.Width;
                    float t2 = (sliceEndX - area.startX) / area.Width;
                    int sliceStartPoint = area.startPoint + Mathf.RoundToInt(t1 * (area.endPoint - area.startPoint));
                    int sliceEndPoint = area.startPoint + Mathf.RoundToInt(t2 * (area.endPoint - area.startPoint));

                    SpawnableArea slice = new SpawnableArea(
                        sliceStartPoint,
                        sliceEndPoint,
                        sliceStartX,
                        sliceEndX,
                        area.y
                    );

                    slice.containsStructure = area.containsStructure;
                    slicedAreas.Add(slice);

                }
            }

            return slicedAreas;
        }



        [ContextMenu("spawn random")]
        private void SpawnRandom()
        {
            var flat = GetRandomUnoccupiedFlatArea(slicedAreas);
            flat.isOccupied = true;
        }

        private void OnDrawGizmos()
        {
            if (!debug) return;

            foreach (var flat in slicedAreas)
            {
                if (flat.containsStructure) Gizmos.color = Color.red;
                else if (flat.isOccupied) Gizmos.color = Color.yellow;
                else Gizmos.color = Color.green;

                Vector3 startPosition = transform.position + new Vector3(flat.startX + flat.Width / 2f, flat.y);
                Vector3 size = new Vector3(flat.Width - 0.1f, 2, 1f);
                Gizmos.DrawCube(startPosition, size);
            }
        }

        public void OnHarvest(IRespawnable harvestNode, float respawnTime)
        {
            StartCoroutine(RespawnObject(harvestNode, respawnTime));
        }

        private IEnumerator RespawnObject(IRespawnable o, float respawnTime)
        {
            yield return new WaitForSeconds(respawnTime);

            o.Respawn();
        }

        public ChunkSaveData GetSaveData()
        {
            var saveData = new ChunkSaveData();
            saveData.HarvestNodes = new List<HarvestNodeSaveData>();
            saveData.Interactables = new List<InteractableSaveData>();

            foreach (var o in spawnedObjects)
            {
                if (o.TryGetComponent<HarvestNode>(out HarvestNode harvestNode))
                {
                    saveData.HarvestNodes.Add(harvestNode.GetSaveData());
                }
                else if (o.TryGetComponent<Interactable>(out Interactable interactable))
                {
                    saveData.Interactables.Add(interactable.GetSaveData());
                }
            }

            return saveData;
        }

        public void SpawnLoadedObjects(ChunkSaveData chunkSaveData)
        {
            GameObject variant;
            HarvestNode prefab;

            foreach (var node in chunkSaveData.HarvestNodes)
            {
                variant = GameManager.Instance.WorldObjectDatabase.GetById(node.Id).variants[node.Variant];
                prefab = variant.GetComponent<HarvestNode>();

                var instance = Instantiate(prefab, node.Position, node.Rotation, transform);
                instance.Init(node.Variant, this);
                instance.Setup(node);
                spawnedObjects.Add(instance.gameObject);
            }
            ItemContainer interactablePrefab;
            foreach (var interactable in chunkSaveData.Interactables)
            {
                variant = GameManager.Instance.WorldObjectDatabase.GetById(interactable.Id).variants[interactable.Variant];
                interactablePrefab = variant.GetComponent<ItemContainer>();
                var instance = Instantiate(interactablePrefab, interactable.Position, interactable.Rotation, transform);
                instance.Init(interactable.Variant, this);
                //instance.Setup(interactable);
                spawnedObjects.Add(instance.gameObject);
            }
        }

        [System.Serializable]
        public struct HarvestNodeSaveData
        {
            public string Id;
            public int Variant;
            public Vector3 Position;
            public Quaternion Rotation;
            public int Health;
            public float RespawnTime;
        }

        [System.Serializable]
        public struct InteractableSaveData
        {
            public string Id;
            public int Variant;
            public Vector3 Position;
            public Quaternion Rotation;
            public float RespawnTime;
        }
    }
}

