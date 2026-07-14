using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider slider;

    [Header("Animation")]
    [SerializeField] private float smoothSpeed = 6f;

    [Header("Settings")]
    [SerializeField] private bool billboard = true;

    //------------------------------------------------

    private float targetValue;

    //------------------------------------------------

    public void Initialize(int maxHP)
    {
        slider.maxValue = maxHP;
        slider.value = maxHP;

        targetValue = maxHP;
    }

    //------------------------------------------------

    public void SetHealth(int currentHP)
    {
        targetValue = currentHP;
    }

    //------------------------------------------------

    private void Update()
    {
        slider.value = Mathf.MoveTowards(
            slider.value,
            targetValue,
            smoothSpeed * Time.deltaTime * slider.maxValue);
    }

    private void LateUpdate()
    {
        // Billboard: Menjaga UI selalu menghadap kamera (tidak terpengaruh rotasi musuh)
        if (billboard && Camera.main != null)
        {
            transform.rotation = Camera.main.transform.rotation;
        }

        // Anti-Mirror: Mencegah UI terbalik jika parent di-flip menggunakan scale negatif
        if (transform.parent != null)
        {
            Vector3 localScale = transform.localScale;
            float parentScaleX = transform.parent.lossyScale.x;

            // Jika scale global parent negatif tapi scale lokal UI positif (atau sebaliknya),
            // balikkan scale lokal UI agar visual teks/bar tidak terbalik (mirror).
            if ((parentScaleX < 0f && localScale.x > 0f) || (parentScaleX > 0f && localScale.x < 0f))
            {
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
        }
    }
}