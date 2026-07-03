using UnityEngine;
using System;
using System.Collections;

public class BossStats : MonoBehaviour
{
    public enum ShieldState
    {
        Active,
        Broken,
        Regenerating
    }

    // Event untuk memberi tahu UI jika HP/Shield berubah
    public event System.Action OnHealthChanged;
    public event System.Action OnShieldBroken;
    public event System.Action OnShieldRecovered;

    [Header("Health")]
    [SerializeField] private float maxHP = 1000f;
    private float currentHP;

    [Header("Shield")]
    [SerializeField] private float maxShield = 500f;
    private float currentShield;

    [Header("Shield Regen")]
    [SerializeField] private float shieldRegenDelay = 2f;
    [SerializeField] private float shieldRegenDuration = 5f;

    private Coroutine regenRoutine;

    [Header("Damage Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float shieldAbsorption = 0.7f;

    [SerializeField] private float brokenDamageMultiplier = 1.5f;

    private ShieldState shieldState = ShieldState.Active;

    //==============================
    // Properties
    //==============================

    public float HPPercent => currentHP / maxHP;
    public float ShieldPercent => currentShield / maxShield;

    public float CurrentHP => currentHP;
    public float CurrentShield => currentShield;

    public float MaxHP => maxHP;
    public float MaxShield => maxShield;

    public ShieldState CurrentShieldState => shieldState;

    private void Awake()
    {
        currentHP = maxHP;
        currentShield = maxShield;

        Debug.Log("===== BossStats Awake =====");
        Debug.Log("Max HP : " + maxHP);
        Debug.Log("Current HP : " + currentHP);
        Debug.Log("Max Shield : " + maxShield);
        Debug.Log("Current Shield : " + currentShield);
    }

    public void TakeDamage(int damage)
    {
        if (currentHP <= 0)
            return;

        // Hit Flash
        BossController controller = GetComponent<BossController>();

        if (controller != null)
        {
            controller.PlayHitFlash();
        }

        float finalDamage = damage;

        //--------------------------------------------------
        // Shield Aktif
        //--------------------------------------------------

        if (shieldState == ShieldState.Active)
        {
            float shieldDamage = finalDamage * shieldAbsorption;
            float hpDamage = finalDamage - shieldDamage;

            currentShield -= shieldDamage;
            currentHP -= hpDamage;

            if (currentShield <= 0)
            {
                currentShield = 0;
                BreakShield();
            }
        }
        //--------------------------------------------------
        // Shield Pecah / Regenerasi
        //--------------------------------------------------
        else
        {
            currentHP -= finalDamage * brokenDamageMultiplier;
        }

        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        OnHealthChanged?.Invoke();

        Debug.Log($"HP : {currentHP}/{maxHP} | Shield : {currentShield}/{maxShield}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void BreakShield()
    {
        shieldState = ShieldState.Broken;

        Debug.Log("===== SHIELD BROKEN =====");

        OnShieldBroken?.Invoke();

        if (regenRoutine != null)
            StopCoroutine(regenRoutine);

        regenRoutine = StartCoroutine(RegenShieldRoutine());
    }

    private IEnumerator RegenShieldRoutine()
    {
        //--------------------------------------------------
        // Delay sebelum mulai regen
        //--------------------------------------------------

        yield return new WaitForSeconds(shieldRegenDelay);

        shieldState = ShieldState.Regenerating;

        Debug.Log("Shield mulai regenerasi...");

        float timer = 0f;

        while (timer < shieldRegenDuration)
        {
            timer += Time.deltaTime;

            currentShield = Mathf.Lerp(
                0f,
                maxShield,
                timer / shieldRegenDuration);

            // Update UI setiap frame regen
            OnHealthChanged?.Invoke();

            yield return null;
        }

        currentShield = maxShield;

        shieldState = ShieldState.Active;

        OnHealthChanged?.Invoke();

        OnShieldRecovered?.Invoke();

        Debug.Log("===== SHIELD RESTORED =====");
    }

    private void Die()
{
    Debug.Log("===== BAHTARA KALA DEAD =====");

    StopAllCoroutines();

    BossAttack attack = GetComponent<BossAttack>();
    if (attack != null)
        attack.enabled = false;

    BossMovement movement = GetComponent<BossMovement>();
    if (movement != null)
        movement.enabled = false;

    BossController controller = GetComponent<BossController>();
    if (controller != null)
        controller.enabled = false;

    //----------------------------------------
    // Beritahu BossManager
    //----------------------------------------

    BossManager manager =
        FindFirstObjectByType<BossManager>();

    if (manager != null)
        manager.EndBossFight();

    //----------------------------------------
    // Destroy setelah sebentar
    //----------------------------------------

    Destroy(gameObject, 1f);
}
}