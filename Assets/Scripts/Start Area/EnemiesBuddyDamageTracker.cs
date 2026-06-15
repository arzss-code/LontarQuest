using TMPro;
using UnityEngine;
using System.Collections;

public class DamageBuddyDamageTracker : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI damageReceivedText;
    [SerializeField] private TextMeshProUGUI dpsText;

    [Header("Hit Effect")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [SerializeField]
    private float flashDuration = 0.1f;

    [Header("Settings")]
    [SerializeField]
    private float resetDamageDelay = 3f;

    int totalDamage = 0;

    float damageThisSecond = 0f;

    float dpsTimer = 0f;
    float noHitTimer = 0f;

    Color originalColor;

    void Start()
    {
        if(spriteRenderer != null)
            originalColor = spriteRenderer.color;

        UpdateUI();
    }

    void Update()
    {
        // ===== RESET DPS tiap 1 detik =====
        dpsTimer += Time.deltaTime;

        if(dpsTimer >= 1f)
        {
            damageThisSecond = 0;
            dpsTimer = 0;

            UpdateUI();
        }

        // ===== RESET TOTAL DAMAGE jika 3 detik tidak kena hit =====
        noHitTimer += Time.deltaTime;

        if(noHitTimer >= resetDamageDelay)
        {
            totalDamage = 0;

            UpdateUI();
        }
    }

    public void TakeDamage(int damage)
    {
        // reset timer idle
        noHitTimer = 0f;

        totalDamage += damage;
        damageThisSecond += damage;
        
        // Memunculkan angka damage putih (false = musuh)
        DamagePopupManager.Create(transform.position, damage, false);

        UpdateUI();

        // Trigger animasi hit
        if(animator != null)
        {
            animator.SetTrigger("Hit");
        }

        // Flash putih
        if(spriteRenderer != null)
        {
            StopAllCoroutines();
            StartCoroutine(HitFlash());
        }
    }

    IEnumerator HitFlash()
    {
        // lebih terang dari putih normal
        spriteRenderer.color =
            new Color(2f,2f,2f);

        yield return new WaitForSeconds(
            flashDuration
        );

        spriteRenderer.color =
            originalColor;
    }

    void UpdateUI()
    {
        damageReceivedText.text =
            "Damage Received : " + totalDamage;

        dpsText.text =
            "DPS : " + damageThisSecond;
    }
}