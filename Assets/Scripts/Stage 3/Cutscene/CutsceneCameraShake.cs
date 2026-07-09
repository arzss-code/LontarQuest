using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CutsceneCameraShake : MonoBehaviour
{
    [SerializeField] private CinemachineCamera targetCamera;

    [SerializeField] private float defaultAmplitude = 2f;
    [SerializeField] private float defaultFrequency = 2f;

    public IEnumerator Shake(
        float amplitude,
        float frequency,
        float duration)
    {
        var noise =
            targetCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();

        if (noise == null)
            yield break;

        noise.AmplitudeGain = amplitude;
        noise.FrequencyGain = frequency;

        yield return new WaitForSeconds(duration);

        noise.AmplitudeGain = 0;
        noise.FrequencyGain = 0;
    }
}