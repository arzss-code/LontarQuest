using UnityEngine;
using UnityEngine.UI;

public class PlayerHudController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    PlayerStats playerStats;

    [Header("Bars")]
    [SerializeField]
    Image hpFill;

    [SerializeField]
    Image manaFill;

    [SerializeField]
    Image staminaFill;

    [Header("Boon Indicator")]
    private UnityEngine.UI.HorizontalLayoutGroup boonIconContainer;
    private System.Collections.Generic.List<Image> boonIcons = new System.Collections.Generic.List<Image>();
    private PlayerModifier playerModifier;

    private void Start()
    {
        if (playerStats != null)
        {
            playerModifier = playerStats.GetComponent<PlayerModifier>();
            if (playerModifier != null)
            {
                playerModifier.OnBoonsChanged += UpdateBoonIcons;
            }
        }
        
        SetupBoonUI();
        UpdateBoonIcons(); // Initial call
    }

    private void OnDestroy()
    {
        if (playerModifier != null)
        {
            playerModifier.OnBoonsChanged -= UpdateBoonIcons;
        }
    }

    private void SetupBoonUI()
    {
        // Cari atau buat panel penampung ikon
        Transform containerTransform = transform.Find("BoonIconContainer");
        GameObject containerObj;
        
        if (containerTransform == null)
        {
            containerObj = new GameObject("BoonIconContainer");
            containerObj.layer = LayerMask.NameToLayer("UI");
            containerObj.transform.SetParent(this.transform, false);
            
            // Atur posisi di bawah bar stamina
            RectTransform rect = containerObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(200, -250); // Posisi baru di bawah bar HP/Stamina, geser ke kanan melewati portrait
            rect.sizeDelta = new Vector2(250, 80);

            boonIconContainer = containerObj.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>();
            boonIconContainer.childAlignment = TextAnchor.MiddleLeft;
            boonIconContainer.spacing = 10;
            boonIconContainer.childControlWidth = false;
            boonIconContainer.childControlHeight = false;
        }
        else
        {
            containerObj = containerTransform.gameObject;
            boonIconContainer = containerObj.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>();
        }

        // Buat 2 Slot (0, 1)
        boonIcons.Clear();
        for (int i = 0; i < 2; i++)
        {
            Transform existingSlot = containerObj.transform.Find($"BoonSlot_{i}");
            GameObject slotObj;
            Image iconImg;

            if (existingSlot == null)
            {
                slotObj = new GameObject($"BoonSlot_{i}");
                slotObj.layer = LayerMask.NameToLayer("UI");
                slotObj.transform.SetParent(containerObj.transform, false);
                
                // Background slot kosong
                Image bgImg = slotObj.AddComponent<Image>();
                bgImg.color = new Color(0, 0, 0, 0.5f); // Hitam transparan
                
                RectTransform slotRect = slotObj.GetComponent<RectTransform>();
                slotRect.sizeDelta = new Vector2(60, 60);

                // Child untuk ikon sesungguhnya
                GameObject iconObj = new GameObject("Icon");
                iconObj.layer = LayerMask.NameToLayer("UI");
                iconObj.transform.SetParent(slotObj.transform, false);
                iconImg = iconObj.AddComponent<Image>();
                
                RectTransform iconRect = iconObj.GetComponent<RectTransform>();
                iconRect.anchorMin = Vector2.zero;
                iconRect.anchorMax = Vector2.one;
                iconRect.offsetMin = new Vector2(2, 2); // Padding
                iconRect.offsetMax = new Vector2(-2, -2); // Padding
            }
            else
            {
                slotObj = existingSlot.gameObject;
                iconImg = existingSlot.Find("Icon").GetComponent<Image>();
            }

            // Set transparan saat mulai
            iconImg.color = new Color(1, 1, 1, 0); 
            boonIcons.Add(iconImg);
        }
    }

    private void UpdateBoonIcons()
    {
        if (playerModifier == null) return;

        var activeBoons = playerModifier.GetActiveBoons();
        
        for (int i = 0; i < boonIcons.Count; i++)
        {
            Image img = boonIcons[i];

            if (i < activeBoons.Count && activeBoons[i] != null && activeBoons[i].icon != null)
            {
                img.sprite = activeBoons[i].icon;
                img.color = new Color(1, 1, 1, 1); // Munculkan gambar
            }
            else
            {
                img.sprite = null;
                img.color = new Color(1, 1, 1, 0); // Sembunyikan (transparan)
            }
        }
    }

    void LateUpdate()
    {
        if(playerStats == null)
            return;

        hpFill.fillAmount =
            (float)playerStats.CurrentHP /
            playerStats.MaxHP;

        manaFill.fillAmount =
            (float)playerStats.CurrentMana /
            playerStats.MaxMana;

        staminaFill.fillAmount =
            (float)playerStats.CurrentStamina /
            playerStats.MaxStamina;
    }
}