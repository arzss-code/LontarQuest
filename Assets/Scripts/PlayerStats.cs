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

    void Start()
    {
        currentHP = maxHP;

        currentMana = maxMana;

        currentStamina = maxStamina;
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
        currentHP -= amount;

        currentHP =
            Mathf.Clamp(
                currentHP,
                0,
                maxHP
            );

        if(currentHP <= 0)
        {
            Die();
        }
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
        Debug.Log(
            "Player Died"
        );
    }
}