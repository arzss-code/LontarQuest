using UnityEngine;
using UnityEditor;

public class PrefabReplacer : EditorWindow
{
    private GameObject prefabInduk;

    [MenuItem("Tools/Alat Sihir/Ganti ke Prefab")]
    public static void ShowWindow()
    {
        GetWindow<PrefabReplacer>("Prefab Replacer");
    }

    private void OnGUI()
    {
        GUILayout.Label("1. Tarik Prefab Gana dari Folder Project ke sini:", EditorStyles.boldLabel);
        prefabInduk = (GameObject)EditorGUILayout.ObjectField("Prefab Induk", prefabInduk, typeof(GameObject), false);

        GUILayout.Space(10);
        GUILayout.Label("2. Seleksi semua Arca_Gana_1 di Hierarchy,", EditorStyles.label);
        GUILayout.Label("   lalu klik tombol di bawah ini:", EditorStyles.label);

        if (GUILayout.Button("UBAH SEMUA JADI PREFAB"))
        {
            if (prefabInduk == null)
            {
                EditorUtility.DisplayDialog("Error", "Prefab Induk belum dimasukkan!", "OK");
                return;
            }

            GameObject[] objekTerpilih = Selection.gameObjects;

            if (objekTerpilih.Length == 0)
            {
                EditorUtility.DisplayDialog("Info", "Pilih objek yang mau diganti di Hierarchy dulu bang!", "OK");
                return;
            }

            foreach (GameObject go in objekTerpilih)
            {
                // Membuat clone dari prefab baru di posisi objek lama
                GameObject objekBaru = (GameObject)PrefabUtility.InstantiatePrefab(prefabInduk);
                objekBaru.transform.position = go.transform.position;
                objekBaru.transform.rotation = go.transform.rotation;
                objekBaru.transform.localScale = go.transform.localScale;
                objekBaru.transform.parent = go.transform.parent;
                objekBaru.name = go.name; // Mempertahankan nama asli

                // Mendaftarkan fitur Undo biar kalau salah bisa Ctrl+Z
                Undo.RegisterCreatedObjectUndo(objekBaru, "Replace with Prefab");
                Undo.DestroyObjectImmediate(go);
            }
        }
    }
}