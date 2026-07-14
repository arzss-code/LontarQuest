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

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUILayout.Label("UI Helpers", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Alat untuk membuat UI otomatis agar tidak perlu setup manual yang rumit.", MessageType.Info);
        
        if (GUILayout.Button("Generate Boon UI Prefab"))
        {
            GenerateBoonUIPrefab();
        }

        EditorGUILayout.Space();
        GUILayout.Label("Stage 2 Helpers", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Alat untuk mengotomatiskan setup Intro & Dialog Stage 2 (Bagian B).", MessageType.Info);
        
        if (GUILayout.Button("Setup Stage 2 (Bagian B)"))
        {
            SetupStage2PartB();
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

    private void GenerateBoonUIPrefab()
    {
        string prefabsPath = "Assets/Prefabs/Systems";
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs")) AssetDatabase.CreateFolder("Assets", "Prefabs");
        if (!AssetDatabase.IsValidFolder(prefabsPath)) AssetDatabase.CreateFolder("Assets/Prefabs", "Systems");

        // Create Canvas
        GameObject canvasObj = new GameObject("BoonSelectionCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // Tambahkan BoonUIManager langsung ke Canvas ini!
        BoonUIManager manager = canvasObj.AddComponent<BoonUIManager>();

        // Create Panel
        GameObject panelObj = new GameObject("BoonSelectionPanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        UnityEngine.UI.Image panelImg = panelObj.AddComponent<UnityEngine.UI.Image>();
        panelImg.color = new Color(0, 0, 0, 0.8f);
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;

        // Create Horizontal Layout Group for Buttons
        UnityEngine.UI.HorizontalLayoutGroup hlg = panelObj.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>();
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.spacing = 50;
        hlg.childControlWidth = false;
        hlg.childControlHeight = false;

        System.Collections.Generic.List<BoonUIElement> generatedElements = new System.Collections.Generic.List<BoonUIElement>();

        // Create 3 Buttons
        for (int i = 0; i < 3; i++)
        {
            GameObject btnObj = new GameObject($"BoonButton_{i+1}");
            btnObj.transform.SetParent(panelObj.transform, false);
            
            // Background Image untuk Card
            UnityEngine.UI.Image btnImg = btnObj.AddComponent<UnityEngine.UI.Image>();
            btnImg.color = new Color(0.15f, 0.15f, 0.15f, 1f); // Warna gelap agar tulisan terbaca
            
            UnityEngine.UI.Button btn = btnObj.AddComponent<UnityEngine.UI.Button>();
            
            RectTransform btnRect = btnObj.GetComponent<RectTransform>();
            btnRect.sizeDelta = new Vector2(280, 420); // Sedikit lebih besar

            BoonUIElement uiElement = btnObj.AddComponent<BoonUIElement>();
            generatedElements.Add(uiElement);

            // Name Text
            GameObject nameObj = new GameObject("NameText");
            nameObj.transform.SetParent(btnObj.transform, false);
            TMPro.TextMeshProUGUI nameText = nameObj.AddComponent<TMPro.TextMeshProUGUI>();
            nameText.text = "Nama Boon";
            nameText.color = new Color(1f, 0.84f, 0f, 1f); // Warna emas
            nameText.fontSize = 24;
            nameText.fontStyle = TMPro.FontStyles.Bold;
            nameText.alignment = TMPro.TextAlignmentOptions.Center;
            RectTransform nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0.8f);
            nameRect.anchorMax = new Vector2(1, 1);
            nameRect.sizeDelta = Vector2.zero;

            // Icon Image
            GameObject iconObj = new GameObject("IconImage");
            iconObj.transform.SetParent(btnObj.transform, false);
            UnityEngine.UI.Image iconImg = iconObj.AddComponent<UnityEngine.UI.Image>();
            RectTransform iconRect = iconObj.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.5f, 0.6f);
            iconRect.anchorMax = new Vector2(0.5f, 0.6f);
            iconRect.sizeDelta = new Vector2(100, 100);
            iconRect.anchoredPosition = Vector2.zero;

            // Desc Text
            GameObject descObj = new GameObject("DescText");
            descObj.transform.SetParent(btnObj.transform, false);
            TMPro.TextMeshProUGUI descText = descObj.AddComponent<TMPro.TextMeshProUGUI>();
            descText.text = "Deskripsi dari boon ini akan muncul di sini...";
            descText.color = Color.white;
            descText.fontSize = 18;
            descText.alignment = TMPro.TextAlignmentOptions.Center;
            descText.enableWordWrapping = true;
            RectTransform descRect = descObj.GetComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0.1f, 0.05f);
            descRect.anchorMax = new Vector2(0.9f, 0.45f);
            descRect.sizeDelta = Vector2.zero;

            // Wire up fields via SerializedObject to access private SerializeFields
            SerializedObject serializedObject = new SerializedObject(uiElement);
            serializedObject.Update();
            serializedObject.FindProperty("nameText").objectReferenceValue = nameText;
            serializedObject.FindProperty("descriptionText").objectReferenceValue = descText;
            serializedObject.FindProperty("iconImage").objectReferenceValue = iconImg;
            serializedObject.FindProperty("selectButton").objectReferenceValue = btn;
            serializedObject.ApplyModifiedProperties();
        }

        // Setup Manager references
        manager.boonSelectionPanel = panelObj;
        manager.boonUIElements = generatedElements;

        // Auto-populate BoonData assets if any exist in the project
        string[] boonGuids = AssetDatabase.FindAssets("t:BoonData");
        manager.allAvailableBoons = new System.Collections.Generic.List<BoonData>();
        foreach (string guid in boonGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            BoonData boon = AssetDatabase.LoadAssetAtPath<BoonData>(assetPath);
            if (boon != null) manager.allAvailableBoons.Add(boon);
        }

        // Save as Prefab
        string path = $"{prefabsPath}/BoonSelectionCanvas.prefab";
        PrefabUtility.SaveAsPrefabAsset(canvasObj, path);
        DestroyImmediate(canvasObj);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"✅ Boon UI Prefab Generated at {path}!");
    }

    [MenuItem("LontarQuest/Setup Stage 2 (Bagian B)")]
    public static void SetupStage2PartB()
    {
        // 1. Verifikasi scene
        if (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name != "Stage2")
        {
            EditorUtility.DisplayDialog("Error", "Buka scene 'Stage2' terlebih dahulu sebelum menjalankan setup ini!", "OK");
            return;
        }

        // 2. Cari GameObjects
        GameObject introManager = GameObject.Find("IntroManager");
        GameObject introDialogueManager = GameObject.Find("IntroDialogueManager");
        GameObject postBossDialogueManager = GameObject.Find("PostBossDialogueManager");
        GameObject introStart = GameObject.Find("IntroStart");
        GameObject introWalk = GameObject.Find("IntroWalk");

        if (introManager == null)
        {
            Debug.LogError("[Stage2 Setup] GameObject 'IntroManager' tidak ditemukan!");
            EditorUtility.DisplayDialog("Error", "GameObject 'IntroManager' tidak ditemukan!", "OK");
            return;
        }

        // Aktifkan GameObjects
        introManager.SetActive(true);
        if (introDialogueManager != null) introDialogueManager.SetActive(true);
        if (postBossDialogueManager != null) postBossDialogueManager.SetActive(true);
        if (introStart != null) introStart.SetActive(true);
        if (introWalk != null) introWalk.SetActive(true);

        // 3. Konfigurasi IntroManager
        // Hapus Stage1IntroStarter lama jika ada
        Stage1IntroStarter oldStarter = introManager.GetComponent<Stage1IntroStarter>();
        if (oldStarter != null)
        {
            DestroyImmediate(oldStarter);
            Debug.Log("[Stage2 Setup] Menghapus Stage1IntroStarter lama dari IntroManager.");
        }

        // Tambah Stage2IntroStarter baru
        Stage2IntroStarter newStarter = introManager.GetComponent<Stage2IntroStarter>();
        if (newStarter == null)
        {
            newStarter = introManager.AddComponent<Stage2IntroStarter>();
            Debug.Log("[Stage2 Setup] Menambahkan Stage2IntroStarter ke IntroManager.");
        }

        // Konfigurasi field Stage2IntroStarter via SerializedObject
        SerializedObject soStarter = new SerializedObject(newStarter);
        soStarter.Update();

        if (introDialogueManager != null)
        {
            IntroDialogue introDialogue = introDialogueManager.GetComponent<IntroDialogue>();
            soStarter.FindProperty("introDialogue").objectReferenceValue = introDialogue;
        }
        if (introStart != null)
        {
            soStarter.FindProperty("portalSpawnPoint").objectReferenceValue = introStart.transform;
        }
        if (introWalk != null)
        {
            soStarter.FindProperty("walkDestination").objectReferenceValue = introWalk.transform;
        }
        soStarter.FindProperty("walkSpeed").floatValue = 3f;
        soStarter.FindProperty("startDelay").floatValue = 0.5f;
        soStarter.FindProperty("initialQuest").stringValue = "Jelajahi Perpustakaan Melayang";
        soStarter.ApplyModifiedProperties();

        // 4. Konfigurasi Dialog Intro (IntroDialogueManager)
        if (introDialogueManager != null)
        {
            IntroDialogue introDialogue = introDialogueManager.GetComponent<IntroDialogue>();
            if (introDialogue != null)
            {
                SerializedObject soDiag = new SerializedObject(introDialogue);
                soDiag.Update();
                soDiag.FindProperty("freezePlayer").boolValue = true;
                soDiag.FindProperty("unfreezePlayerWhenFinished").boolValue = true;

                SerializedProperty dialoguesProp = soDiag.FindProperty("dialogues");
                dialoguesProp.ClearArray();

                // Dialog 1
                dialoguesProp.InsertArrayElementAtIndex(0);
                SerializedProperty diag0 = dialoguesProp.GetArrayElementAtIndex(0);
                diag0.FindPropertyRelative("speaker").stringValue = "Saka";
                diag0.FindPropertyRelative("text").stringValue = "Tempat apa ini...? Rak-rak buku melayang di tengah kekosongan. Seperti perpustakaan yang ditinggalkan oleh waktu.";

                // Dialog 2
                dialoguesProp.InsertArrayElementAtIndex(1);
                SerializedProperty diag1 = dialoguesProp.GetArrayElementAtIndex(1);
                diag1.FindPropertyRelative("speaker").stringValue = "Saka";
                diag1.FindPropertyRelative("text").stringValue = "Lontar-lontar berterbangan di udara... Pasti ada sesuatu yang penting tersembunyi di sini.";

                // Dialog 3
                dialoguesProp.InsertArrayElementAtIndex(2);
                SerializedProperty diag2 = dialoguesProp.GetArrayElementAtIndex(2);
                diag2.FindPropertyRelative("speaker").stringValue = "Saka";
                diag2.FindPropertyRelative("text").stringValue = "Tapi batu-batu penjaga itu masih bergerak. Aku harus tetap waspada.";

                soDiag.ApplyModifiedProperties();
                Debug.Log("[Stage2 Setup] Konfigurasi monolog pembuka Saka selesai.");
            }
        }

        // 5. Konfigurasi Dialog Pasca-Boss (PostBossDialogueManager)
        if (postBossDialogueManager != null)
        {
            IntroDialogue postBossDialogue = postBossDialogueManager.GetComponent<IntroDialogue>();
            if (postBossDialogue != null)
            {
                SerializedObject soDiag = new SerializedObject(postBossDialogue);
                soDiag.Update();
                soDiag.FindProperty("freezePlayer").boolValue = true;
                soDiag.FindProperty("unfreezePlayerWhenFinished").boolValue = true;

                SerializedProperty dialoguesProp = soDiag.FindProperty("dialogues");
                dialoguesProp.ClearArray();

                // Dialog 1
                dialoguesProp.InsertArrayElementAtIndex(0);
                SerializedProperty diag0 = dialoguesProp.GetArrayElementAtIndex(0);
                diag0.FindPropertyRelative("speaker").stringValue = "Saka";
                diag0.FindPropertyRelative("text").stringValue = "Makhluk itu... panah energinya bisa membunuhku kalau aku lengah sedikit saja.";

                // Dialog 2
                dialoguesProp.InsertArrayElementAtIndex(1);
                SerializedProperty diag1 = dialoguesProp.GetArrayElementAtIndex(1);
                diag1.FindPropertyRelative("speaker").stringValue = "Saka";
                diag1.FindPropertyRelative("text").stringValue = "Perpustakaan ini menyimpan sesuatu. Kenapa ada roh penjaga sekuat ini di tempat yang seharusnya sudah lama mati?";

                // Dialog 3
                dialoguesProp.InsertArrayElementAtIndex(2);
                SerializedProperty diag2 = dialoguesProp.GetArrayElementAtIndex(2);
                diag2.FindPropertyRelative("speaker").stringValue = "Saka";
                diag2.FindPropertyRelative("text").stringValue = "Lontar itu bersinar... Sepertinya ada kekuatan kuno lagi yang bisa kuserap. Aku harus mengambilnya.";

                soDiag.ApplyModifiedProperties();
                Debug.Log("[Stage2 Setup] Konfigurasi monolog pasca-boss Saka selesai.");
            }
        }

        // Tandai scene kotor agar Unity menyimpannya
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        EditorUtility.DisplayDialog("Success", "Setup Stage 2 Bagian B selesai dikonfigurasi! Silakan save scene (Ctrl+S).", "OK");
    }
}
#endif
