using Demo.Perception;
using TriInspector;
using UnityEngine;

namespace CarGame
{
    [CreateAssetMenu(menuName = "Car/Enemy/New Enemy")]
    public class EnemyData : ScriptableObject
    {

        [ReadOnly] public string Id;
        [ContextMenuItem("Create new ID", "RegenId")]
        public Enemy Prefab;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = System.Guid.NewGuid().ToString();
            }
        }


        //[Button("Create new Id")]
        private void RegenId()
        {
            Id = System.Guid.NewGuid().ToString();
        }

    }
}
