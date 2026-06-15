using UnityEngine;

[CreateAssetMenu(fileName = "New Lore", menuName = "LontarQuest/Lore Data")]
public class LoreData : ScriptableObject
{
    [Header("Identitas Entri")]
    [Tooltip("ID unik untuk sistem Save Game, pastikan tidak ada yang sama!")]
    public string loreID;
    
    public string monsterName;
    public Sprite monsterSprite;

    [Header("Informasi Jurnal")]
    [TextArea(4, 10)]
    public string mythologyDescription;
    
    [TextArea(2, 5)]
    public string weaknessHint;
}
