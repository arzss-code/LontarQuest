using UnityEngine;

public class GravityPullController : MonoBehaviour
{
    [Header("Gravity")]
    [SerializeField] private Transform gravityCenter;
    [SerializeField] private float pullRadius = 3.5f;
    [SerializeField] private float pullForce = 45f;

    private Transform player;
    private PlayerController playerController;

    private bool isPulling = false;

    private void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj == null)
        {
            Debug.LogError("[GravityPull] Player tidak ditemukan!");
            enabled = false;
            return;
        }

        player = playerObj.transform;
        playerController = playerObj.GetComponent<PlayerController>();

        if (playerController == null)
        {
            Debug.LogError("[GravityPull] PlayerController tidak ditemukan!");
            enabled = false;
            return;
        }

        if (gravityCenter == null)
        {
            Debug.LogError("[GravityPull] Gravity Center belum di-assign!");
            enabled = false;
            return;
        }
    }

    public void StartPull()
    {
        isPulling = true;
    }

    public void StopPull()
    {
        isPulling = false;

        if (playerController != null)
            playerController.ClearExternalMovement();
    }

    private void FixedUpdate()
    {
        if (!isPulling)
            return;

        float distance = Vector2.Distance(
            gravityCenter.position,
            player.position);

        // Player di luar radius
        if (distance >= pullRadius)
        {
            playerController.ClearExternalMovement();
            return;
        }

        // Arah menuju pusat gravitasi
        Vector2 direction =
            ((Vector2)gravityCenter.position -
            (Vector2)player.position).normalized;

        // Persentase kedekatan
        float t = 1f - (distance / pullRadius);

        // Tarikan dari 10% sampai 100%
        float strength =
            pullForce *
            Mathf.Lerp(0.1f, 1f, t);

        Vector2 force = direction * strength;

        playerController.SetExternalMovement(force);
    }

    private void OnDisable()
    {
        if (playerController != null)
            playerController.ClearExternalMovement();
    }

    private void OnDrawGizmosSelected()
    {
        if (gravityCenter == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(gravityCenter.position, pullRadius);
    }
}