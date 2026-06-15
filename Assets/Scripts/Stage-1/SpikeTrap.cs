using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
public class SpikeTrap : MonoBehaviour
{
    [Header("Pengaturan Kerusakan")]
    public int damageAmount = 10;
    public float damageDelay = 0.5f;

    private bool isDangerous = false;
    private bool isPlayerOnTop = false;
    private bool canDamage = true; // Ditambahkan agar damage bisa continuous tapi berjedanya aman
    
    // PENTING: Diubah ke PlayerStats agar tidak error CS0246 di project Anda
    private PlayerStats playerHP;

    // --- DUA FUNGSI BARU INI YANG AKAN MUNCUL DI ANIMATION EVENT --- //
    public void ActivateSpikes()
    {
        isDangerous = true;
    }

    public void DeactivateSpikes()
    {
        isDangerous = false;
    }
    // -------------------------------------------------------------- //

    void Update()
    {
        if (isDangerous && isPlayerOnTop && canDamage)
        {
            StartCoroutine(ApplyDamageToPlayer());
        }
    }

    private IEnumerator ApplyDamageToPlayer()
    {
        if (playerHP != null)
        {
            canDamage = false; // Kunci sementara
            
            playerHP.TakeDamage(damageAmount);
            
            // Jeda sementara agar tidak kena damage berkali-kali per frame
            yield return new WaitForSeconds(damageDelay);
            
            canDamage = true; // Buka kunci
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnTop = true;
            playerHP = other.GetComponent<PlayerStats>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnTop = false;
            playerHP = null;
        }
    }
}
