using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class FireflyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject fireflyObject;
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private Light2D fireflyLight;
    [SerializeField] private FireflyGuide fireflyGuide;
    

    //--------------------------------------------------
    // Munculkan Firefly lalu terbang ke waypoint tertentu
    //--------------------------------------------------

    public void Reveal(int waypointIndex)
    {
        if (fireflyObject != null)
            fireflyObject.SetActive(true);

        if (particleSystem != null)
            particleSystem.Play();

        if (fireflyLight != null)
            fireflyLight.enabled = true;

        if (fireflyGuide != null)
            fireflyGuide.MoveToWaypoint(waypointIndex);

        Debug.Log($"Firefly Reveal -> Waypoint {waypointIndex}");
    }

    //--------------------------------------------------
    // Sembunyikan Firefly
    //--------------------------------------------------

    public void Hide()
    {
        if (particleSystem != null)
        {
            particleSystem.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (fireflyLight != null)
            fireflyLight.enabled = false;

        if (fireflyObject != null)
            fireflyObject.SetActive(false);

        Debug.Log("Firefly Hidden");
    }

    //--------------------------------------------------
    // Hidup / Matikan Light saja
    //--------------------------------------------------

    public void EnableLight(bool value)
    {
        if (fireflyLight != null)
            fireflyLight.enabled = value;
    }

    //--------------------------------------------------
    // Play Particle saja
    //--------------------------------------------------

    public void PlayParticle()
    {
        if (particleSystem != null)
            particleSystem.Play();
    }

    //--------------------------------------------------
    // Stop Particle saja
    //--------------------------------------------------

    public void StopParticle()
    {
        if (particleSystem != null)
            particleSystem.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public IEnumerator Nod()
    {
        if (fireflyGuide == null)
            yield break;

        yield return fireflyGuide.PlayNod();
    }

    

    public bool IsMoving()
    {
        if (fireflyGuide == null)
            return false;

        return fireflyGuide.IsMoving();
    }

    public IEnumerator ShowAltarOrder()
    {
        if (fireflyGuide == null)
            yield break;

        yield return fireflyGuide.GuideRoute();
    }
}