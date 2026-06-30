using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [Header("UI References")]
    [Tooltip("Panel Pause Menu. Akan di-generate otomatis jika kosong.")]
    public GameObject pausePanel;

    [Header("UI Settings")]
    [Tooltip("Masukkan font Kurale (dari Assets/Fonts) ke kotak ini.")]
    public Font pauseMenuFont;

    [Tooltip("Sprite untuk Panel Background (Opsional, klik ikon lingkaran kecil untuk memilih 'Background' bawaan Unity)")]
    public Sprite backgroundSprite;

    [Tooltip("Sprite untuk Tombol (Opsional, klik ikon lingkaran kecil untuk memilih 'UISprite' bawaan Unity)")]
    public Sprite buttonSprite;

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (pausePanel == null)
        {
            GeneratePauseUI();
        }
        else
        {
            pausePanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Jangan buka pause menu jika Jurnal sedang terbuka (karena Jurnal juga pakai Time.timeScale = 0)
            if (JournalManager.Instance != null && JournalManager.Instance.journalUIPanel != null && JournalManager.Instance.journalUIPanel.activeSelf)
            {
                return;
            }

            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        
        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
        }

        Time.timeScale = isPaused ? 0f : 1f;

        // Opsional: Pause seluruh audio di game
        AudioListener.pause = isPaused;
    }

    // Fungsi pembantu untuk membuat UI Pause secara kode jika tidak diset manual
    private void GeneratePauseUI()
    {
        // 1. Create Canvas
        GameObject canvasObj = new GameObject("PauseCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 998;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();
        DontDestroyOnLoad(canvasObj);

        // 2. Create Background Panel (Latar gelap transparan)
        pausePanel = new GameObject("PausePanel");
        pausePanel.transform.SetParent(canvasObj.transform, false);
        Image bgImage = pausePanel.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.6f);
        RectTransform bgRect = pausePanel.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // 3. Create Center Panel (Warna putih/abu terang)
        GameObject centerPanel = new GameObject("CenterPanel");
        centerPanel.transform.SetParent(pausePanel.transform, false);
        Image centerImg = centerPanel.AddComponent<Image>();
        if (backgroundSprite != null)
        {
            centerImg.sprite = backgroundSprite;
            centerImg.type = Image.Type.Sliced;
        }
        centerImg.color = new Color(0.95f, 0.95f, 0.95f, 1f);
        RectTransform centerRect = centerPanel.GetComponent<RectTransform>();
        centerRect.anchorMin = new Vector2(0.5f, 0.5f);
        centerRect.anchorMax = new Vector2(0.5f, 0.5f); 
        centerRect.pivot = new Vector2(0.5f, 0.5f);
        centerRect.sizeDelta = new Vector2(800, 800);
        centerRect.anchoredPosition = Vector2.zero;

        // Gunakan font dari Inspector, jika kosong gunakan font bawaan Unity
        Font kuraleFont = pauseMenuFont;
        if (kuraleFont == null) kuraleFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // 4. Create Title Text (LontarQuest)
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(centerPanel.transform, false);
        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "LontarQuest";
        titleText.font = kuraleFont;
        titleText.fontSize = 80;
        titleText.fontStyle = FontStyle.Italic;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = new Color(0.1f, 0.1f, 0.1f, 1f); // Hitam gelap
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(500, 150);
        titleRect.anchoredPosition = new Vector2(0, -50);

        // 5. Create Buttons
        // Resume
        GameObject resumeBtn = CreateButton("ResumeBtn", "Resume", centerPanel.transform, new Vector2(0, 120), kuraleFont);
        resumeBtn.GetComponent<Button>().onClick.AddListener(TogglePause);

        // Settings
        GameObject settingsBtn = CreateButton("SettingsBtn", "Settings", centerPanel.transform, new Vector2(0, 20), kuraleFont);
        
        // Main Menu
        GameObject menuBtn = CreateButton("MenuBtn", "Main Menu", centerPanel.transform, new Vector2(0, -80), kuraleFont);

        // Quit
        GameObject quitBtn = CreateButton("QuitBtn", "Quit", centerPanel.transform, new Vector2(0, -180), kuraleFont);
        quitBtn.GetComponent<Button>().onClick.AddListener(() => {
            Time.timeScale = 1f;
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        });

        pausePanel.SetActive(false);
    }

    private GameObject CreateButton(string name, string textContent, Transform parent, Vector2 anchoredPos, Font btnFont)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);
        
        Image btnImage = buttonObj.AddComponent<Image>();
        if (buttonSprite != null)
        {
            btnImage.sprite = buttonSprite;
            btnImage.type = Image.Type.Sliced;
        }
        btnImage.color = new Color(0.85f, 0.85f, 0.85f, 1f); // Abu-abu terang khas referensi
        
        Button btn = buttonObj.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor = new Color(0.85f, 0.85f, 0.85f, 1f);
        cb.highlightedColor = new Color(0.95f, 0.95f, 0.95f, 1f);
        cb.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        btn.colors = cb;

        RectTransform btnRect = buttonObj.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.5f);
        btnRect.anchorMax = new Vector2(0.5f, 0.5f);
        btnRect.sizeDelta = new Vector2(300, 70); // Ukuran tombol memanjang
        btnRect.anchoredPosition = anchoredPos;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        Text btnText = textObj.AddComponent<Text>();
        btnText.text = textContent;
        btnText.font = btnFont;
        btnText.fontSize = 45;
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.color = new Color(0.1f, 0.1f, 0.1f, 1f); // Teks Hitam
        
        RectTransform txtRect = textObj.GetComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.offsetMin = Vector2.zero;
        txtRect.offsetMax = Vector2.zero;

        return buttonObj;
    }
}
