using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    private Collider2D hitbox;
    private PlayerStats playerStats;

    void Awake()
    {
        playerStats = GetComponentInParent<PlayerStats>();
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
        // Hanya serang jika memiliki Tag "Enemy" 
        // (Pastikan semua musuh, termasuk bos dan Gana, memiliki Tag "Enemy" di Unity)
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Damage Masuk ke: " + other.name);
            
            // Ambil damage dari PlayerStats Saka
            int damageAmount = (playerStats != null) ? playerStats.meleeDamage : 35;
            
            other.SendMessageUpwards("TakeDamage", damageAmount, SendMessageOptions.DontRequireReceiver);
        }
    }
}