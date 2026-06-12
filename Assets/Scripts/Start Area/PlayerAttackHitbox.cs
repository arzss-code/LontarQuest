using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField]
    private int damage = 25;

    private Collider2D hitbox;

    void Awake()
    {
        hitbox = GetComponent<Collider2D>();

        if(hitbox != null)
        {
            hitbox.enabled = false;
        }
        else
        {
            Debug.LogError(gameObject.name + " tidak punya Collider2D!");
        }
    }

    public void ActivateHitbox()
    {
        Debug.Log("AKTIFKAN HITBOX");
        
        if(hitbox != null)
        {
            hitbox.enabled = true;

            Debug.Log(gameObject.name + " HITBOX AKTIF");
        }
    }

    public void DeactivateHitbox()
    {
        if(hitbox != null)
        {
            hitbox.enabled = false;

            Debug.Log(gameObject.name + " HITBOX MATI");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Kena: " + other.name);

        DamageBuddyDamageTracker buddy =
            other.GetComponentInParent<DamageBuddyDamageTracker>();

        if(buddy != null)
        {
            Debug.Log("Damage Masuk: " + damage);

            buddy.TakeDamage(damage);
        }
    }
}