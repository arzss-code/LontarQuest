using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider slider;

    [Header("Animation")]
    [SerializeField] private float smoothSpeed = 6f;

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
}