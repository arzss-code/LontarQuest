using UnityEngine;
using System.Collections.Generic;

public class PlayerAttackHitbox : MonoBehaviour
{
    private Collider2D hitbox;
    private PlayerStats playerStats;
    private List<GameObject> hitEnemies = new List<GameObject>();

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
            hitEnemies.Clear(); // Bersihkan daftar musuh terpukul untuk serangan baru
            Debug.Log(gameObject.name + " HITBOX AKTIF");
        }
    }

    public void DeactivateHitbox()
    {
        if(hitbox != null)
        {
            hitbox.enabled = false;
            hitEnemies.Clear(); // Bersihkan memori list
            Debug.Log(gameObject.name + " HITBOX MATI");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Panjat ke parent teratas yang di-tag "Enemy" untuk mengidentifikasi objek musuh utama
            Transform current = other.transform;
            GameObject targetEnemy = other.gameObject;
            while (current != null)
            {
                if (current.CompareTag("Enemy"))
                {
                    targetEnemy = current.gameObject;
                    break;
                }
                current = current.parent;
            }

            // Jika musuh ini sudah terkena damage dari ayunan pedang ini, abaikan
            if (hitEnemies.Contains(targetEnemy))
            {
                return;
            }
            hitEnemies.Add(targetEnemy);

            Debug.Log("Damage Masuk ke: " + targetEnemy.name);
            
            int damageAmount = (playerStats != null) ? playerStats.meleeDamage : 35;
            targetEnemy.SendMessageUpwards("TakeDamage", damageAmount, SendMessageOptions.DontRequireReceiver);

            // Cek efek elemen (Nyala Api Kawi)
            PlayerModifier pm = GetComponentInParent<PlayerModifier>();
            if (pm != null && pm.HasElementalEffect)
            {
                BurnEffect existingBurn = targetEnemy.GetComponent<BurnEffect>();
                if (existingBurn == null)
                {
                    BurnEffect burn = targetEnemy.gameObject.AddComponent<BurnEffect>();
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