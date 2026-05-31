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