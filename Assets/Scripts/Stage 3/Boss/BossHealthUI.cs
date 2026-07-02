using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BossStats bossStats;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider shieldSlider;

    [Header("Animation")]
    [SerializeField] private float smoothSpeed = 8f;

    private float targetHP;
    private float targetShield;

    private void Start()
    {
        if (bossStats == null)
        {
            Debug.LogError("BossStats belum di-assign!");
            enabled = false;
            return;
        }

        if (hpSlider == null || shieldSlider == null)
        {
            Debug.LogError("HP Slider / Shield Slider belum di-assign!");
            enabled = false;
            return;
        }

        Debug.Log("===== BossHealthUI START =====");

        hpSlider.maxValue = bossStats.MaxHP;
        shieldSlider.maxValue = bossStats.MaxShield;

        targetHP = bossStats.CurrentHP;
        targetShield = bossStats.CurrentShield;

        hpSlider.value = targetHP;
        shieldSlider.value = targetShield;

        Debug.Log("HP = " + targetHP);
        Debug.Log("Shield = " + targetShield);

        bossStats.OnHealthChanged += RefreshUI;
    }

    private void OnDestroy()
    {
        if (bossStats != null)
            bossStats.OnHealthChanged -= RefreshUI;
    }

    private void RefreshUI()
    {
        targetHP = bossStats.CurrentHP;
        targetShield = bossStats.CurrentShield;
    }

    private void Update()
    {
        hpSlider.value = Mathf.Lerp(
            hpSlider.value,
            targetHP,
            Time.deltaTime * smoothSpeed);

        shieldSlider.value = Mathf.Lerp(
            shieldSlider.value,
            targetShield,
            Time.deltaTime * smoothSpeed);
    }
}