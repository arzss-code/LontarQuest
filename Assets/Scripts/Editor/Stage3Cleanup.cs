using UnityEngine;
using UnityEditor;
using System.Linq;

public class Stage3Cleanup : MonoBehaviour
{
    [MenuItem("LontarQuest/Clean Stage 3 Prefabs")]
    public static void CleanPrefabs()
    {
        string[] prefabs = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        foreach (string guid in prefabs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (obj != null)
            {
                bool modified = false;
                
                // Remove missing scripts
                int removedCount = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
                if (removedCount > 0)
                {
                    Debug.Log($"Removed {removedCount} missing scripts from {obj.name}");
                    modified = true;
                }

                // If it's a player prefab (has PlayerController or PlayerStats)
                if (obj.name.Contains("Saka") || obj.GetComponentInChildren<PlayerController>() != null)
                {
                    if (obj.GetComponent<PlayerStats>() == null)
                    {
                        obj.AddComponent<PlayerStats>();
                        modified = true;
                        Debug.Log($"Added PlayerStats to {obj.name}");
                    }
                    if (obj.GetComponent<PlayerModifier>() == null)
                    {
                        obj.AddComponent<PlayerModifier>();
                        modified = true;
                        Debug.Log($"Added PlayerModifier to {obj.name}");
                    }
                }

                if (modified)
                {
                    PrefabUtility.SavePrefabAsset(obj);
                }
            }
        }
        
        Debug.Log("Finished cleaning prefabs!");
    }
}
