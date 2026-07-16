using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Camera))]
public class ForceAspectRatio : MonoBehaviour
{
    private const float TargetAspect = 16f / 9f;
    private Camera mainCamera;
    private Camera backgroundCamera;

    // Mendaftarkan event listener sebelum scene pertama dimuat
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeGlobalListener()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AttachToMainCamera();
    }

    private static void AttachToMainCamera()
    {
        // Cari kamera utama di scene yang baru dimuat
        Camera targetCam = Camera.main;
        if (targetCam == null)
        {
            targetCam = FindFirstObjectByType<Camera>();
        }

        if (targetCam != null)
        {
            // Hindari menempelkan script ke kamera background buatan sendiri
            if (targetCam.gameObject.name == "BackgroundCamera")
                return;

            if (targetCam.GetComponent<ForceAspectRatio>() == null)
            {
                targetCam.gameObject.AddComponent<ForceAspectRatio>();
                Debug.Log($"[ForceAspectRatio] Otomatis memasang ForceAspectRatio pada {targetCam.gameObject.name} di scene {SceneManager.GetActiveScene().name}");
            }
        }
    }

    void Awake()
    {
        // Paksa resolusi game ke 1920x1080 Full HD saat inisialisasi awal
        if (Screen.width != 1920 || Screen.height != 1080)
        {
            Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
            Debug.Log($"[ForceAspectRatio] Resolusi layar dipaksa ke 1920x1080. Resolusi sebelumnya: {Screen.width}x{Screen.height}");
        }

        mainCamera = GetComponent<Camera>();
        CreateBackgroundCamera();
    }

    void Start()
    {
        AdjustResolution();
    }

    void Update()
    {
        AdjustResolution();
    }

    private void CreateBackgroundCamera()
    {
        // Membuat kamera latar belakang hitam agar area letterbox/pillarbox bersih
        GameObject bgCamObj = new GameObject("BackgroundCamera");
        bgCamObj.transform.SetParent(transform);
        
        backgroundCamera = bgCamObj.AddComponent<Camera>();
        backgroundCamera.depth = mainCamera.depth - 1; // Render di belakang Main Camera
        backgroundCamera.clearFlags = CameraClearFlags.SolidColor;
        backgroundCamera.backgroundColor = Color.black;
        backgroundCamera.cullingMask = 0; // Tidak me-render layer objek apapun
        backgroundCamera.useOcclusionCulling = false;
        backgroundCamera.allowHDR = false;
        backgroundCamera.allowMSAA = false;
    }

    private void AdjustResolution()
    {
        float currentAspect = (float)Screen.width / Screen.height;
        float scaleHeight = currentAspect / TargetAspect;

        if (scaleHeight < 1.0f)
        {
            // Layar lebih tinggi dari 16:9 (misalnya 16:10, 4:3) -> Letterbox (hitam di atas/bawah)
            Rect rect = mainCamera.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            mainCamera.rect = rect;
        }
        else
        {
            // Layar lebih lebar dari 16:9 (misalnya 21:9 ultra-wide) -> Pillarbox (hitam di kiri/kanan)
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = mainCamera.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            mainCamera.rect = rect;
        }
    }
}
