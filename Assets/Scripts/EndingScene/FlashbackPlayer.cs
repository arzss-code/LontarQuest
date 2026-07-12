using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class FlashbackPlayer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject flashbackCanvas;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private CanvasGroup whiteFlash;

    [Header("Settings")]
    [SerializeField] private float flashDuration = 0.4f;

    public bool IsPlaying { get; private set; }

    //----------------------------------------------------

    private void Awake()
    {
        flashbackCanvas.SetActive(false);
        whiteFlash.alpha = 0;
    }

    //----------------------------------------------------

    public IEnumerator PlayFlashback()
    {
        IsPlaying = true;

        //----------------------------------
        // White Fade In
        //----------------------------------

        yield return FadeWhite(0, 1);

        //----------------------------------
        // Tampilkan Video
        //----------------------------------

        flashbackCanvas.SetActive(true);

        videoPlayer.Stop();

        videoPlayer.time = 0;

        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
            yield return null;

        videoPlayer.Play();

        //----------------------------------
        // White Fade Out
        //----------------------------------

        yield return FadeWhite(1, 0);

        //----------------------------------
        // Tunggu Video Selesai
        //----------------------------------

        while (videoPlayer.isPlaying)
            yield return null;

        //----------------------------------
        // White Fade In Lagi
        //----------------------------------

        yield return FadeWhite(0, 1);

        flashbackCanvas.SetActive(false);

        //----------------------------------
        // White Fade Out
        //----------------------------------

        yield return FadeWhite(1, 0);

        IsPlaying = false;
    }


    public IEnumerator PlayVideo(VideoClip clip)
    {
        IsPlaying = true;

        //----------------------------------
        // White Fade In
        //----------------------------------

        yield return FadeWhite(0, 1);

        //----------------------------------
        // Canvas ON
        //----------------------------------

        flashbackCanvas.SetActive(true);

        //----------------------------------
        // Ganti Video
        //----------------------------------

        videoPlayer.Stop();

        videoPlayer.clip = clip;

        videoPlayer.time = 0;

        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
            yield return null;

        //----------------------------------
        // Play
        //----------------------------------

        videoPlayer.Play();

        //----------------------------------
        // Fade Out White
        //----------------------------------

        yield return FadeWhite(1, 0);

        //----------------------------------
        // Tunggu Video
        //----------------------------------

        float duration = Mathf.Min(
            3f,
            (float)videoPlayer.length);

        yield return new WaitForSeconds(duration);

        videoPlayer.Stop();

        //----------------------------------
        // White Fade In
        //----------------------------------

        yield return FadeWhite(0, 1);

        flashbackCanvas.SetActive(false);

        //----------------------------------
        // Fade Out White
        //----------------------------------

        yield return FadeWhite(1, 0);

        IsPlaying = false;
    }

    //----------------------------------------------------
    // Play Video Tanpa Menunggu Selesai
    //----------------------------------------------------

    public void PlayVideoAsync(VideoClip clip)
    {
        StartCoroutine(PlayVideoAsyncRoutine(clip));
    }

    private IEnumerator PlayVideoAsyncRoutine(VideoClip clip)
    {
        IsPlaying = true;

        //----------------------------------
        // Fade White In
        //----------------------------------

        yield return FadeWhite(0, 1);

        //----------------------------------
        // Canvas ON
        //----------------------------------

        flashbackCanvas.SetActive(true);

        //----------------------------------
        // Ganti Clip
        //----------------------------------

        videoPlayer.Stop();

        videoPlayer.clip = clip;

        videoPlayer.time = 0;

        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
            yield return null;

        //----------------------------------
        // Play
        //----------------------------------

        videoPlayer.Play();

        //----------------------------------
        // Fade White Out
        //----------------------------------

        yield return FadeWhite(1, 0);

        //----------------------------------
        // Video jalan sendiri
        //----------------------------------

        float duration = Mathf.Min(
            3f,
            (float)videoPlayer.length);

        yield return new WaitForSeconds(duration);

        videoPlayer.Stop();

        //----------------------------------
        // Fade White In
        //----------------------------------

        yield return FadeWhite(0, 1);

        flashbackCanvas.SetActive(false);

        //----------------------------------
        // Fade White Out
        //----------------------------------

        yield return FadeWhite(1, 0);

        IsPlaying = false;
    }


    //----------------------------------------------------

    IEnumerator FadeWhite(float from, float to)
    {
        float timer = 0;

        while (timer < flashDuration)
        {
            timer += Time.deltaTime;

            whiteFlash.alpha =
                Mathf.Lerp(
                    from,
                    to,
                    timer / flashDuration);

            yield return null;
        }

        whiteFlash.alpha = to;
    }
}