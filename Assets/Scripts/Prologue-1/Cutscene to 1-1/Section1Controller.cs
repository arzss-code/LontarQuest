using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class Section1Controller : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cineCamera;

    [SerializeField] private Transform jeep;

    [SerializeField] private float followDuration = 2f;

    private IEnumerator Start()
    {
        cineCamera.Follow = jeep;

        yield return new WaitForSeconds(followDuration);

        cineCamera.Follow = null;
    }
}