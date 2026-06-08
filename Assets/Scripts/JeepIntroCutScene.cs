using System.Collections;
using UnityEngine;

public class JeepIntroCutscene : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform cameraIntroPoint;
    [SerializeField] private float roadShotDuration = 1.2f;
    [SerializeField] private float cameraSmoothSpeed = 5f;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 0f, -10f);

    [Header("Jeep Driving")]
    [SerializeField] private GameObject jeepObject;
    [SerializeField] private Transform jeepVisual;
    [SerializeField] private Transform[] pathPoints;
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float rotateSpeed = 0.8f;
    [SerializeField] private float jeepShowDelay = 0.5f;

    [Header("Jeep Swap")]
    [SerializeField] private GameObject parkedEmptyJeep;

    [Header("Player")]
    [SerializeField] private GameObject playerObject;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform playerSpawnPoint;

    [Header("After Jeep Arrives")]
    [SerializeField] private float arriveDelay = 0.8f;

    [Header("Next Cutscene")]
    [SerializeField] private CutsceneManager cutsceneManager;

    private Transform cameraTarget;
    private bool cameraCanFollow = false;

    private void Start()
    {
        StartCoroutine(PlayIntro());
    }

    private void LateUpdate()
    {
        if (!cameraCanFollow)
            return;

        if (mainCamera == null || cameraTarget == null)
            return;

        Vector3 targetPosition = cameraTarget.position + cameraOffset;

        mainCamera.transform.position = Vector3.Lerp(
            mainCamera.transform.position,
            targetPosition,
            cameraSmoothSpeed * Time.deltaTime
        );
    }

    private IEnumerator PlayIntro()
    {
        if (playerController != null)
            playerController.SetCanMove(false);

        if (playerObject != null)
            playerObject.SetActive(false);

        if (jeepObject != null)
            jeepObject.SetActive(false);

        if (parkedEmptyJeep != null)
            parkedEmptyJeep.SetActive(false);

        cameraCanFollow = false;

        if (mainCamera != null && cameraIntroPoint != null)
        {
            mainCamera.transform.position = cameraIntroPoint.position + cameraOffset;
        }

        yield return new WaitForSeconds(roadShotDuration);

        if (pathPoints.Length == 0)
        {
            Debug.LogError("Path Points belum diisi!");
            yield break;
        }

        jeepObject.transform.position = pathPoints[0].position;
        jeepObject.SetActive(true);

        if (mainCamera != null)
        {
            mainCamera.transform.position = jeepObject.transform.position + cameraOffset;
        }

        cameraTarget = jeepObject.transform;
        cameraCanFollow = true;

        yield return new WaitForSeconds(jeepShowDelay);

        for (int i = 1; i < pathPoints.Length; i++)
        {
            yield return MoveJeepToPoint(pathPoints[i]);
        }

        yield return new WaitForSeconds(arriveDelay);

        // Simpan posisi dan rotasi akhir jeep yang bergerak
        Vector3 finalJeepPosition = jeepObject.transform.position;
        Quaternion finalJeepRotation = jeepVisual != null
            ? jeepVisual.rotation
            : jeepObject.transform.rotation;

        // Hilangkan jeep yang ada Saka
        jeepObject.SetActive(false);

        // Munculkan jeep kosong di posisi akhir
        if (parkedEmptyJeep != null)
        {
            parkedEmptyJeep.transform.position = finalJeepPosition;
            parkedEmptyJeep.transform.rotation = finalJeepRotation;
            parkedEmptyJeep.SetActive(true);
        }

        // Munculkan Player/Saka
        if (playerObject != null && playerSpawnPoint != null)
        {
            playerObject.transform.position = playerSpawnPoint.position;
            playerObject.SetActive(true);
        }

        // Kamera pindah follow player
        if (playerObject != null)
        {
            cameraTarget = playerObject.transform;
        }

        // Mulai dialog
        if (cutsceneManager != null)
        {
            cutsceneManager.StartIntroCutscene();
        }
    }

    private IEnumerator MoveJeepToPoint(Transform targetPoint)
    {
        while (Vector2.Distance(jeepObject.transform.position, targetPoint.position) > 0.05f)
        {
            Vector3 direction = targetPoint.position - jeepObject.transform.position;

            RotateJeep(direction);

            jeepObject.transform.position = Vector3.MoveTowards(
                jeepObject.transform.position,
                targetPoint.position,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        jeepObject.transform.position = targetPoint.position;
    }

    private void RotateJeep(Vector3 direction)
    {
        if (jeepVisual == null)
            return;

        if (direction.sqrMagnitude < 0.001f)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);

        jeepVisual.rotation = Quaternion.RotateTowards(
            jeepVisual.rotation,
            targetRotation,
            rotateSpeed * 60f * Time.deltaTime
        );
    }
}