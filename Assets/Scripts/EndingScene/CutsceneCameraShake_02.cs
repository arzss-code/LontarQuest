using UnityEngine;
using System.Collections;

public class CutsceneCameraShake_02 : MonoBehaviour
{
    [SerializeField] private Transform cameraTarget;

    Vector3 originalPos;

    private void Awake()
    {
        if (cameraTarget == null)
            cameraTarget = transform;

        originalPos = cameraTarget.localPosition;
    }

    //--------------------------------------------------

    public IEnumerator Shake(float duration, float magnitude)
    {
        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float strength =
                Mathf.Lerp(
                    magnitude,
                    0,
                    timer / duration);

            cameraTarget.position =
                originalPos +
                (Vector3)Random.insideUnitCircle * strength;

            yield return null;
        }

        cameraTarget.localPosition = originalPos;
    }
}