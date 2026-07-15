using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerStats))]
public class PlayerModifier : MonoBehaviour
{
    private PlayerStats playerStats;
    private List<BoonData> activeBoons = new List<BoonData>();

    public System.Action OnBoonsChanged;

    // Variabel penampung buff sementara
    public float TotalAttackSpeedBonus { get; private set; } = 0f;
    public float TotalMovementSpeedBonus { get; private set; } = 0f;
    public float TotalDamageReduction { get; private set; } = 0f;
    public float TotalExtraStamina { get; private set; } = 0f;
    public float TotalGlobalDamageBonus { get; private set; } = 0f;
    public int TotalExtraHealth { get; private set; } = 0;
    public bool HasElementalEffect { get; private set; } = false;

    private float healthRegenTimer = 0f;
    private float totalHealthRegenPerSec = 0f;

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();

        // Load Boon dari Run Sebelumnya (Jika pindah stage)
        if (SaveManager.Instance != null && SaveManager.Instance.CurrentRun.isRunActive)
        {
            foreach (BoonData boon in SaveManager.Instance.CurrentRun.activeBoons)
            {
                if (boon != null)
                {
                    activeBoons.Add(boon);
                }
            }
            RecalculateModifiers();
            OnBoonsChanged?.Invoke();
        }
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

        // Cek apakah pemain sudah memiliki Boon dari elemen (type) yang sama
        int existingIndex = activeBoons.FindIndex(b => b.type == newBoon.type);

        if (existingIndex != -1)
        {
            // Jika sudah punya elemen ini, tumpuk/timpa Boon lama dengan yang baru (Upgrade ke Lv2)
            activeBoons[existingIndex] = newBoon;
        }
        else
        {
            // Jika belum punya elemen ini, dan slot belum penuh (maks 2)
            if (activeBoons.Count < 2)
            {
                activeBoons.Add(newBoon);
            }
            else
            {
                // Slot penuh, ini adalah kondisi 'Replace' (mengganti boon acak, atau di masa depan bisa dibuat pilih)
                // Untuk sementara, ganti slot pertama (index 0)
                activeBoons[0] = newBoon;
            }
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

        // Simpan ke SaveManager agar terbawa ke scene selanjutnya
        if (SaveManager.Instance != null)
        {
            int currentHp = playerStats != null ? playerStats.CurrentHP : -1;
            SaveManager.Instance.SaveRunState(currentHp, new List<BoonData>(activeBoons));
        }

        OnBoonsChanged?.Invoke();
    }

    private void RecalculateModifiers()
    {
        // Reset dulu semua stat
        TotalAttackSpeedBonus = 0f;
        TotalMovementSpeedBonus = 0f;
        TotalDamageReduction = 0f;
        TotalExtraStamina = 0f;
        TotalGlobalDamageBonus = 0f;
        TotalExtraHealth = 0;
        totalHealthRegenPerSec = 0f;
        HasElementalEffect = false;

        // Hitung ulang dari semua boon yang dimiliki
        foreach (var boon in activeBoons)
        {
            TotalAttackSpeedBonus += boon.attackSpeedBonus;
            TotalMovementSpeedBonus += boon.movementSpeedBonus;
            TotalDamageReduction += boon.damageReduction;
            TotalExtraStamina += boon.extraStamina;
            TotalGlobalDamageBonus += boon.globalDamageBonus;
            TotalExtraHealth += boon.extraHealth;
            totalHealthRegenPerSec += boon.healthRegen;
            
            if (boon.hasElementalEffect) HasElementalEffect = true;
        }
    }

    public bool HasBoonOfType(BoonType type, out BoonData currentBoon)
    {
        currentBoon = activeBoons.Find(b => b.type == type);
        return currentBoon != null;
    }

    public List<BoonData> GetActiveBoons()
    {
        return activeBoons;
    }

    // Fungsi ini dipanggil saat Saka mati (Roguelike Reset)
    public void ResetAllBoons()
    {
        activeBoons.Clear();
        RecalculateModifiers();
        OnBoonsChanged?.Invoke();
    }
}
