using CarGame.Saving;
using Ellie.Json;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static CarGame.TerrainChunk;

namespace CarGame
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        private const string SAVE_STRING = "SaveString";
        public SaveData SaveData;

        private void Awake()
        {
            Instance = this;
        }

        public static bool HasSaveFile()
        {
#if UNITY_WEBGL
    return PlayerPrefs.HasKey(SAVE_STRING);
#else
            return File.Exists(GetSavePath(SAVE_STRING));
#endif
        }

        public void CreateNewGame()
        {
            SaveData = new SaveData();
        }

        public async Awaitable SaveAsync()
        {
            SaveData.PlayerData = GameManager.Instance.Player.GetSaveData();
            SaveData.WorldData.Time = TimeManager.Instance.GetTime();
            SaveData.WorldData.PlayerBuildings = BuildingManager.Instance.GetSaveData();
            SaveData.WorldData.Enemies = EnemySpawnManager.Instance.GetSaveData();
            SaveData.WorldData.Chunks = TerrainManager.Instance.GetSaveData();
            string json = JsonConvert.SerializeObject(SaveData, Settings);
#if UNITY_WEBGL

            PlayerPrefs.SetString(SAVE_STRING, json);
            PlayerPrefs.Save();
#else
            //string json = await Task.Run(() => JsonConvert.SerializeObject(SaveData, Settings));
            //await Awaitable.MainThreadAsync();

            string path = GetSavePath(SAVE_STRING);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            await File.WriteAllTextAsync(path, json);
#endif

            await Awaitable.NextFrameAsync();
            Debug.Log("Saved game data");
        }

        public void Load()
        {
            string json;

#if UNITY_WEBGL
            json = PlayerPrefs.GetString(SAVE_STRING);
#else
            string path = GetSavePath(SAVE_STRING);
            if (!File.Exists(path))
            {
                Debug.LogWarning("No Save found");
                return;
            }
            json = File.ReadAllText(path);
#endif

            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning("No Save found");
                return;
            }

            SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json, Settings);
            ItemDatabase database = GameManager.Instance.ItemDatabase;

            foreach (var item in saveData.PlayerData.inventory)
            {
                if (item == null) continue;
                item.ItemData = database.GetById(item.Id);
            }

            GameManager.Instance.Player.OnLoadData(saveData.PlayerData);
            TimeManager.Instance.SetTime(saveData.WorldData.Time);
            BuildingManager.Instance.LoadBuildings(saveData.WorldData.PlayerBuildings);
            EnemySpawnManager.Instance.LoadData(saveData.WorldData.Enemies);
            TerrainManager.Instance.LoadData(saveData.WorldData.Chunks);

            Debug.Log("Game Loaded");
        }

        private static string GetSavePath(string saveString)
        {
#if UNITY_WEBGL
            return null; // Use PlayerPrefs
#elif UNITY_EDITOR
            return Path.Combine(Application.persistentDataPath, "SaveData", $"{saveString}.json");
#else
    return Path.Combine(Application.dataPath, "SaveData", $"{saveString}.json");
#endif
        }


        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            SerializationBinder = new JsonResolver(),

            Converters = new List<JsonConverter>
            {
                new Vector3Converter(),
                new QuaternionConverter(),
                new ColorConverter(),
            },

            Formatting = Formatting.Indented
        };
    }


    [System.Serializable]
    public class SaveData
    {
        public PlayerSaveData PlayerData;

        // World Data (Chunks, etc
        public WorldSaveData WorldData;
    }

    [System.Serializable]
    public struct PlayerSaveData
    {
        public Vector3 position;
        public Vector3 scale;

        public Vector3 SpawnPosition;

        // Stats
        public int health;
        public float turbo;
        public float hunger;


        // Inventory
        public InventoryItem[] inventory;

        // Gadgets
        public GadgetSaveData Gadgets;

        // Workshop Upgrades
        public int workshopLevel;




    }

    [System.Serializable]
    public struct GadgetSaveData
    {
        public InventoryItem[] Inventory;
        public float[] Cooldowns;
    }

    [System.Serializable]
    public class WorldSaveData
    {
        // Time
        public float Time;

        // Chunks
        public List<ChunkSaveData> Chunks;

        // Buildings
        public List<BuildingSaveData> PlayerBuildings;

        // Enemies
        public List<EnemySaveData> Enemies;
    }

    [JsonObject]
    public class BuildingSaveData
    {
        public string Id;
        public Vector3 Position;
        public Quaternion Rotation;
        public BuildingSaveData[] Attachments;
        public InventoryItem[] inventory;
    }

    [System.Serializable]
    public struct EnemySaveData
    {
        public string Id;
        public int Health;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public float AngularVelocity;
    }

    [System.Serializable]
    public struct ChunkSaveData
    {
        public List<HarvestNodeSaveData> HarvestNodes;
        public List<InteractableSaveData> Interactables;
    }


}
