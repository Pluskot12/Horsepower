using System.Collections.Generic;
using System.IO;
using System.Linq;
using TriInspector;
using UnityEngine;

namespace CarGame
{
    [CreateAssetMenu(menuName = "Ellie/Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        [SerializeField] private string path;

        [SerializeField] private List<ItemData> items;

        public ItemData GetById(string id) => items.Find(i => i.Id == id);

        public T GetById<T>(string id) where T : ItemData => GetById(id) as T;

#if UNITY_EDITOR

        [Button("Find Items")]
        public void AutoFillItems()
        {
            string rootPath = path;

            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:ItemData", new[] { rootPath });
            var recipes = guids
                .Select(guid => UnityEditor.AssetDatabase.LoadAssetAtPath<ItemData>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid)))
                .Where(r => r != null)
                .ToList();

            items = recipes;

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();

            Debug.Log("Fetched items. " + recipes.Count + " items found.");
        }
#endif
    }

}
