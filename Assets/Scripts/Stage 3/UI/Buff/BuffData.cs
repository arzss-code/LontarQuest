using UnityEngine;

[CreateAssetMenu(menuName = "Stage3/Buff Data")]
public class BuffData : ScriptableObject
{
    [SerializeField]
    private GameObject cardPrefab;

    public GameObject CardPrefab => cardPrefab;
    public string title;

    public Sprite cardSprite;

    [TextArea]
    public string description;

    public BuffEffects[] effects;
}