using UnityEngine;
using UnityEngine.UI;

public class CardBuffUI : MonoBehaviour
{
    public BuffData buffData;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button != null)
            button.onClick.AddListener(OnSelected);
    }

    void OnSelected()
    {
        Stage3PlayerModifier.Instance.Apply(buffData);

        BuffRewardUI.Instance.HideReward();

        Debug.Log("Applied : " + buffData.title);
    }

    public void Setup(BuffData data)
    {
        buffData = data;
    }
}