using UnityEngine;

public class AnimationRelay : MonoBehaviour
{
    PlayerController player;

    void Awake()
    {
        player = GetComponentInParent<PlayerController>();
    }

    public void EndAttack()
    {
        player.EndAttack();
    }

    public void ActivateHitbox()
    {
        Debug.Log("EVENT AKTIF");
        player.ActivateHitbox();
    }

    public void DeactivateHitbox()
    {
        Debug.Log("EVENT MATI");
        player.DeactivateHitbox();
    }
}