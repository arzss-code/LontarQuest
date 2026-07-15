using UnityEngine;

public class Stage3PlayerBridge : MonoBehaviour
{
    private PlayerStats stats;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();

        if (stats == null)
        {
            Debug.LogError("PlayerStats tidak ditemukan!");
        }
    }

    //----------------------------------
    // HP
    //----------------------------------

    public void FullHeal()
    {
        stats.Heal(stats.MaxHP);
    }

    public void HealHP(int amount)
    {
        stats.Heal(amount);
    }

    //----------------------------------
    // Mana
    //----------------------------------

    public int CurrentMana => stats.CurrentMana;

    public int MaxMana => stats.MaxMana;

    public void AddMaxMana(int amount)
    {
        stats.AddMaxMana(amount);
    }

    //----------------------------------
    // Stamina
    //----------------------------------

    public int CurrentStamina => stats.CurrentStamina;

    public int MaxStamina => stats.MaxStamina;

    public void AddMaxStamina(int amount)
    {
        stats.AddMaxStamina(amount);
    }
    
}