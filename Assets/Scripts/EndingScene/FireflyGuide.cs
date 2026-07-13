using UnityEngine;
using System.Collections;

public class FireflyGuide : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem particleSystem;

    [Header("Path")]
    [SerializeField] private Transform[] waypoints;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stopDistance = 0.05f;

    private bool moving = false;
    private int currentWaypointIndex = 0;

    //-------------------------------------------------------

    private void Awake()
    {
        if (particleSystem == null)
            particleSystem = GetComponentInChildren<ParticleSystem>();

        if (particleSystem != null)
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    //-------------------------------------------------------
    // Dipanggil setelah Dialogue C
    //-------------------------------------------------------

    public void BeginGuide()
    {
        if (waypoints.Length < 2)
        {
            Debug.LogWarning("Waypoint minimal harus 2.");
            return;
        }

        Debug.Log("Firefly BeginGuide");

        transform.position = waypoints[0].position;

        currentWaypointIndex = 1;

        if (particleSystem != null)
            particleSystem.Play();

        moving = true;

        StartCoroutine(MoveRoutine());
    }

    //-------------------------------------------------------
    // Dipanggil oleh Trigger Area
    //-------------------------------------------------------

    public void ContinueGuide()
    {
        if (moving)
            return;

        currentWaypointIndex++;

        if (currentWaypointIndex >= waypoints.Length)
        {
            Debug.Log("Firefly selesai memandu.");

            moving = false;

            return;
        }

        Debug.Log("Firefly lanjut ke Waypoint " + (currentWaypointIndex + 1));

        moving = true;

        StartCoroutine(MoveRoutine());
    }

    //-------------------------------------------------------

    private IEnumerator MoveRoutine()
    {
        Transform target = waypoints[currentWaypointIndex];

        Debug.Log("Target = " + target.name);

        while (Vector2.Distance(transform.position, target.position) > stopDistance)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                target.position,
                moveSpeed * Time.deltaTime);

            yield return null;
        }

        transform.position = target.position;

        moving = false;

        Debug.Log("Firefly berhenti di " + target.name);
    }

    //-------------------------------------------------------

    public bool IsMoving()
    {
        return moving;
    }
}