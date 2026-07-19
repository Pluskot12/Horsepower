using UnityEngine;

namespace CarGame
{
    public interface Interactable
    {
        TerrainChunk.InteractableSaveData GetSaveData();
        public void TryInteract();
    }
}
