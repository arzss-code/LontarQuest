using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartMenuManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject anyKeyText;
    public CanvasGroup fadePanel;

    [Header("Scene")]
    public string nextSceneName = "SampleScene";

    private bool canPress = false;
    private bool loading = false;

    IEnumerator Start()
    {
        // Sembunyikan text saat awal
        anyKeyText.SetActive(false);

        // Tunggu animasi logo selesai
        yield return new WaitForSeconds(0.8f);

        // Munculkan text
        anyKeyText.SetActive(true);

        // Sekarang player boleh menekan tombol
        canPress = true;
    }

    void Update()
    {
        if (!canPress || loading)
            return;

        if (Input.anyKeyDown)
        {
            Debug.Log("Any Key Ditekan");

            loading = true;
            StartCoroutine(LoadNextScene());
        }
    }

    IEnumerator LoadNextScene()
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;

            if (fadePanel != null)
                fadePanel.alpha = t;

            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}