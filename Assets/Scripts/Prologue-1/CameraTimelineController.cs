using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class CameraTimelineController : MonoBehaviour
{
    public CinemachineCamera playerCamera;
    public CinemachineCamera doorCamera;

    public void ShowDoorForSeconds(float duration = 3f)
    {
        StartCoroutine(DoorRoutine(duration));
    }

    private IEnumerator DoorRoutine(float duration)
    {
        doorCamera.Priority = 20;
        playerCamera.Priority = 10;

        yield return new WaitForSeconds(duration);

        doorCamera.Priority = 5;
        playerCamera.Priority = 10;
    }
}