using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("HP")]
    [SerializeField]
    int maxHP = 200;

    int currentHP;

    [Header("Mana")]
    [SerializeField]
    int maxMana = 100;

    float currentMana;

    [Header("Stamina")]
    [SerializeField]
    int maxStamina = 100;

    float currentStamina;

    [Header("Offense (Damage)")]
    [Tooltip("Besar damage untuk pukulan jarak dekat (Pedang)")]
    public int meleeDamage = 35;
    
    [Tooltip("Besar damage untuk serangan jarak jauh (Panah)")]
    public int rangedDamage = 15;

    [Header("Recharge Rate")]
    [SerializeField]
    float manaRechargeRate = 10f;

    [SerializeField]
    float staminaRechargeRate = 20f;

    [Header("Recharge Delay")]
    [SerializeField]
    float manaRechargeDelay = 1f;

    [SerializeField]
    float staminaRechargeDelay = 1f;

    float manaRechargeTimer;
    float staminaRechargeTimer;

    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;

    public int MaxMana => maxMana;
    public int CurrentMana => Mathf.RoundToInt(currentMana);

    public int MaxStamina => maxStamina;
    public int CurrentStamina => Mathf.RoundToInt(currentStamina);

    [Header("Damage Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float invincibilityDuration = 0.6f;
    
    private Color originalColor;
    private bool isInvincible = false;
    private bool isDead = false;

    void Start()
    {
        currentHP = maxHP;
        currentMana = maxMana;
        currentStamina = maxStamina;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void Update()
    {
        UpdateManaRecharge();

        UpdateStaminaRecharge();
    }

    void UpdateManaRecharge()
    {
        if(manaRechargeTimer > 0)
        {
            manaRechargeTimer -=
            Time.deltaTime;

            return;
        }

        if(currentMana >= maxMana)
            return;

        currentMana +=
            manaRechargeRate *
            Time.deltaTime;

        currentMana =
            Mathf.Clamp(
                currentMana,
                0,
                maxMana
            );
    }

    void UpdateStaminaRecharge()
    {
        if(staminaRechargeTimer > 0)
        {
            staminaRechargeTimer -=
            Time.deltaTime;

            return;
        }

        if(currentStamina >= maxStamina)
            return;

        currentStamina +=
            staminaRechargeRate *
            Time.deltaTime;

        currentStamina =
            Mathf.Clamp(
                currentStamina,
                0,
                maxStamina
            );
    }

    public bool UseMana(int amount)
    {
        if(currentMana < amount)
            return false;

        currentMana -= amount;

        manaRechargeTimer =
            manaRechargeDelay;

        return true;
    }

    public bool UseStamina(int amount)
    {
        if(currentStamina < amount)
            return false;

        currentStamina -= amount;

        staminaRechargeTimer =
            staminaRechargeDelay;

        return true;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return; // Kalau sudah mati, jangan terima damage lagi
        if (isInvincible) return; // Abaikan damage jika masih kebal (I-Frames)

        currentHP -= amount;
        
        // Panggil teks damage melayang merah (true = damage pemain)
        DamagePopupManager.Create(transform.position, amount, true);

        currentHP =
            Mathf.Clamp(
                currentHP,
                0,
                maxHP
            );

        if (spriteRenderer != null && gameObject.activeInHierarchy)
        {
            StartCoroutine(DamageFlashRoutine());
        }

        if(currentHP <= 0)
        {
            Die();
        }
    }

    private System.Collections.IEnumerator DamageFlashRoutine()
    {
        isInvincible = true;

        // Berkedip merah SEKALI saja
        spriteRenderer.color = damageColor;
        
        // Tahan warna merah sebentar saja (0.15 detik)
        float flashTime = 0.15f;
        yield return new WaitForSeconds(flashTime);
        
        // Kembalikan ke warna asli
        spriteRenderer.color = originalColor;

        // Tunggu sisa waktu kebal (I-Frames) tanpa berkedip lagi
        float remainingInvincibility = invincibilityDuration - flashTime;
        if (remainingInvincibility > 0)
        {
            yield return new WaitForSeconds(remainingInvincibility);
        }

        isInvincible = false;
    }

    public void Heal(int amount)
    {
        currentHP += amount;

        currentHP =
            Mathf.Clamp(
                currentHP,
                0,
                maxHP
            );
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Player Died");

        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.TriggerDeath();
            // Matikan script PlayerController agar Saka tidak bisa digerakkan sama sekali
            pc.enabled = false;
        }

        // Jalankan fade out dan restart
        StartCoroutine(RestartSceneRoutine(4.5f));
    }

    private System.Collections.IEnumerator RestartSceneRoutine(float delay)
    {
        // 1. Tunggu 1 detik agar pemain melihat animasi mati diputar sampai full
        yield return new WaitForSeconds(1.5f);
        
        // 2. Buat Canvas dan Layar Hitam (Fade Out) secara dinamis
        GameObject canvasObj = new GameObject("GameOverCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        
        GameObject imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(canvasObj.transform, false);
        UnityEngine.UI.Image fadeImage = imageObj.AddComponent<UnityEngine.UI.Image>();
        fadeImage.color = new Color(0, 0, 0, 0); // Mulai dari transparan
        
        RectTransform rect = fadeImage.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        // 3. Lakukan proses Fade In ke warna Hitam
        float fadeDuration = delay - 1.5f;
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        
        // Memuat ulang scene
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneIndex);
    }
}