using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class EndingPortalSequence : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerController player;

    [SerializeField] private PlayerCutsceneController playerCutscene;

    [SerializeField] private Transform playerToFather;
    [SerializeField] private Transform playerToPortal;

    [Header("Arya")]
    [SerializeField] private AryaCutsceneController arya;

    [Header("Dialogue")]
    [SerializeField] private IntroDialogue dialogueK01;
    [SerializeField] private IntroDialogue dialogueK02;

    [Header("Truth Reveal")]

    [SerializeField] private Transform sakaNearArya;

    [SerializeField] private Transform aryaExplain;

    [SerializeField] private IntroDialogue dialogueK03;

    [SerializeField] private IntroDialogue dialogueK04;

    [SerializeField] private IntroDialogue dialogueK05;

    [Header("Dialogue L")]

    [SerializeField] private IntroDialogue dialogueL;

    [SerializeField] private Transform sakaStep01;

    [SerializeField] private Transform sakaStep02;
    
    [Header("Dialogue M01")]

    [SerializeField] private IntroDialogue dialogueM01;

    [SerializeField] private Transform sakaRefused;

    [SerializeField] private Transform ayahCalmSaka;

    [SerializeField] private Transform ayahGoNearPortal;

    [Header("Final Decision")]

    [SerializeField] private IntroDialogue dialogueM02;

    [SerializeField] private Transform sakaClosePoint;

    [SerializeField] private CinemachineCamera cmWide2;

    [Header("Push Sequence")]

    [SerializeField] private Transform aryaPushPoint;

    [SerializeField] private Transform aryaPortalCenter;
    [SerializeField] private AudioClip pushSFX;

    [Header("Ending")]
    [SerializeField] private EndingEpilogue endingEpilogue;


    [Header("Portal")]
    [SerializeField] private PortalController portal;

    [SerializeField] private CanvasGroup whiteFlash;
    [SerializeField] private float flashDuration = 0.8f;
    [SerializeField] private CinemachineShake cameraShake;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip thunderSFX;

    [Header("Reality Shock Audio")]
    [SerializeField] private AudioClip earthquakeSFX;

    [Header("Camera")]

    [SerializeField] private CinemachineCamera cmPlayer;

    [SerializeField] private CinemachineCamera cmArya;
    [SerializeField] private CinemachineCamera cmWide;

    [Header("Portal Approach")]
    [SerializeField] private Transform sakaNearPortal;

    [SerializeField] private Transform aryaNearPortal;

    private bool k05EventRegistered = false;
    

    //--------------------------------------------------

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
    public void Play()
    {
        StartCoroutine(SequenceRoutine());
    }

    //--------------------------------------------------

    IEnumerator SequenceRoutine()
    {
        //----------------------------------
        // Freeze Player
        //----------------------------------

        player.SetCanMove(false);

        //----------------------------------
        // Jalan ke Ayah
        //----------------------------------

        yield return StartCoroutine(
            playerCutscene.MoveTo(playerToFather));

        //----------------------------------
        // Menghadap Ayah
        //----------------------------------

        playerCutscene.Face(Vector2.up);

        //----------------------------------
        // Dialogue
        //----------------------------------

        dialogueK01.StartDialogue();

        yield return new WaitUntil(() =>
            dialogueK01.HasFinished());

        //----------------------------------
        // Camera Shake
        //----------------------------------

        StartCoroutine(
            cameraShake.Shake(
                0.8f,
                2.0f,
                2.5f));

        // Thunder
        PlayThunder();


        //--------------------
        // White Flash
        //--------------------

        yield return StartCoroutine(
            FlashWhite());

        portal.Spawn();

        yield return new WaitForSeconds(0.1f);

        yield return StartCoroutine(
            FlashBack());

        //----------------------------------
        // Jalan mendekati Portal
        //----------------------------------

        Coroutine sakaMove =
            StartCoroutine(
                playerCutscene.MoveTo(sakaNearPortal));

        Coroutine aryaMove =
            StartCoroutine(
                arya.MoveTo(aryaNearPortal));

        yield return sakaMove;
        yield return aryaMove;

        playerCutscene.Face(Vector2.up);

        arya.Face(Vector2.up);


        //--------------------
        // Dialogue K02
        //--------------------

        dialogueK02.StartDialogue();

        yield return new WaitUntil(() =>
            dialogueK02.HasFinished());

        //----------------------------------
        // Saka berjalan mendekati Portal
        //----------------------------------

        yield return StartCoroutine(
            playerCutscene.MoveTo(playerToPortal));

        // Menghadap Portal
        playerCutscene.Face(Vector2.up);

        // Beri jeda sedikit
        yield return new WaitForSeconds(0.4f);

        //----------------------------------
        // Dialogue K03
        //----------------------------------

        yield return StartCoroutine(
            PlayTruthReveal());

        

        Debug.Log("Sequence A Finish");
    }


    private IEnumerator PlayTruthReveal()
    {
        //----------------------------------
        // Saka menghadap Ayah
        //----------------------------------

        playerCutscene.Face(Vector2.down);

        //----------------------------------
        // Dialogue K03
        //----------------------------------

        dialogueK03.StartDialogue();

        yield return new WaitUntil(() =>
            dialogueK03.HasFinished());

        //----------------------------------
        // Saka mendekat ke Ayah
        //----------------------------------

        yield return StartCoroutine(
            playerCutscene.MoveTo(sakaNearArya));

        playerCutscene.Face(Vector2.down);

        //----------------------------------
        // Dialogue K04
        //----------------------------------

        dialogueK04.StartDialogue();

        yield return new WaitUntil(() =>
            dialogueK04.HasFinished());

        //----------------------------------
        // Camera Focus Arya
        //----------------------------------

        cmPlayer.Priority = 10;
        cmArya.Priority = 20;

        yield return new WaitForSeconds(0.5f);

        //----------------------------------
        // Arya maju sedikit
        //----------------------------------

        yield return StartCoroutine(
            arya.MoveTo(aryaExplain));

        //----------------------------------
        // Arya menghadap Saka dulu
        //----------------------------------

        arya.Face(Vector2.up);

        //----------------------------------
        // Register Event K05
        //----------------------------------

        dialogueK05.OnLineChanged += HandleDialogueK05;

        //----------------------------------
        // Dialogue K05
        //----------------------------------

        dialogueK05.StartDialogue();

        yield return new WaitUntil(() =>
            dialogueK05.HasFinished());

        yield return StartCoroutine(
            PlayDenialSequence());

        //----------------------------------
        // Unregister Event
        //----------------------------------

        dialogueK05.OnLineChanged -= HandleDialogueK05;
    }

    private IEnumerator PlayDenialSequence()
    {
        //----------------------------------
        // Kamera kembali ke Player
        //----------------------------------

        cmArya.Priority = 10;
        cmPlayer.Priority = 20;

        yield return new WaitForSeconds(0.8f);

        //----------------------------------
        // Register Event
        //----------------------------------

        dialogueL.OnLineChanged += HandleDialogueL;

        //----------------------------------
        // Dialogue L
        //----------------------------------

        dialogueL.StartDialogue();

        yield return new WaitUntil(() =>
            dialogueL.HasFinished());

        yield return StartCoroutine(
            PlayAcceptanceSequence());

        //----------------------------------
        // Unregister
        //----------------------------------

        dialogueL.OnLineChanged -= HandleDialogueL;
    }

    private IEnumerator PlayAcceptanceSequence()
    {
        //----------------------------------
        // Register Event
        //----------------------------------

        dialogueM01.OnLineChanged += HandleDialogueM01;

        //----------------------------------
        // Dialogue
        //----------------------------------

        dialogueM01.StartDialogue();

        yield return new WaitUntil(() =>
            dialogueM01.HasFinished());

        yield return StartCoroutine(
            PlayFinalDecision());

        //----------------------------------
        // Unregister
        //----------------------------------

        dialogueM01.OnLineChanged -= HandleDialogueM01;
    }

    private IEnumerator PlayFinalDecision()
    {
        dialogueM02.OnLineChanged += HandleDialogueM02;

        dialogueM02.StartDialogue();

        yield return new WaitUntil(() =>
            dialogueM02.HasFinished());

        dialogueM02.OnLineChanged -= HandleDialogueM02;

        //----------------------------------
        // Push Sequence
        //----------------------------------

        yield return StartCoroutine(
            PushSequence());
    }

    private IEnumerator PushSequence()
    {
        //----------------------------------
        // Diam sejenak
        //----------------------------------

        yield return new WaitForSeconds(0.5f);

        //----------------------------------
        // Saka Mendorong
        //----------------------------------

        yield return StartCoroutine(
            PushSaka());

        //----------------------------------
        // Impact
        //----------------------------------

        PlayPush();

        StartCoroutine(
            cameraShake.Shake(
                0.15f,
                0.8f,
                3f));

        //----------------------------------
        // Arya Terpental
        //----------------------------------

        yield return StartCoroutine(
            KnockbackArya());

        //----------------------------------
        // Arya Masuk Portal
        //----------------------------------

        yield return StartCoroutine(
            LaunchAryaToPortal());

        //----------------------------------
        // White Flash
        //----------------------------------

        yield return StartCoroutine(
            FlashWhite());
        portal.StopPortalSound();

        arya.gameObject.SetActive(false);

        yield return StartCoroutine(
            endingEpilogue.Play());
    }



    private void HandleDialogueM02(int line)
    {
        Debug.Log("Dialogue M02 : " + line);

        switch (line)
        {
            case 0:

                StartCoroutine(
                    MoveSakaClose());

                break;
            
            case 4:

                SetActiveCamera(cmWide2);

                break;

            
        }
    }

    private void HandleDialogueM01(int line)
    {
        Debug.Log("Dialogue M01 Line : " + line);

        switch (line)
        {
            case 0:

                cmPlayer.Priority = 10;
                cmArya.Priority = 20;

                break;

            case 6:

                StartCoroutine(
                    MoveSakaRefused());

                break;

            case 8:

                StartCoroutine(
                    MoveAryaCalm());

                break;

            case 11:

                StartCoroutine(
                    MoveAryaPortal());

                break;
        }
    }


    private void HandleDialogueK05(int line)
    {
        Debug.Log("Dialogue K05 Line : " + line);

        switch (line)
        {
            //----------------------------------
            // Line 0
            // Kamera mulai fokus ke Arya
            //----------------------------------

            case 0:

                cmPlayer.Priority = 10;
                cmArya.Priority = 20;

                break;

            //----------------------------------
            // Line 1
            // "Ayah di sini sudah seperti seorang tahanan..."
            //----------------------------------

            case 1:

                arya.Face(Vector2.down);

                break;

            //----------------------------------
            // Line 3
            // "Jika Ayah pergi dari tempat ini..."
            //----------------------------------

            case 3:

                // Melihat Portal
                arya.Face(Vector2.up);

                break;

            //----------------------------------
            // Line 8
            // "Itulah sebabnya Ayah tetap berada di sini..."
            //----------------------------------

            case 8:

                // Menatap kembali ke Saka
                arya.Face(Vector2.down);

                break;
        }
    }

    private void HandleDialogueL(int line)
    {
        Debug.Log("Dialogue L Line : " + line);

        switch (line)
        {
            //----------------------------------
            // Line 0
            // "..."
            //----------------------------------

            case 0:

                StartCoroutine(MoveSakaStep01());

                break;

            //----------------------------------
            // Line 1
            // "...Kau bohong, kan?"
            //----------------------------------

            case 1:

                // Tidak ada action

                break;

            //----------------------------------
            // Line 2
            // "AYAH!! TOLONG JANGAN BERCANDA!!"
            //----------------------------------

            case 2:

                StartCoroutine(MoveSakaStep02());

                break;

            //----------------------------------
            // Line 3
            // "INI KENYATAANNYA!!"
            //----------------------------------

            case 4:

                StartCoroutine(RealityShock());

                break;
        }
    }

    private IEnumerator MoveSakaClose()
    {
        SetActiveCamera(cmPlayer);

        Collider2D sakaCollider = player.GetComponent<Collider2D>();
        Collider2D aryaCollider = arya.GetComponent<Collider2D>();

        if (sakaCollider != null)
            sakaCollider.enabled = false;

        if (aryaCollider != null)
            aryaCollider.enabled = false;

        yield return StartCoroutine(
            playerCutscene.MoveTo(sakaClosePoint));

        playerCutscene.Face(Vector2.up);

        if (sakaCollider != null)
            sakaCollider.enabled = true;

        if (aryaCollider != null)
            aryaCollider.enabled = true;
    }

    private IEnumerator MoveSakaStep01()
    {
        yield return StartCoroutine(
            playerCutscene.MoveTo(sakaStep01));

        playerCutscene.Face(Vector2.down);
    }

    private IEnumerator MoveSakaStep02()
    {
        yield return StartCoroutine(
            playerCutscene.MoveTo(sakaStep02));

        playerCutscene.Face(Vector2.down);
    }
    private IEnumerator MoveSakaRefused()
    {
        Collider2D sakaCollider = player.GetComponent<Collider2D>();
        Collider2D aryaCollider = arya.GetComponent<Collider2D>();

        if (sakaCollider != null)
            sakaCollider.enabled = false;

        if (aryaCollider != null)
            aryaCollider.enabled = false;

        yield return StartCoroutine(
            playerCutscene.MoveTo(sakaRefused));

        playerCutscene.Face(Vector2.up);

        if (sakaCollider != null)
            sakaCollider.enabled = true;

        if (aryaCollider != null)
            aryaCollider.enabled = true;

        cmArya.Priority = 10;
        cmPlayer.Priority = 20;
    }

    

    private IEnumerator MoveAryaCalm()
    {
        yield return StartCoroutine(
            arya.MoveTo(ayahCalmSaka));

        arya.Face(Vector2.down);

        playerCutscene.Face(Vector2.up);

        cmPlayer.Priority = 10;
        cmArya.Priority = 20;
    }

    private IEnumerator MoveAryaPortal()
    {
        yield return StartCoroutine(
            arya.MoveTo(ayahGoNearPortal));

        arya.Face(Vector2.down);

        // Kamera Wide
        cmArya.Priority = 10;
        cmWide.Priority = 20;
    }

    private IEnumerator RealityShock()
    {
        // Tunggu typewriter selesai
        yield return new WaitForSeconds(0.9f);

        //----------------------------------
        // Earthquake SFX
        //----------------------------------

        PlayEarthquake();

        //----------------------------------
        // Camera Shake
        //----------------------------------

        StartCoroutine(
            cameraShake.Shake(
                0.8f,
                2.0f,
                2.5f));

        //----------------------------------
        // White Flash
        //----------------------------------

        yield return StartCoroutine(
            FlashWhite());

        //----------------------------------
        // Tahan sebentar
        //----------------------------------

        yield return new WaitForSeconds(0.15f);

        //----------------------------------
        // White Fade Out
        //----------------------------------

        yield return StartCoroutine(
            FlashBack());
    }

    private IEnumerator LaunchAryaToPortal()
    {
        float duration = 0.22f;

        float timer = 0;

        Vector3 start = arya.transform.position;
        Vector3 end = aryaPortalCenter.position;

        arya.Face(Vector2.down);

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            // Ease Out
            t = 1f - Mathf.Pow(1f - t, 3f);

            arya.transform.position =
                Vector3.Lerp(start, end, t);

            yield return null;
        }

        arya.transform.position = end;
    }

    IEnumerator FlashWhite()
    {
        float timer = 0;

        while(timer < flashDuration)
        {
            timer += Time.deltaTime;

            whiteFlash.alpha =
                Mathf.Lerp(
                    0,
                    1,
                    timer/flashDuration);

            yield return null;
        }

        whiteFlash.alpha = 1;
    }
    IEnumerator FlashBack()
    {
        float timer = 0;

        while(timer < flashDuration)
        {
            timer += Time.deltaTime;

            whiteFlash.alpha =
                Mathf.Lerp(
                    1,
                    0,
                    timer/flashDuration);

            yield return null;
        }

        whiteFlash.alpha = 0;
    }

    private void PlayThunder()
    {
        if (audioSource == null)
            return;

        if (thunderSFX == null)
            return;

        audioSource.PlayOneShot(thunderSFX);
    }

    private void PlayEarthquake()
    {
        if (audioSource == null)
            return;

        if (earthquakeSFX == null)
            return;

        audioSource.PlayOneShot(earthquakeSFX);
    }

    private void SetActiveCamera(CinemachineCamera active)
    {
        cmPlayer.Priority = 5;
        cmArya.Priority = 5;
        cmWide.Priority = 5;
        cmWide2.Priority = 5;

        active.Priority = 20;
    }

    private void PlayPush()
    {
        if (audioSource == null)
            return;

        if (pushSFX == null)
            return;

        audioSource.PlayOneShot(pushSFX);
    }

    private IEnumerator PushSaka()
    {
        float duration = 0.08f;

        float timer = 0;

        Vector3 start = player.transform.position;

        Vector3 end =
            start + Vector3.up * 0.18f;

        while(timer < duration)
        {
            timer += Time.deltaTime;

            player.transform.position =
                Vector3.Lerp(
                    start,
                    end,
                    timer / duration);

            yield return null;
        }

        player.transform.position = end;

        playerCutscene.Face(Vector2.up);
    }

    private IEnumerator KnockbackArya()
    {
        float duration = 0.07f;

        float timer = 0;

        Vector3 start = arya.transform.position;

        Vector3 end =
            start + Vector3.up * 0.18f;

        arya.Face(Vector2.down);

        while(timer < duration)
        {
            timer += Time.deltaTime;

            arya.transform.position =
                Vector3.Lerp(
                    start,
                    end,
                    timer / duration);

            yield return null;
        }

        arya.transform.position = end;
    }
}