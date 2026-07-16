using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartMenuManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject anyKeyText;
    public CanvasGroup fadePanel;

    [Header("Scene")]
    public string nextSceneName = "Story-1";
    public string continueSceneName = "SafeHub";

    [Header("Font Settings")]
    [Tooltip("Font untuk UI Main Menu. Jika kosong, akan memakai font LegacyRuntime.")]
    public Font menuFont;

    private bool canPress = false;
    private bool loading = false;
    private bool menuActive = false;

    private GameObject mainMenuPanel;

    IEnumerator Start()
    {
        // Sembunyikan text saat awal
        if (anyKeyText != null)
            anyKeyText.SetActive(false);

        // Tunggu animasi logo selesai
        yield return new WaitForSeconds(0.8f);

        // Munculkan text
        if (anyKeyText != null)
            anyKeyText.SetActive(true);

        // Sekarang player boleh menekan tombol
        canPress = true;

        // Inisialisasi SaveManager jika belum ada (safety fallback)
        if (SaveManager.Instance == null)
        {
            new GameObject("SaveManager (Auto)").AddComponent<SaveManager>();
        }
    }

    void Update()
    {
        if (!canPress || loading || menuActive)
            return;

        if (Input.anyKeyDown)
        {
            Debug.Log("Any Key Ditekan, Membuka Main Menu");
            ShowMainMenu();
        }
    }

    private void ShowMainMenu()
    {
        menuActive = true;
        
        // Sembunyikan text "Press Any Key"
        if (anyKeyText != null)
            anyKeyText.SetActive(false);

        // Buat UI Main Menu secara dinamis
        GenerateMainMenuUI();
    }

    private void GenerateMainMenuUI()
    {
        // 1. Temukan Canvas di scene (utamakan Canvas lokal tempat anyKeyText berada)
        Canvas canvas = null;
        if (anyKeyText != null)
        {
            canvas = anyKeyText.GetComponentInParent<Canvas>();
        }

        if (canvas == null)
        {
            // Cari Canvas di scene, hindari Canvas persisten milik PauseManager
            Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            foreach (Canvas c in canvases)
            {
                if (c.gameObject.name != "PauseCanvas" && c.gameObject.name != "JournalCanvas")
                {
                    canvas = c;
                    break;
                }
            }
        }

        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("MainMenuCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // 2. Buat Main Menu Panel
        mainMenuPanel = new GameObject("MainMenuPanel");
        mainMenuPanel.transform.SetParent(canvas.transform, false);
        
        // Atur posisi panel di tengah layar bawah
        RectTransform panelRect = mainMenuPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.35f);
        panelRect.anchorMax = new Vector2(0.5f, 0.35f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(400, 300);
        panelRect.anchoredPosition = Vector2.zero;

        Font fontToUse = menuFont;
        if (fontToUse == null) fontToUse = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Cek apakah berkas save game ada
        bool hasSave = false;
        if (SaveManager.Instance != null)
        {
            hasSave = SaveManager.Instance.HasSaveFile();
        }

        // 3. Buat Tombol Lanjutkan (Continue)
        GameObject continueBtn = CreateButton("ContinueBtn", "Lanjutkan", mainMenuPanel.transform, new Vector2(0, 80), fontToUse);
        Button btnContinue = continueBtn.GetComponent<Button>();
        btnContinue.interactable = hasSave;
        
        // Jika tidak ada save, buat tombol tampak memudar
        if (!hasSave)
        {
            Image img = continueBtn.GetComponent<Image>();
            if (img != null) img.color = new Color(0.4f, 0.4f, 0.4f, 0.5f);
            Text txt = continueBtn.GetComponentInChildren<Text>();
            if (txt != null) txt.color = new Color(0.6f, 0.6f, 0.6f, 0.5f);
        }
        
        btnContinue.onClick.AddListener(() => {
            if (loading) return;
            loading = true;
            Debug.Log("Melanjutkan Game dari SafeHub");
            StartCoroutine(LoadSceneWithFade(continueSceneName));
        });

        // 4. Buat Tombol Game Baru (New Game)
        GameObject newGameBtn = CreateButton("NewGameBtn", "Game Baru", mainMenuPanel.transform, new Vector2(0, 0), fontToUse);
        newGameBtn.GetComponent<Button>().onClick.AddListener(() => {
            if (loading) return;
            loading = true;
            Debug.Log("Memulai Game Baru dari Awal");
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.NewGame();
            }
            StartCoroutine(LoadSceneWithFade(nextSceneName));
        });

        // 5. Buat Tombol Keluar (Quit)
        GameObject quitBtn = CreateButton("QuitBtn", "Keluar", mainMenuPanel.transform, new Vector2(0, -80), fontToUse);
        quitBtn.GetComponent<Button>().onClick.AddListener(() => {
            Debug.Log("Keluar dari Game");
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        });
        
        // Animasi fade in panel main menu
        CanvasGroup cg = mainMenuPanel.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        StartCoroutine(FadeInCanvasGroup(cg, 0.4f));
    }

    private IEnumerator FadeInCanvasGroup(CanvasGroup cg, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, timer / duration);
            yield return null;
        }
        cg.alpha = 1f;
    }

    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            if (fadePanel != null)
                fadePanel.alpha = t;
            yield return null;
        }
        SceneManager.LoadScene(sceneName);
    }

    private GameObject CreateButton(string name, string textContent, Transform parent, Vector2 anchoredPos, Font btnFont)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);
        
        Image btnImage = buttonObj.AddComponent<Image>();
        btnImage.color = new Color(0.15f, 0.15f, 0.15f, 0.9f); // Dark industrial style color
        
        Button btn = buttonObj.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor = new Color(0.15f, 0.15f, 0.15f, 0.9f);
        cb.highlightedColor = new Color(0.28f, 0.28f, 0.28f, 0.95f);
        cb.pressedColor = new Color(0.1f, 0.1f, 0.1f, 1f);
        btn.colors = cb;

        RectTransform btnRect = buttonObj.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.5f);
        btnRect.anchorMax = new Vector2(0.5f, 0.5f);
        btnRect.sizeDelta = new Vector2(280, 55); 
        btnRect.anchoredPosition = anchoredPos;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        Text btnText = textObj.AddComponent<Text>();
        btnText.text = textContent;
        btnText.font = btnFont;
        btnText.fontSize = 28;
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.color = Color.white;
        
        RectTransform txtRect = textObj.GetComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.offsetMin = Vector2.zero;
        txtRect.offsetMax = Vector2.zero;

        // Tambahkan efek outline yang cantik
        Outline outline = textObj.AddComponent<Outline>();
        outline.effectColor = new Color(0, 0, 0, 0.6f);
        outline.effectDistance = new Vector2(1, -1);

        return buttonObj;
    }
}