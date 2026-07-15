using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [Header("UI References")]
    [Tooltip("Panel Pause Menu. Akan di-generate otomatis jika kosong.")]
    public GameObject pausePanel;

    [Header("UI Settings")]
    [Tooltip("Masukkan font Kurale (dari Assets/Fonts) ke kotak ini.")]
    public Font pauseMenuFont;

    [Tooltip("Sprite untuk Panel Background (Opsional)")]
    public Sprite backgroundSprite;

    [Tooltip("Sprite untuk Tombol (Opsional)")]
    public Sprite buttonSprite;

    private bool isPaused = false;
    
    // Panel References
    private GameObject centerPanel;
    private GameObject settingsPanel;

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
        if (SceneManager.GetActiveScene().name == "StartMenu")
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Jangan buka pause menu jika Jurnal sedang terbuka
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
            
            // Selalu reset ke panel utama saat pause dibuka
            if (isPaused && centerPanel != null && settingsPanel != null)
            {
                centerPanel.SetActive(true);
                settingsPanel.SetActive(false);
            }
        }

        Time.timeScale = isPaused ? 0f : 1f;

        // Opsional: Pause seluruh audio di game (Hapus baris ini jika ingin BGM tetap nyala saat pause)
        // AudioListener.pause = isPaused; 
    }

    // Fungsi pembantu untuk membuat UI Pause secara kode
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

        // 3. Create Center Panel (Main Menu)
        centerPanel = new GameObject("CenterPanel");
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

        Font kuraleFont = pauseMenuFont;
        if (kuraleFont == null) kuraleFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // 4. Create Title Text
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(centerPanel.transform, false);
        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "LontarQuest";
        titleText.font = kuraleFont;
        titleText.fontSize = 80;
        titleText.fontStyle = FontStyle.Italic;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = new Color(0.1f, 0.1f, 0.1f, 1f);
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(500, 150);
        titleRect.anchoredPosition = new Vector2(0, -50);

        // 5. Create Main Buttons
        GameObject resumeBtn = CreateButton("ResumeBtn", "Resume", centerPanel.transform, new Vector2(0, 120), kuraleFont);
        resumeBtn.GetComponent<Button>().onClick.AddListener(TogglePause);

        GameObject settingsBtn = CreateButton("SettingsBtn", "Settings", centerPanel.transform, new Vector2(0, 20), kuraleFont);
        settingsBtn.GetComponent<Button>().onClick.AddListener(() => {
            centerPanel.SetActive(false);
            settingsPanel.SetActive(true);
        });
        
        GameObject menuBtn = CreateButton("MenuBtn", "Main Menu", centerPanel.transform, new Vector2(0, -80), kuraleFont);
        menuBtn.GetComponent<Button>().onClick.AddListener(() => {
            Time.timeScale = 1f;
            isPaused = false;
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }
            SceneManager.LoadScene("StartMenu"); // Pindah ke Main Menu
        });

        GameObject quitBtn = CreateButton("QuitBtn", "Quit", centerPanel.transform, new Vector2(0, -180), kuraleFont);
        quitBtn.GetComponent<Button>().onClick.AddListener(() => {
            Time.timeScale = 1f;
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        });

        // 6. Create Settings Panel
        settingsPanel = new GameObject("SettingsPanel");
        settingsPanel.transform.SetParent(pausePanel.transform, false);
        Image settingsImg = settingsPanel.AddComponent<Image>();
        if (backgroundSprite != null)
        {
            settingsImg.sprite = backgroundSprite;
            settingsImg.type = Image.Type.Sliced;
        }
        settingsImg.color = new Color(0.95f, 0.95f, 0.95f, 1f);
        RectTransform settingsRect = settingsPanel.GetComponent<RectTransform>();
        settingsRect.anchorMin = new Vector2(0.5f, 0.5f);
        settingsRect.anchorMax = new Vector2(0.5f, 0.5f); 
        settingsRect.pivot = new Vector2(0.5f, 0.5f);
        settingsRect.sizeDelta = new Vector2(800, 800);
        settingsRect.anchoredPosition = Vector2.zero;

        // Settings Title
        GameObject sTitleObj = new GameObject("SettingsTitle");
        sTitleObj.transform.SetParent(settingsPanel.transform, false);
        Text sTitleText = sTitleObj.AddComponent<Text>();
        sTitleText.text = "Settings (Audio)";
        sTitleText.font = kuraleFont;
        sTitleText.fontSize = 70;
        sTitleText.fontStyle = FontStyle.Italic;
        sTitleText.alignment = TextAnchor.MiddleCenter;
        sTitleText.color = new Color(0.1f, 0.1f, 0.1f, 1f);
        RectTransform sTitleRect = sTitleObj.GetComponent<RectTransform>();
        sTitleRect.anchorMin = new Vector2(0.5f, 1f);
        sTitleRect.anchorMax = new Vector2(0.5f, 1f);
        sTitleRect.pivot = new Vector2(0.5f, 1f);
        sTitleRect.sizeDelta = new Vector2(500, 150);
        sTitleRect.anchoredPosition = new Vector2(0, -50);

        // Volume Label
        GameObject volLabelObj = new GameObject("VolumeLabel");
        volLabelObj.transform.SetParent(settingsPanel.transform, false);
        Text volLabelText = volLabelObj.AddComponent<Text>();
        volLabelText.text = "Master Volume";
        volLabelText.font = kuraleFont;
        volLabelText.fontSize = 45;
        volLabelText.alignment = TextAnchor.MiddleCenter;
        volLabelText.color = new Color(0.1f, 0.1f, 0.1f, 1f);
        RectTransform volLabelRect = volLabelObj.GetComponent<RectTransform>();
        volLabelRect.anchorMin = new Vector2(0.5f, 0.5f);
        volLabelRect.anchorMax = new Vector2(0.5f, 0.5f);
        volLabelRect.sizeDelta = new Vector2(400, 70);
        volLabelRect.anchoredPosition = new Vector2(0, 120);

        // Volume Slider (Native Unity Slider is complex to build entirely via script, so we use a simple UI approach)
        Slider volSlider = CreateSlider("VolumeSlider", settingsPanel.transform, new Vector2(0, 50));
        volSlider.value = AudioListener.volume;
        volSlider.onValueChanged.AddListener((float val) => {
            AudioListener.volume = val;
        });

        // Back Button
        GameObject backBtn = CreateButton("BackBtn", "Back", settingsPanel.transform, new Vector2(0, -180), kuraleFont);
        backBtn.GetComponent<Button>().onClick.AddListener(() => {
            settingsPanel.SetActive(false);
            centerPanel.SetActive(true);
        });

        // Finalize
        settingsPanel.SetActive(false);
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
        btnImage.color = new Color(0.85f, 0.85f, 0.85f, 1f);
        
        Button btn = buttonObj.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor = new Color(0.85f, 0.85f, 0.85f, 1f);
        cb.highlightedColor = new Color(0.95f, 0.95f, 0.95f, 1f);
        cb.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        btn.colors = cb;

        RectTransform btnRect = buttonObj.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.5f);
        btnRect.anchorMax = new Vector2(0.5f, 0.5f);
        btnRect.sizeDelta = new Vector2(300, 70); 
        btnRect.anchoredPosition = anchoredPos;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        Text btnText = textObj.AddComponent<Text>();
        btnText.text = textContent;
        btnText.font = btnFont;
        btnText.fontSize = 45;
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.color = new Color(0.1f, 0.1f, 0.1f, 1f);
        
        RectTransform txtRect = textObj.GetComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.offsetMin = Vector2.zero;
        txtRect.offsetMax = Vector2.zero;

        return buttonObj;
    }

    private Slider CreateSlider(string name, Transform parent, Vector2 anchoredPos)
    {
        // Setup base object
        GameObject sliderObj = new GameObject(name);
        sliderObj.transform.SetParent(parent, false);
        RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.5f, 0.5f);
        sliderRect.anchorMax = new Vector2(0.5f, 0.5f);
        sliderRect.sizeDelta = new Vector2(400, 30);
        sliderRect.anchoredPosition = anchoredPos;

        // Background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(sliderObj.transform, false);
        Image bgImg = bgObj.AddComponent<Image>();
        bgImg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0.25f);
        bgRect.anchorMax = new Vector2(1, 0.75f);
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // Fill Area
        GameObject fillAreaObj = new GameObject("Fill Area");
        fillAreaObj.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRect = fillAreaObj.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = new Vector2(5, 0);
        fillAreaRect.offsetMax = new Vector2(-15, 0);

        // Fill
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(fillAreaObj.transform, false);
        Image fillImg = fillObj.AddComponent<Image>();
        fillImg.color = new Color(0.8f, 0.5f, 0.2f, 1f); // Orange color
        RectTransform fillRect = fillObj.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.zero;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        // Handle Slide Area
        GameObject handleAreaObj = new GameObject("Handle Slide Area");
        handleAreaObj.transform.SetParent(sliderObj.transform, false);
        RectTransform handleAreaRect = handleAreaObj.AddComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.offsetMin = new Vector2(10, 0);
        handleAreaRect.offsetMax = new Vector2(-10, 0);

        // Handle
        GameObject handleObj = new GameObject("Handle");
        handleObj.transform.SetParent(handleAreaObj.transform, false);
        Image handleImg = handleObj.AddComponent<Image>();
        handleImg.color = new Color(1f, 1f, 1f, 1f);
        RectTransform handleRect = handleObj.GetComponent<RectTransform>();
        handleRect.anchorMin = Vector2.zero;
        handleRect.anchorMax = Vector2.zero;
        handleRect.sizeDelta = new Vector2(30, 0);
        handleRect.offsetMin = new Vector2(-15, 0);
        handleRect.offsetMax = new Vector2(15, 0);

        // Setup Slider Component
        Slider slider = sliderObj.AddComponent<Slider>();
        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImg;
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;

        return slider;
    }
}
