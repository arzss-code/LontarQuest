using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CinemachineShake : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cmCamera;

    private CinemachineBasicMultiChannelPerlin noise;

    private void Awake()
    {
        noise = cmCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();

        if (noise == null)
            Debug.LogError("Noise Component belum ditambahkan!");
    }

    public IEnumerator Shake(float duration, float amplitude, float frequency)
    {
        noise.AmplitudeGain = amplitude;
        noise.FrequencyGain = frequency;

        yield return new WaitForSeconds(duration);

        noise.AmplitudeGain = 0;
        noise.FrequencyGain = 0;
    }
}