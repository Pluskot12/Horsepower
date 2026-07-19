using UnityEditor;
using UnityEngine;

namespace CarGame
{
    [CreateAssetMenu(menuName = "Car/Item/New Gadget")]
    public class GadgetItem : ItemData
    {
        [Header("Gadget")]
        public Gadget prefab;
        public Slot slot;

        public enum Slot 
        {
            Back,
            Middle, 
            Front
        }

        [ContextMenu("Create Recipe SO")]
        void CreateRecipe()
        {
#if UNITY_EDITOR
            CraftingRecipe recipe = ScriptableObject.CreateInstance<CraftingRecipe>();
            recipe.item = this;

            string path = AssetDatabase.GetAssetPath(this);
            string directory = System.IO.Path.GetDirectoryName(path);
            string assetPath = $"{directory}/Recipe - {displayName}.asset";

            // Ensure unique path in case the asset already exists
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

            AssetDatabase.CreateAsset(recipe, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = recipe;

            Debug.Log($"Created CraftingRecipe at: {assetPath}");
#endif
        }
    }
}
