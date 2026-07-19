using TriInspector;
using UnityEngine;

namespace CarGame
{
    [CreateAssetMenu(menuName = "Car/World Object")]
    public class WorldObjectData : ScriptableObject
    {
        [ReadOnly] public string Id;
        public GameObject[] variants;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = System.Guid.NewGuid().ToString();
            }
        }
    }
}
