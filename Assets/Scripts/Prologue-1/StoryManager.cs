using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.Playables;

public class StoryManager : MonoBehaviour
{
    [SerializeField] private string nextScene;

    [SerializeField] private float fadeDuration = 1.0f;
    
    [Header("Auto Transition Settings (Opsional)")]
    [Tooltip("Waktu tunggu otomatis (detik) sebelum pindah scene. Isi 0 jika cutscene menggunakan VideoPlayer atau Timeline.")]
    [SerializeField] private float autoSkipDelay = 0f;

    private bool isFading = false;
    private VideoPlayer videoPlayer;
    private PlayableDirector director;

    void Start()
    {
        // 1. Coba deteksi jika menggunakan Video Player di scene ini
        videoPlayer = Object.FindAnyObjectByType<VideoPlayer>();
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += (vp) => SkipCutscene();
        }

        // 2. Coba deteksi jika menggunakan Timeline / PlayableDirector di scene ini
        director = Object.FindAnyObjectByType<PlayableDirector>();
        if (director != null)
        {
            director.stopped += (d) => SkipCutscene();
        }

        // 3. Gunakan timer jika diisi
        if (autoSkipDelay > 0f)
        {
            Invoke(nameof(SkipCutscene), autoSkipDelay);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SkipCutscene();
        }
    }

    public void SkipCutscene()
    {
        if (!isFading)
        {
            StartCoroutine(FadeAndLoadScene());
        }
    }

    private System.Collections.IEnumerator FadeAndLoadScene()
    {
        isFading = true;

        // 1. Buat Canvas dan Layar Hitam (Fade Out) secara dinamis
        GameObject canvasObj = new GameObject("FadeCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // Pastikan layar hitam berada paling depan
        
        GameObject imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(canvasObj.transform, false);
        UnityEngine.UI.Image fadeImage = imageObj.AddComponent<UnityEngine.UI.Image>();
        fadeImage.color = new Color(0, 0, 0, 0); // Mulai dari transparan
        
        RectTransform rect = fadeImage.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // 2. Lakukan proses Fade In ke warna Hitam
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // 3. Pindah Scene
        if (!string.IsNullOrEmpty(nextScene))
        {
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.LogWarning("Next Scene belum diisi di Inspector!");
        }
    }
}