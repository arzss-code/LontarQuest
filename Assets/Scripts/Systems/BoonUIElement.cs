using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoonUIElement : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button selectButton;

    private BoonData currentBoon;

    private void Start()
    {
        if (selectButton != null)
        {
            selectButton.onClick.AddListener(OnBoonSelected);
        }
    }

    /// <summary>
    /// Mengisi data boon ke dalam UI Button ini
    /// </summary>
    public void Setup(BoonData boonData)
    {
        currentBoon = boonData;
        gameObject.SetActive(true);

        if (nameText != null)
            nameText.text = boonData.boonName;

        if (descriptionText != null)
        {
            // Auto-Generate Deskripsi berdasarkan Stat
            string autoDesc = "";
            if (boonData.attackSpeedBonus > 0) autoDesc += $"<color=green>+{boonData.attackSpeedBonus * 100}%</color> Attack Speed\n";
            if (boonData.movementSpeedBonus > 0) autoDesc += $"<color=green>+{boonData.movementSpeedBonus * 100}%</color> Move Speed\n";
            if (boonData.damageReduction > 0) autoDesc += $"<color=blue>-{boonData.damageReduction * 100}%</color> Damage Taken\n";
            if (boonData.healthRegen > 0) autoDesc += $"<color=red>+{boonData.healthRegen}</color> HP Regen/s\n";
            if (boonData.extraStamina > 0) autoDesc += $"<color=orange>+{boonData.extraStamina}</color> Max Stamina\n";
            if (boonData.hasElementalEffect) autoDesc += $"<color=purple>Efek Elemental Aktif</color>\n";

            // Jika semua stat 0, gunakan teks deskripsi manual sebagai cadangan
            if (string.IsNullOrEmpty(autoDesc))
            {
                autoDesc = boonData.description;
            }

            descriptionText.text = autoDesc;
        }

        if (iconImage != null)
        {
            if (boonData.icon != null)
            {
                iconImage.sprite = boonData.icon;
                iconImage.enabled = true;
                iconImage.color = Color.white; // Pastikan warnanya tidak transparan
            }
            else
            {
                iconImage.enabled = false;
                Debug.LogWarning($"[Boon UI] {boonData.boonName} tidak memiliki gambar Ikon di datanya!");
            }
        }
        else
        {
            Debug.LogError($"[Boon UI] Komponen IconImage pada tombol {gameObject.name} terlepas/kosong! Harap generate ulang UI Prefab.");
        }
    }

    /// <summary>
    /// Fungsi yang terpanggil saat tombol diklik
    /// </summary>
    private void OnBoonSelected()
    {
        if (currentBoon != null && BoonUIManager.Instance != null)
        {
            BoonUIManager.Instance.SelectBoon(currentBoon);
        }
    }
}
