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

    [Header("Hover")]
    [SerializeField] private bool enableHover = false;

    [SerializeField] private float hoverHeight = 0.12f;
    [SerializeField] private float hoverSpeed = 2f;

    private Vector3 hoverOrigin;

    private bool moving = false;
    private int currentWaypointIndex = 0;

    // Coroutine movement
    private Coroutine moveRoutine;

    //-------------------------------------------------------

    private void Awake()
    {
        if (particleSystem == null)
            particleSystem = GetComponentInChildren<ParticleSystem>();

        if (particleSystem != null)
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void Update()
    {
        if (!enableHover)
            return;

        if (moving)
            return;

        Vector3 pos = hoverOrigin;

        pos.y += Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;

        transform.position = pos;
    }

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

        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveRoutine());
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

        Debug.Log("Firefly lanjut ke " + waypoints[currentWaypointIndex].name);

        moving = true;

        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveRoutine());
    }

    //-------------------------------------------------------
    // Pergi ke Waypoint tertentu
    //-------------------------------------------------------

    public void MoveToWaypoint(int index)
    {
        if (index < 0 || index >= waypoints.Length)
            return;

        enableHover = false;

        currentWaypointIndex = index;

        moving = true;

        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveRoutine());
    }

    //-------------------------------------------------------

    private IEnumerator MoveRoutine()
    {
        if (currentWaypointIndex >= waypoints.Length)
        {
            moving = false;
            yield break;
        }

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

        //-----------------------------------
        // Pastikan posisi tepat di waypoint
        //-----------------------------------

        transform.position = target.position;

        //-----------------------------------
        // Selesai bergerak
        //-----------------------------------

        moving = false;

        //-----------------------------------
        // Mulai Hover
        //-----------------------------------

        hoverOrigin = transform.position;
        enableHover = true;

        Debug.Log("Firefly berhenti di " + target.name);
    }

    private IEnumerator MoveSmooth(Vector3 from, Vector3 to, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            transform.position =
                Vector3.Lerp(
                    from,
                    to,
                    timer / duration);

            yield return null;
        }

        transform.position = to;
    }


    //-------------------------------------------------------

    public bool IsMoving()
    {
        return moving;
    }

    public IEnumerator PlayNod()
    {
        yield return StartCoroutine(NodRoutine());
    }  

    public IEnumerator NodRoutine()
    {
        enableHover = false;

        Vector3 start = hoverOrigin;

        Vector3 up = start + Vector3.up * 0.18f;

        Vector3 down = start - Vector3.up * 0.08f;

        yield return MoveSmooth(start, up, 0.2f);

        yield return MoveSmooth(up, down, 0.2f);

        yield return MoveSmooth(down, up, 0.2f);

        yield return MoveSmooth(up, start, 0.2f);

        hoverOrigin = start;

        enableHover = true;
    }

    public IEnumerator MoveToWaypointRoutine(int index)
    {
        MoveToWaypoint(index);

        yield return new WaitUntil(() => !moving);
    }

    public IEnumerator GuideRoute()
    {
        // Altar 1
        yield return MoveToWaypointRoutine(7);

        // Altar 2
        yield return MoveToWaypointRoutine(8);

        // Altar 3
        yield return MoveToWaypointRoutine(9);

        // Altar 4
        yield return MoveToWaypointRoutine(10);

        // Altar 5
        yield return MoveToWaypointRoutine(11);

        // Kembali ke Ayah
        yield return MoveToWaypointRoutine(6);
    }
}