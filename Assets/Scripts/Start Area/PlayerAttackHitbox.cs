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
        // Hanya serang jika memiliki Tag "Enemy" 
        // (Pastikan semua musuh, termasuk bos dan Gana, memiliki Tag "Enemy" di Unity)
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Damage Masuk ke: " + other.name);
            
            // SendMessageUpwards akan otomatis memanggil fungsi "TakeDamage(int)" 
            // pada script apapun (KalaAI, GanaAI, dll) yang menempel di objek ini atau parentnya.
            other.SendMessageUpwards("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        }
    }
}