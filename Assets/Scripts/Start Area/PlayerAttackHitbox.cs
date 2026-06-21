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
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Damage Masuk ke: " + other.name);
            
            int damageAmount = (playerStats != null) ? playerStats.meleeDamage : 35;
            other.SendMessageUpwards("TakeDamage", damageAmount, SendMessageOptions.DontRequireReceiver);

            // Cek efek elemen (Nyala Api Kawi)
            PlayerModifier pm = GetComponentInParent<PlayerModifier>();
            if (pm != null && pm.HasElementalEffect)
            {
                BurnEffect existingBurn = other.GetComponent<BurnEffect>();
                if (existingBurn == null)
                {
                    BurnEffect burn = other.gameObject.AddComponent<BurnEffect>();
                    burn.StartBurn(5, 4); // 5 damage per detik selama 4 detik
                }
                else
                {
                    existingBurn.StopAllCoroutines();
                    existingBurn.StartBurn(5, 4); // Reset durasi api
                }
            }
        }
    }
}