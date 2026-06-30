using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerStats))]
public class PlayerModifier : MonoBehaviour
{
    private PlayerStats playerStats;
    private Dictionary<BoonSlot, BoonData> activeBoons = new Dictionary<BoonSlot, BoonData>();

    // Variabel penampung buff sementara
    public float TotalAttackSpeedBonus { get; private set; } = 0f;
    public float TotalMovementSpeedBonus { get; private set; } = 0f;
    public float TotalDamageReduction { get; private set; } = 0f;
    public float TotalExtraStamina { get; private set; } = 0f;
    public bool HasElementalEffect { get; private set; } = false;

    private float healthRegenTimer = 0f;
    private float totalHealthRegenPerSec = 0f;

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        // Jalankan efek Health Regen jika ada
        if (totalHealthRegenPerSec > 0)
        {
            healthRegenTimer += Time.deltaTime;
            if (healthRegenTimer >= 1f)
            {
                playerStats.Heal(Mathf.RoundToInt(totalHealthRegenPerSec));
                healthRegenTimer -= 1f;
            }
        }
    }

    [Header("Visual & Audio Feedback")]
    public GameObject boonPickupVFXPrefab;
    public AudioClip boonPickupSound;

    public void AddBoon(BoonData newBoon)
    {
        if (newBoon == null) return;

        if (activeBoons.ContainsKey(newBoon.slot))
        {
            activeBoons[newBoon.slot] = newBoon;
        }
        else
        {
            activeBoons.Add(newBoon.slot, newBoon);
        }

        RecalculateModifiers();
        
        // Play Visual Feedback
        if (boonPickupVFXPrefab != null)
        {
            Instantiate(boonPickupVFXPrefab, transform.position, Quaternion.identity);
        }

        // Play Audio Feedback
        if (boonPickupSound != null)
        {
            AudioSource.PlayClipAtPoint(boonPickupSound, transform.position);
        }

        Debug.Log($"Boon Diperoleh: {newBoon.boonName} ({newBoon.type})");
    }

    private void RecalculateModifiers()
    {
        // Reset dulu semua stat
        TotalAttackSpeedBonus = 0f;
        TotalMovementSpeedBonus = 0f;
        TotalDamageReduction = 0f;
        TotalExtraStamina = 0f;
        totalHealthRegenPerSec = 0f;
        HasElementalEffect = false;

        // Hitung ulang dari semua boon yang dimiliki
        foreach (var boon in activeBoons.Values)
        {
            TotalAttackSpeedBonus += boon.attackSpeedBonus;
            TotalMovementSpeedBonus += boon.movementSpeedBonus;
            TotalDamageReduction += boon.damageReduction;
            TotalExtraStamina += boon.extraStamina;
            totalHealthRegenPerSec += boon.healthRegen;
            
            if (boon.hasElementalEffect) HasElementalEffect = true;
        }
    }

    public bool HasBoonInSlot(BoonSlot slot, out BoonData currentBoon)
    {
        return activeBoons.TryGetValue(slot, out currentBoon);
    }

    public Dictionary<BoonSlot, BoonData> GetActiveBoons()
    {
        return activeBoons;
    }

    // Fungsi ini dipanggil saat Saka mati (Roguelike Reset)
    public void ResetAllBoons()
    {
        activeBoons.Clear();
        RecalculateModifiers();
    }
}
