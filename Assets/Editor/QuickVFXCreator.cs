#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class QuickVFXCreator
{
    [MenuItem("Tools/Buat Efek Serangan Cepat (Instan)")]
    public static void CreateSlashVFX()
    {
        // Buat GameObject baru
        GameObject vfx = new GameObject("Hit_VFX_Otomatis");
        
        // Tambahkan Particle System
        ParticleSystem ps = vfx.AddComponent<ParticleSystem>();
        
        // Konfigurasi Main Module (Waktu, Ukuran, Kecepatan)
        var main = ps.main;
        main.duration = 0.2f;
        main.startLifetime = 0.15f;
        main.startSpeed = 15f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.6f);
        main.startColor = Color.red;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.playOnAwake = true;
        main.stopAction = ParticleSystemStopAction.Destroy; // Otomatis hancur setelah selesai

        // Konfigurasi Pancaran (Burst)
        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[]{ new ParticleSystem.Burst(0f, 20) }); // Ledakan 20 partikel seketika
        
        // Konfigurasi Bentuk (Menyebar / Kerucut)
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 60f;
        shape.radius = 0.1f;
        
        // Konfigurasi Tampilan
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = 50; // Agar selalu tampil di atas karakter
        
        // Simpan menjadi Prefab di folder Assets
        string folderPath = "Assets/Prefabs";
        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
            AssetDatabase.Refresh();
        }

        string prefabPath = folderPath + "/Hit_VFX_Otomatis.prefab";
        PrefabUtility.SaveAsPrefabAsset(vfx, prefabPath);
        GameObject.DestroyImmediate(vfx); // Hapus dari layar, kita cuma butuh Prefab-nya
        
        Debug.Log("✅ BERHASIL! Prefab 'Hit_VFX_Otomatis' sudah dibuat di folder: " + prefabPath);
        
        // Buka folder tersebut dan tandai Prefab-nya agar user langsung lihat
        Object prefabObj = AssetDatabase.LoadAssetAtPath<Object>(prefabPath);
        Selection.activeObject = prefabObj;
        EditorGUIUtility.PingObject(prefabObj);
    }
}
#endif
