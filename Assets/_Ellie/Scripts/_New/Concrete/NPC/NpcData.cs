using Demo.Perception;
using TriInspector;
using UnityEngine;

namespace Horsepower.NPC
{
    [CreateAssetMenu(menuName = "Car/Enemy/New Npc")]
    public class NpcData : ScriptableObject
    {
        [ReadOnly] public string Id;
        public Enemy Prefab;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = System.Guid.NewGuid().ToString();
            }
        }

        /*
        [Button("Create new Id")]
        private void RegenId()
        {
            Id = System.Guid.NewGuid().ToString();
        }
        */
    }
}
