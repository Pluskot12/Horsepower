using TriInspector;
using UnityEngine;

namespace CarGame
{
    [CreateAssetMenu(menuName = "Car/Enemy/New Enemy")]
    public class EnemyData : ScriptableObject
    {
        [ReadOnly] public string Id;
        public EnemyController Prefab;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = System.Guid.NewGuid().ToString();
            }
        }
    }
}
