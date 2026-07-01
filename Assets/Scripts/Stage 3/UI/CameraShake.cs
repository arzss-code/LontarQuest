using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private CinemachineBasicMultiChannelPerlin noise;

    private Coroutine shakeRoutine;

    private void Awake()
    {
        StopShake();
    }

    // ==========================
    // API Lama (Kompatibel)
    // ==========================
    public void StartShake(float amplitude, float frequency)
    {
        noise.AmplitudeGain = amplitude;
        noise.FrequencyGain = frequency;
    }

    public void StopShake()
    {
        noise.AmplitudeGain = 0f;
        noise.FrequencyGain = 0f;
    }

    // ==========================
    // API Baru (Boss Slam)
    // ==========================
    public void Shake(float amplitude, float frequency, float duration)
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(
            ShakeRoutine(amplitude, frequency, duration));
    }

    private IEnumerator ShakeRoutine(
        float amplitude,
        float frequency,
        float duration)
    {
        StartShake(amplitude, frequency);

        yield return new WaitForSeconds(duration);

        StopShake();
    }
}