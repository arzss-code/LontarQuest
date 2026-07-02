using UnityEngine;

public class BossTeleport : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Teleport Points")]
    [SerializeField] private Transform[] teleportPoints;

    private Transform player;

    private void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;
    }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.T))
    //     {
    //         TeleportToFarthestPoint();
    //     }
    // }

    public void StartTeleport()
    {
        Debug.Log("START TELEPORT");

        animator.SetTrigger("Teleport");
    }

    public void TeleportToFarthestPoint()
    {
        Debug.Log("TELEPORT EVENT");
        if (teleportPoints.Length == 0)
            return;

        Transform farthestPoint = teleportPoints[0];
        float farthestDistance = 0f;

        foreach (Transform point in teleportPoints)
        {
            float distance =
                Vector2.Distance(
                    point.position,
                    player.position);

            if (distance > farthestDistance)
            {
                farthestDistance = distance;
                farthestPoint = point;
            }
        }

        transform.position = farthestPoint.position;
    }
}