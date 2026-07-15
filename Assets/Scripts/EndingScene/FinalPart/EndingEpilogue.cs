using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndingEpilogue : MonoBehaviour
{
    [Header("Illustration")]

    [SerializeField] private CanvasGroup illustrationPanel;

    [SerializeField] private Image illustrationImage;

    [SerializeField] private TMP_Text nameText;

    [SerializeField] private TMP_Text dialogueText;

    [SerializeField] private Sprite illustration01;

    [SerializeField] private Sprite illustration02;

    [Header("Ending")]

    
    [SerializeField] private TMP_Text narrationText;

    [SerializeField] private TMP_Text endText;

    [Header("Settings")]

    [SerializeField] private float typingSpeed = 0.03f;

    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Gameplay")]

    [SerializeField] private PlayerController player;

    [SerializeField] private PortalController portal;

    [Header("Transition")]

    [SerializeField] private CanvasGroup whiteFlash;
    [SerializeField] private Image blackPanel;
    [SerializeField] private float narrationStayTime = 3f;

    [Header("Audio")]

    [SerializeField] private AudioSource bgmSource;

    [SerializeField] private AudioClip endingPianoBGM;

    [SerializeField] private float bgmFadeDuration = 2f;

    [SerializeField] private float bgmVolume = 0.15f;

    [Header("Scene")]

    [SerializeField] private string mainMenuScene = "StartMenu";

    [SerializeField] private float endingDelay = 3f;

    [Header("Credits Settings")]
    [SerializeField] private float creditsStayTime = 2.5f;
    [SerializeField] private string[] creditsCards = {
        "LONTAR QUEST",
        "Sebuah Game Oleh\n\nTRISTAN\nFADHIKA\nWARSENO\nARYA",
        "--- TIM PENGEMBANG ---\n\nStory Designer\nTristan\n\nGame Designer & Level Designer\nArya\nWarseno\n\nAssets Sprites Generator\nFadhika",
        "--- ASET PIHAK KETIGA ---\n\n2D Art & Environment\nRafaelMatos\n\nVisual Effects & Audio\nJMO Assets, Travis Game Assets",
        "TERIMA KASIH TELAH BERMAIN"
    };

    [Header("Post Credits Settings")]
    [SerializeField] private Sprite postCreditIllustration;
    [SerializeField] private float postCreditDelay = 2.0f;
    [SerializeField] private string postCreditNarrationStart = "Beberapa bulan setelah hilangnya Saka...";
    [SerializeField] private string postCreditTextEnd = "Kisah Saka akan berlanjut.";
 
    private bool nextDialogue;

    private bool isTyping;
    private bool skipTyping;

    private readonly string[] endingNarrations =
    {
        "Saka berakhir di dalam Dimensi Lontar.\n\nMenjadi inti yang menjaga keseimbangan dunia tersebut.",

        "Dirinya kini abadi.\n\nMenunggu hari ketika seseorang datang menggantikannya.",

        "Entah bertahun-tahun.\n\nAtaupun berabad-abad.",

        "Namun...",

        "Saka tidak pernah menyesali keputusannya."
    };

//===========================================================//

    private void Awake()
    {
        illustrationPanel.gameObject.SetActive(false);

        blackPanel.gameObject.SetActive(false);

        narrationText.gameObject.SetActive(false);

        endText.gameObject.SetActive(false);

        if (postCreditIllustration == null)
        {
            postCreditIllustration = Resources.Load<Sprite>("EndingScene/PostCreditIllustration");
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(isTyping)
            {
                skipTyping = true;
            }
            else
            {
                nextDialogue = true;
            }
        }
    }

    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";

        isTyping = true;
        skipTyping = false;

        foreach(char c in text)
        {
            if(skipTyping)
            {
                dialogueText.text = text;
                break;
            }

            dialogueText.text += c;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    
    private IEnumerator PlayIllustration01()
    {

        //----------------------------------
        // Panel Aktif
        //----------------------------------

        illustrationPanel.gameObject.SetActive(true);

        illustrationImage.sprite = illustration01;

        illustrationPanel.alpha = 1;

        //----------------------------------
        // White masih penuh
        //----------------------------------

        whiteFlash.alpha = 1;

        //----------------------------------
        // Fade White Keluar
        //----------------------------------

        float timer = 0;

        while(timer < fadeDuration)
        {
            timer += Time.deltaTime;

            whiteFlash.alpha =
                Mathf.Lerp(
                    1,
                    0,
                    timer / fadeDuration);

            yield return null;
        }

        whiteFlash.alpha = 0;

        //----------------------------------
        // Jeda
        //----------------------------------

        yield return new WaitForSeconds(0.4f);

        //----------------------------------
        // Dialog
        //----------------------------------

        nameText.text = "Prof. Arya";

        yield return StartCoroutine(
            TypeText("Saka..."));

        nextDialogue = false;

        yield return new WaitUntil(() => nextDialogue);

        yield return StartCoroutine(
            TypeText("Apa yang kau lakukan...?"));

        nextDialogue = false;

        yield return new WaitUntil(() => nextDialogue);

        //----------------------------------
        // Cross Fade ke Illustration 02
        //----------------------------------

        yield return StartCoroutine(
            CrossFadeIllustration(
                illustration02));

        
    }

    private IEnumerator PlayIllustration02()
    {
        //----------------------------------
        // Jeda
        //----------------------------------

        yield return new WaitForSeconds(0.3f);

        //----------------------------------
        // Nama
        //----------------------------------

        nameText.text = "Saka";

        //----------------------------------
        // Line 1
        //----------------------------------

        yield return StartCoroutine(
            TypeText("Maafkan aku... Ayah..."));

        nextDialogue = false;

        yield return new WaitUntil(() => nextDialogue);

        //----------------------------------
        // Line 2
        //----------------------------------

        yield return StartCoroutine(
            TypeText("Terima kasih..."));

        nextDialogue = false;

        yield return new WaitUntil(() => nextDialogue);

        //----------------------------------
        // Line 3
        //----------------------------------

        yield return StartCoroutine(
            TypeText("Untuk semuanya."));

        nextDialogue = false;

        yield return new WaitUntil(() => nextDialogue);
    }

    private IEnumerator PlayEndingNarration()
    {
        //----------------------------------
        // Tutup Illustration
        //----------------------------------

        yield return StartCoroutine(
            FadeCanvas(
                illustrationPanel,
                1,
                0));

        illustrationPanel.gameObject.SetActive(false);

        //----------------------------------
        // Black Screen
        //----------------------------------

        blackPanel.gameObject.SetActive(true);

        Color black = blackPanel.color;
        black.a = 0;
        blackPanel.color = black;

        yield return StartCoroutine(
            FadeBlack(
                0,
                1));

        //----------------------------------
        // Narration
        //----------------------------------

        narrationText.gameObject.SetActive(true);

        foreach(string narration in endingNarrations)
        {
            narrationText.text = narration;

            Color color = narrationText.color;
            color.a = 0;
            narrationText.color = color;

            yield return StartCoroutine(
                FadeNarration(
                    0,
                    1));

            yield return new WaitForSeconds(
                narrationStayTime);

            yield return StartCoroutine(
                FadeNarration(
                    1,
                    0));

            yield return new WaitForSeconds(0.8f);
        }

        narrationText.gameObject.SetActive(false);
    }

    private IEnumerator PlayTheEnd()
    {
        //----------------------------------
        // Text ON
        //----------------------------------

        endText.gameObject.SetActive(true);

        //----------------------------------
        // Pertama
        //----------------------------------

        endText.text =
    @"Perjalanan Saka
    Berakhir Di Sini";

        Color color = endText.color;

        color.a = 0;

        endText.color = color;

        //----------------------------------
        // Fade In
        //----------------------------------

        float timer = 0;

        while(timer < fadeDuration)
        {
            timer += Time.deltaTime;

            color.a =
                Mathf.Lerp(
                    0,
                    1,
                    timer / fadeDuration);

            endText.color = color;

            yield return null;
        }

        //----------------------------------
        // Diam
        //----------------------------------

        yield return new WaitForSeconds(endingDelay);

        //----------------------------------
        // Fade Out
        //----------------------------------

        timer = 0;

        while(timer < fadeDuration)
        {
            timer += Time.deltaTime;

            color.a =
                Mathf.Lerp(
                    1,
                    0,
                    timer / fadeDuration);

            endText.color = color;

            yield return null;
        }

        //----------------------------------
        // Ganti Text
        //----------------------------------

        endText.text = "- THE END -";

        //----------------------------------
        // Fade In Lagi
        //----------------------------------

        timer = 0;

        while(timer < fadeDuration)
        {
            timer += Time.deltaTime;

            color.a =
                Mathf.Lerp(
                    0,
                    1,
                    timer / fadeDuration);

            endText.color = color;

            yield return null;
        }

        //----------------------------------
        // Diam
        //----------------------------------

        yield return new WaitForSeconds(5f);

        //----------------------------------
        // Fade Out - THE END -
        //----------------------------------

        timer = 0;

        while(timer < fadeDuration)
        {
            timer += Time.deltaTime;

            color.a =
                Mathf.Lerp(
                    1,
                    0,
                    timer / fadeDuration);

            endText.color = color;

            yield return null;
        }

        color.a = 0;
        endText.color = color;
        endText.gameObject.SetActive(false);
    }

    private IEnumerator PlayCredits()
    {
        // Pastikan blackPanel aktif dan menutupi layar
        blackPanel.gameObject.SetActive(true);
        Color black = blackPanel.color;
        black.a = 1;
        blackPanel.color = black;

        // Gunakan narrationText untuk menampilkan slide kredit
        narrationText.gameObject.SetActive(true);

        foreach (string creditCard in creditsCards)
        {
            narrationText.text = creditCard;

            // Fade In
            Color color = narrationText.color;
            color.a = 0;
            narrationText.color = color;

            float timer = 0;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                color.a = Mathf.Lerp(0, 1, timer / fadeDuration);
                narrationText.color = color;
                yield return null;
            }
            color.a = 1;
            narrationText.color = color;

            // Diam
            yield return new WaitForSeconds(creditsStayTime);

            // Fade Out
            timer = 0;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                color.a = Mathf.Lerp(1, 0, timer / fadeDuration);
                narrationText.color = color;
                yield return null;
            }
            color.a = 0;
            narrationText.color = color;
            narrationText.text = "";

            yield return new WaitForSeconds(0.5f);
        }

        narrationText.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.5f);
    }

    private IEnumerator PlayPostCreditScene()
    {
        // 1. Tampilkan Narrasi Awal ("Beberapa bulan setelah hilangnya Saka...")
        narrationText.gameObject.SetActive(true);
        narrationText.text = postCreditNarrationStart;
        Color narrColor = narrationText.color;
        narrColor.a = 0;
        narrationText.color = narrColor;

        // Fade In Narasi
        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            narrColor.a = Mathf.Lerp(0, 1, timer / fadeDuration);
            narrationText.color = narrColor;
            yield return null;
        }
        narrColor.a = 1;
        narrationText.color = narrColor;

        yield return new WaitForSeconds(3.0f);

        // Fade Out Narasi
        timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            narrColor.a = Mathf.Lerp(1, 0, timer / fadeDuration);
            narrationText.color = narrColor;
            yield return null;
        }
        narrColor.a = 0;
        narrationText.color = narrColor;
        narrationText.text = "";
        narrationText.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.0f);

        // 2. Tampilkan ilustrasi pasca-kredit
        if (postCreditIllustration != null)
        {
            illustrationImage.sprite = postCreditIllustration;
        }
        illustrationPanel.gameObject.SetActive(true);
        illustrationPanel.alpha = 0;

        // Fade In Ilustrasi
        timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            illustrationPanel.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            yield return null;
        }
        illustrationPanel.alpha = 1;

        yield return new WaitForSeconds(0.5f);

        // 3. Jalankan dialog pasca-kredit
        nameText.text = "Prof. Arya";
        yield return StartCoroutine(TypeText("Saka... portal itu memang sudah tertutup."));
        nextDialogue = false;
        yield return new WaitUntil(() => nextDialogue);

        yield return StartCoroutine(TypeText("Tapi detak energi Dimensi Lontar masih terekam di sini."));
        nextDialogue = false;
        yield return new WaitUntil(() => nextDialogue);

        yield return StartCoroutine(TypeText("Ayah tidak akan membiarkanmu terjebak di sana selamanya."));
        nextDialogue = false;
        yield return new WaitUntil(() => nextDialogue);

        yield return StartCoroutine(TypeText("Pasti ada cara untuk membuka kembali jalan masuk..."));
        nextDialogue = false;
        yield return new WaitUntil(() => nextDialogue);

        nameText.text = "???";
        yield return StartCoroutine(TypeText("...Ayah..."));
        nextDialogue = false;
        yield return new WaitUntil(() => nextDialogue);

        nameText.text = "Prof. Arya";
        yield return StartCoroutine(TypeText("(terkejut) Saka?! Apakah itu suaramu?!"));
        nextDialogue = false;
        yield return new WaitUntil(() => nextDialogue);

        // 4. Fade Out Ilustrasi & Dialog
        timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            illustrationPanel.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            yield return null;
        }
        illustrationPanel.alpha = 0;
        illustrationPanel.gameObject.SetActive(false);

        // Pastikan layar tetap hitam
        blackPanel.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        // 5. Tampilkan teks penutup ("Kisah Saka akan berlanjut.")
        endText.gameObject.SetActive(true);
        endText.text = postCreditTextEnd;
        Color endTextColor = endText.color;
        endTextColor.a = 0;
        endText.color = endTextColor;

        // Fade In
        timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            endTextColor.a = Mathf.Lerp(0, 1, timer / fadeDuration);
            endText.color = endTextColor;
            yield return null;
        }
        endTextColor.a = 1;
        endText.color = endTextColor;

        yield return new WaitForSeconds(4.0f);

        // Fade Out
        timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            endTextColor.a = Mathf.Lerp(1, 0, timer / fadeDuration);
            endText.color = endTextColor;
            yield return null;
        }
        endTextColor.a = 0;
        endText.color = endTextColor;
        endText.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.0f);
    }

    public IEnumerator Play()
    {
        //----------------------------------
        // Freeze Player
        //----------------------------------

        if(player != null)
            player.SetCanMove(false);

        //----------------------------------
        // Stop Portal
        //----------------------------------

        if(portal != null)
            portal.StopPortalSound();

        //----------------------------------
        // Ganti BGM
        //----------------------------------

        yield return StartCoroutine(
            ChangeEndingBGM());

        //----------------------------------
        // Illustration
        //----------------------------------

        yield return StartCoroutine(
            PlayIllustration01());

        yield return StartCoroutine(
            PlayIllustration02());

        yield return StartCoroutine(
            PlayEndingNarration());

        yield return StartCoroutine(
            PlayTheEnd());

        //----------------------------------
        // Credits Roll
        //----------------------------------

        yield return StartCoroutine(
            PlayCredits());

        //----------------------------------
        // Post Credits Scene
        //----------------------------------

        yield return StartCoroutine(
            PlayPostCreditScene());

        //----------------------------------
        // Fade Out Music
        //----------------------------------

        yield return StartCoroutine(
            FadeOutBGM());

        //----------------------------------
        // Kembali ke Menu
        //----------------------------------

        SceneManager.LoadScene(mainMenuScene);
    }



    private IEnumerator FadeCanvas(CanvasGroup group, float from, float to)
    {
        float timer = 0;

        while(timer < fadeDuration)
        {
            timer += Time.deltaTime;

            group.alpha =
                Mathf.Lerp(
                    from,
                    to,
                    timer / fadeDuration);

            yield return null;
        }

        group.alpha = to;
    }

    private IEnumerator ChangeEndingBGM()
    {
        if (bgmSource == null)
            yield break;

        if (endingPianoBGM == null)
            yield break;

        //----------------------------------
        // Fade Out
        //----------------------------------

        float timer = 0;

        float startVolume = bgmSource.volume;

        while (timer < bgmFadeDuration)
        {
            timer += Time.deltaTime;

            bgmSource.volume =
                Mathf.Lerp(
                    startVolume,
                    0,
                    timer / bgmFadeDuration);

            yield return null;
        }

        //----------------------------------
        // Ganti Lagu
        //----------------------------------

        bgmSource.Stop();

        bgmSource.clip = endingPianoBGM;

        bgmSource.loop = true;

        bgmSource.Play();

        //----------------------------------
        // Fade In
        //----------------------------------

        timer = 0;

        while (timer < bgmFadeDuration)
        {
            timer += Time.deltaTime;

            bgmSource.volume =
                Mathf.Lerp(
                    0,
                    bgmVolume,
                    timer / bgmFadeDuration);

            yield return null;
        }

        bgmSource.volume = bgmVolume;
    }

    private IEnumerator CrossFadeIllustration(Sprite nextSprite)
    {
        float duration = 0.4f;

        float timer = 0;

        //----------------------------------
        // Fade Out
        //----------------------------------

        while (timer < duration)
        {
            timer += Time.deltaTime;

            illustrationPanel.alpha =
                Mathf.Lerp(
                    1,
                    0,
                    timer / duration);

            yield return null;
        }

        //----------------------------------
        // Ganti Sprite
        //----------------------------------

        illustrationImage.sprite = nextSprite;

        //----------------------------------
        // Fade In
        //----------------------------------

        timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            illustrationPanel.alpha =
                Mathf.Lerp(
                    0,
                    1,
                    timer / duration);

            yield return null;
        }

        illustrationPanel.alpha = 1;
    }

    private IEnumerator FadeBlack(float from,float to)
    {
        Color color = blackPanel.color;

        float timer = 0;

        while(timer < fadeDuration)
        {
            timer += Time.deltaTime;

            color.a =
                Mathf.Lerp(
                    from,
                    to,
                    timer/fadeDuration);

            blackPanel.color = color;

            yield return null;
        }

        color.a = to;

        blackPanel.color = color;
    }

    private IEnumerator FadeNarration(float from,float to)
    {
        Color color = narrationText.color;

        float timer = 0;

        while(timer < fadeDuration)
        {
            timer += Time.deltaTime;

            color.a =
                Mathf.Lerp(
                    from,
                    to,
                    timer/fadeDuration);

            narrationText.color = color;

            yield return null;
        }

        color.a = to;

        narrationText.color = color;
    }

    private IEnumerator FadeOutBGM()
    {
        if (bgmSource == null)
            yield break;

        float startVolume = bgmSource.volume;

        float timer = 0;

        while (timer < bgmFadeDuration)
        {
            timer += Time.deltaTime;

            bgmSource.volume =
                Mathf.Lerp(
                    startVolume,
                    0,
                    timer / bgmFadeDuration);

            yield return null;
        }

        bgmSource.Stop();

        bgmSource.volume = bgmVolume;
    }
}