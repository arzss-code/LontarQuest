using UnityEngine;

public class Stage3PlayerModifier : MonoBehaviour
{
    public static Stage3PlayerModifier Instance;
    private Stage3PlayerBridge bridge;

    // private Stage3PlayerStats stats;

    [Header("Buff Multipliers")]

    [Range(0f,1f)]
    public float damageReduction = 0f;

    public float manaCostMultiplier = 1f;

    public float staminaCostMultiplier = 1f;
    

    private void Awake()
    {
        Instance = this;

        bridge = GetComponent<Stage3PlayerBridge>();
    }

    public void Apply(BuffData data)
    {
        foreach (BuffEffects effect in data.effects)
        {
            ApplyEffect(effect);
        }

        Debug.Log("Applied : " + data.title);
    }

    void ApplyEffect(BuffEffects effect)
    {
        switch(effect.buffType)
        {
            //----------------------------------
            // Heal Full HP
            //----------------------------------

            case BuffType.HealFull:

                bridge.FullHeal();

                Debug.Log("HP Fully Recovered");

                break;

            //----------------------------------
            // Heal HP
            //----------------------------------

            case BuffType.HealHP:

                bridge.HealHP(Mathf.RoundToInt(effect.value));

                Debug.Log("Heal +" + effect.value);

                break;

            //----------------------------------
            // Damage Reduction
            //----------------------------------

            case BuffType.DamageReduction:

                damageReduction += effect.value / 100f;

                Debug.Log("Damage Reduction +" + effect.value + "%");

                break;

            //----------------------------------
            // Mana Cost
            //----------------------------------

            case BuffType.ManaCostReduction:

                manaCostMultiplier *=
                    1f - effect.value / 100f;

                Debug.Log("Mana Cost -" + effect.value + "%");

                break;

            //----------------------------------
            // Dash Cost
            //----------------------------------

            case BuffType.DashCostReduction:

                staminaCostMultiplier *=
                    1f - effect.value / 100f;

                Debug.Log("Dash Cost -" + effect.value + "%");

                break;

            //----------------------------------
            // Max Mana
            //----------------------------------

            case BuffType.MaxMana:

            bridge.AddMaxMana(
                Mathf.RoundToInt(effect.value));

            Debug.Log("Max Mana +" + effect.value);

            break;

            //----------------------------------
            // Max Stamina
            //----------------------------------

            case BuffType.MaxStamina:

            bridge.AddMaxStamina(
                Mathf.RoundToInt(effect.value));

            Debug.Log("Max Stamina +" + effect.value);

            break;
        }
    }
}