using UnityEngine;

namespace CarGame
{
    public class WorldManager : MonoBehaviour
    {
        [SerializeField] private TerrainManager terrainManager;

        public void TrySpawn()
        {
            foreach (var biome in terrainManager.Biomes)
            {
                Debug.Log("Biome " + biome);
                biome.TryRespawnNodes(terrainManager, biome.Data.nodes, false);
                biome.TryRespawnNodes(terrainManager, biome.Data.interactables, true);
            }
        }



    }
}
