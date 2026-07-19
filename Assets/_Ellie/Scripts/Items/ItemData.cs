using TriInspector;
using UnityEngine;

namespace CarGame
{
    [CreateAssetMenu(menuName = "Car/Item/New Item")]
    public class ItemData : ScriptableObject
    {
        [ReadOnly] public string Id;
        public string displayName;
        public Sprite sprite;
        public int maxStackSize = 1;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = System.Guid.NewGuid().ToString();
            }
        }
    }
}