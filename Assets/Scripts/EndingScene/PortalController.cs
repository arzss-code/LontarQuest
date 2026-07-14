using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PortalController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem portalParticle;
    [SerializeField] private Light2D portalLight;

    [Header("Audio")]
    [SerializeField] private AudioSource portalAudio;

    [SerializeField] private AudioClip portalIdleSFX;

    private void Awake()
    {
        gameObject.SetActive(false);

        if (portalAudio == null)
            portalAudio = GetComponent<AudioSource>();
    }

    //--------------------------------------------------

    public void Spawn()
    {
        gameObject.SetActive(true);

        if (portalLight != null)
            portalLight.enabled = true;

        if (portalParticle != null)
            portalParticle.Play();

        PlayPortalLoop();

        Debug.Log("Portal Spawn");
    }

    //--------------------------------------------------

    public void Hide()
    {
        if (portalParticle != null)
            portalParticle.Stop();

        if (portalLight != null)
            portalLight.enabled = false;

        gameObject.SetActive(false);
    }

    private void PlayPortalLoop()
    {
        if (portalAudio == null)
            return;

        if (portalIdleSFX == null)
            return;

        portalAudio.clip = portalIdleSFX;
        portalAudio.loop = true;
        portalAudio.volume = 0.2f;   // sesuaikan

        portalAudio.Play();
    }

    public void StopPortalSound()
    {
        if (portalAudio != null)
            portalAudio.Stop();
    }
}