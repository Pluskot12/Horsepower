using System.Collections.Generic;
using System.Linq;
using TriInspector;
using UnityEngine;

namespace CarGame
{
    [CreateAssetMenu(menuName = "Ellie/Enemy Database")]
    public class EnemyDatabase : ScriptableObject
    {
        [SerializeField] private string path;

        [SerializeField] private List<EnemyData> enemies;

        public EnemyData GetById(string id) => enemies.Find(i => i.Id == id);

        public T GetById<T>(string id) where T : EnemyData => GetById(id) as T;

#if UNITY_EDITOR

        [Button("Find Items")]
        public void AutoFillEnemies()
        {
            string rootPath = path;

            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:EnemyData", new[] { rootPath });
            var enemies = guids
                .Select(guid => UnityEditor.AssetDatabase.LoadAssetAtPath<EnemyData>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid)))
                .Where(r => r != null)
                .ToList();

            this.enemies = enemies;

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();

            Debug.Log("Fetched enemies. " + enemies.Count + " enemies found.");
        }
#endif
    }
}