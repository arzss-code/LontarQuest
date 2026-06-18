#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class LontarQuestSetupTool : EditorWindow
{
    [MenuItem("LontarQuest/Setup Roguelike Data & Prefabs")]
    public static void ShowWindow()
    {
        GetWindow<LontarQuestSetupTool>("LontarQuest Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Roguelike & Journal System Setup", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Alat ini akan secara otomatis membuat struktur folder Data, sampel ScriptableObjects (Boon & Lore), serta kerangka Prefab untuk sistem Roguelike LontarQuest.", MessageType.Info);

        if (GUILayout.Button("Generate Data Folders & ScriptableObjects"))
        {
            GenerateDataAssets();
        }

        if (GUILayout.Button("Generate Base Prefabs"))
        {
            GeneratePrefabs();
        }

        EditorGUILayout.Space();
        GUILayout.Label("Stage Specific Setup", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Sesuai dengan GDD, Stage 1 difokuskan pada Candi, Kepala Kala, dan Aksara Batak.", MessageType.Info);
        
        if (GUILayout.Button("Generate Stage 1 Content (Kepala Kala & Batak)"))
        {
            GenerateStage1Content();
        }
    }

    private void GenerateDataAssets()
    {
        string dataPath = "Assets/Data";
        if (!AssetDatabase.IsValidFolder(dataPath)) AssetDatabase.CreateFolder("Assets", "Data");

        string boonsPath = "Assets/Data/Boons";
        if (!AssetDatabase.IsValidFolder(boonsPath)) AssetDatabase.CreateFolder("Assets/Data", "Boons");

        string loresPath = "Assets/Data/Lores";
        if (!AssetDatabase.IsValidFolder(loresPath)) AssetDatabase.CreateFolder("Assets/Data", "Lores");

        // Generate Sample Boon
        BoonData sampleBoon = ScriptableObject.CreateInstance<BoonData>();
        sampleBoon.boonName = "Lari Kencang Lontara";
        sampleBoon.type = BoonType.Lontara;
        sampleBoon.movementSpeedBonus = 2.0f;
        
        string assetPath = $"{boonsPath}/Boon_KecepatanLontara.asset";
        if (!File.Exists(assetPath))
        {
            AssetDatabase.CreateAsset(sampleBoon, assetPath);
        }

        // Generate Sample Lore
        LoreData sampleLore = ScriptableObject.CreateInstance<LoreData>();
        sampleLore.loreID = "monster_kepalakala";
        sampleLore.monsterName = "Kepala Kala";
        sampleLore.mythologyDescription = "Wajah raksasa batu tanpa rahang bawah yang terlepas dari ambang pintu candi. Makhluk buta ini melayang berpatroli dan mendeteksi suara.";
        
        string loreAssetPath = $"{loresPath}/Lore_KepalaKala.asset";
        if (!File.Exists(loreAssetPath))
        {
            AssetDatabase.CreateAsset(sampleLore, loreAssetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✅ Data Folders and Sample ScriptableObjects Generated!");
    }

    private void GeneratePrefabs()
    {
        string prefabsPath = "Assets/Prefabs";
        if (!AssetDatabase.IsValidFolder(prefabsPath)) AssetDatabase.CreateFolder("Assets", "Prefabs");

        string systemsPrefabsPath = "Assets/Prefabs/Systems";
        if (!AssetDatabase.IsValidFolder(systemsPrefabsPath)) AssetDatabase.CreateFolder("Assets/Prefabs", "Systems");

        // 1. Global Managers Prefab
        GameObject globalManagers = new GameObject("Global_Managers");
        globalManagers.AddComponent<JournalManager>();
        globalManagers.AddComponent<BoonUIManager>();
        PrefabUtility.SaveAsPrefabAsset(globalManagers, $"{systemsPrefabsPath}/Global_Managers.prefab");
        DestroyImmediate(globalManagers);

        // 2. Room Template Prefab
        GameObject roomTemplate = new GameObject("Room_Template");
        RoomManager rmBase = roomTemplate.AddComponent<RoomManager>();
        BoxCollider2D colBase = roomTemplate.GetComponent<BoxCollider2D>();
        colBase.isTrigger = true;
        colBase.size = new Vector2(15, 10); // Area trigger ruangan standar
        
        GameObject enemiesHolder = new GameObject("Enemies");
        enemiesHolder.transform.SetParent(roomTemplate.transform);
        
        GameObject doorsHolder = new GameObject("Doors");
        doorsHolder.transform.SetParent(roomTemplate.transform);

        PrefabUtility.SaveAsPrefabAsset(roomTemplate, $"{systemsPrefabsPath}/Room_Template.prefab");
        DestroyImmediate(roomTemplate);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✅ Base Prefabs Generated!");
    }

    private void GenerateStage1Content()
    {
        // 1. Generate Boon Batak (Defense)
        string boonsPath = "Assets/Data/Boons";
        if (!AssetDatabase.IsValidFolder("Assets/Data")) AssetDatabase.CreateFolder("Assets", "Data");
        if (!AssetDatabase.IsValidFolder(boonsPath)) AssetDatabase.CreateFolder("Assets/Data", "Boons");

        BoonData batakBoon = ScriptableObject.CreateInstance<BoonData>();
        batakBoon.boonName = "Ketahanan Batak";
        batakBoon.type = BoonType.Batak;
        batakBoon.damageReduction = 0.2f; // 20% kurang damage
        batakBoon.description = "Boon pelindung yang cocok untuk bertahan di Stage 1 melawan ledakan Kepala Kala.";
        
        string batakAssetPath = $"{boonsPath}/Boon_KetahananBatak.asset";
        if (!File.Exists(batakAssetPath))
        {
            AssetDatabase.CreateAsset(batakBoon, batakAssetPath);
        }

        // Pastikan lore KepalaKala juga digenerate kalau belum ada (misal player skip tombol pertama)
        GenerateDataAssets();

        // 2. Generate Room Template Stage 1
        string stage1PrefabsPath = "Assets/Prefabs/Stage1";
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs")) AssetDatabase.CreateFolder("Assets", "Prefabs");
        if (!AssetDatabase.IsValidFolder(stage1PrefabsPath)) AssetDatabase.CreateFolder("Assets/Prefabs", "Stage1");

        GameObject roomTemplateStage1 = new GameObject("Room_Template_Stage1");
        RoomManager rm = roomTemplateStage1.AddComponent<RoomManager>();
        BoxCollider2D stage1Col = roomTemplateStage1.GetComponent<BoxCollider2D>();
        stage1Col.isTrigger = true;
        stage1Col.size = new Vector2(15, 10); // Area trigger ruangan Stage 1

        GameObject enemiesHolder = new GameObject("Enemies");
        enemiesHolder.transform.SetParent(roomTemplateStage1.transform);

        GameObject doorsHolder = new GameObject("Doors");
        doorsHolder.transform.SetParent(roomTemplateStage1.transform);

        // Tambahkan Boon Door Batak ke dalam template ruangan
        GameObject batakDoor = new GameObject("BoonDoor_Batak");
        batakDoor.transform.SetParent(doorsHolder.transform);
        batakDoor.transform.localPosition = new Vector3(0, 5, 0); // Asumsi di atas ruangan
        
        BoxCollider2D col = batakDoor.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        
        BoonDoor bd = batakDoor.AddComponent<BoonDoor>();
        bd.doorRewardType = BoonType.Batak;

        PrefabUtility.SaveAsPrefabAsset(roomTemplateStage1, $"{stage1PrefabsPath}/Room_Template_Stage1.prefab");
        DestroyImmediate(roomTemplateStage1);

        // 3. Generate Lore Interactable Stage 1
        GameObject prasasti = new GameObject("Prasasti_KepalaKala");
        BoxCollider2D loreCol = prasasti.AddComponent<BoxCollider2D>();
        loreCol.isTrigger = true;
        loreCol.size = new Vector2(2, 2);
        
        LoreInteractable li = prasasti.AddComponent<LoreInteractable>();
        // Load Lore data yang ada
        LoreData loreData = AssetDatabase.LoadAssetAtPath<LoreData>("Assets/Data/Lores/Lore_KepalaKala.asset");
        if (loreData != null)
        {
            li.loreDataToUnlock = loreData;
        }

        PrefabUtility.SaveAsPrefabAsset(prasasti, $"{stage1PrefabsPath}/Prasasti_KepalaKala.prefab");
        DestroyImmediate(prasasti);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✅ Stage 1 Content Generated (Boon Batak, Room Template, Prasasti)!");
    }
}
#endif
