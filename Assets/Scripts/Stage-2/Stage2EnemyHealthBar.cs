using UnityEngine;
using UnityEngine.UI;

public class Stage2EnemyHealthBar : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject visualContainer;

    [Header("Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private bool hideWhenFull = true;

    private float targetValue;
    private int maxHP;

    /// <summary>
    /// Inisialisasi batas slider berdasarkan HP Maksimal musuh
    /// </summary>
    public void Initialize(int maxHealth)
    {
        maxHP = maxHealth;
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
        targetValue = maxHealth;

        if (hideWhenFull && visualContainer != null)
        {
            visualContainer.SetActive(false);
        }
    }

    /// <summary>
    /// Update target HP
    /// </summary>
    public void SetHealth(int currentHP)
    {
        targetValue = currentHP;

        // Tampilkan bar darah saat musuh terluka
        if (visualContainer != null)
        {
            if (currentHP < maxHP && currentHP > 0)
            {
                visualContainer.SetActive(true);
            }
            else if (currentHP <= 0)
            {
                // Biarkan slider turun dulu, nanti visualContainer dimatikan di Update setelah slider mencapai 0
            }
        }
    }

    private void Update()
    {
        if (slider == null) return;

        // Smooth lerp slider ke nilai target
        slider.value = Mathf.MoveTowards(
            slider.value,
            targetValue,
            smoothSpeed * Time.deltaTime * slider.maxValue
        );

        // Sembunyikan bar darah jika musuh mati (slider sudah mencapai 0)
        if (visualContainer != null && visualContainer.activeSelf)
        {
            if (slider.value <= 0.05f)
            {
                visualContainer.SetActive(false);
            }
        }
    }
}
