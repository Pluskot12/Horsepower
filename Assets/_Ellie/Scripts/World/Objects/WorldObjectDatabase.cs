using System.Collections.Generic;
using System.Linq;
using TriInspector;
using UnityEngine;

namespace CarGame
{
    [CreateAssetMenu(menuName = "Ellie/World Object Database")]
    public class WorldObjectDatabase : ScriptableObject
    {
        [SerializeField] private string path;

        [SerializeField] private List<WorldObjectData> items;

        public WorldObjectData GetById(string id) => items.Find(i => i.Id == id);

        public T GetById<T>(string id) where T : WorldObjectData => GetById(id) as T;

#if UNITY_EDITOR

        [Button("Find Objects")]
        public void AutoFillItems()
        {
            string rootPath = path;

            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:WorldObjectData", new[] { rootPath });
            var data = guids
                .Select(guid => UnityEditor.AssetDatabase.LoadAssetAtPath<WorldObjectData>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid)))
                .Where(r => r != null)
                .ToList();

            items = data;

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();

            Debug.Log("Fetched world objects. " + data.Count + " items found.");
        }
#endif
    }
}
