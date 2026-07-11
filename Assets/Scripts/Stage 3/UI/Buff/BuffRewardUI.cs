using System.Collections.Generic;
using UnityEngine;

public class BuffRewardUI : MonoBehaviour
{
    public static BuffRewardUI Instance;

    [Header("References")]
    [SerializeField] private BuffDatabase database;
    [SerializeField] private Transform cardHolder;
    [SerializeField] private GameObject rewardUIRoot;

    private void Awake()
    {
        Instance = this;

        if (rewardUIRoot != null)
            rewardUIRoot.SetActive(false);
    }

    public void ShowReward()
    {
        if (rewardUIRoot != null)
            rewardUIRoot.SetActive(true);

        SpawnRandomCards();

        Time.timeScale = 0f;
    }

    public void HideReward()
    {
        if (rewardUIRoot != null)
            rewardUIRoot.SetActive(false);

        Time.timeScale = 1f;
    }

    private void SpawnRandomCards()
    {
        foreach (Transform child in cardHolder)
            Destroy(child.gameObject);

        List<BuffData> availableBuffs =
            new List<BuffData>(database.allBuffs);

        int amount = Mathf.Min(3, availableBuffs.Count);

        for (int i = 0; i < amount; i++)
        {
            int randomIndex = Random.Range(0, availableBuffs.Count);

            BuffData data = availableBuffs[randomIndex];

            availableBuffs.RemoveAt(randomIndex);

            GameObject card =
                Instantiate(data.CardPrefab, cardHolder);

            CardBuffUI ui = card.GetComponent<CardBuffUI>();

            if (ui != null)
                ui.Setup(data);
        }
    }
}