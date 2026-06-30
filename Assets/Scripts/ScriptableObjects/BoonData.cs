using UnityEngine;

public enum BoonType
{
    Lontara, // Bonus kecepatan (Dash/Attack Speed)
    Batak,   // Bonus pertahanan (Damage Reduction/HP Reg)
    Kawi     // Bonus kekuatan spesial (Elemental/Stamina)
}

public enum BoonSlot
{
    Melee,
    Bow,
    Dash,
    Passive
}

[CreateAssetMenu(fileName = "New Boon", menuName = "LontarQuest/Boon Data")]
public class BoonData : ScriptableObject
{
    [Header("Identitas Boon")]
    public string boonName;
    public BoonType type;
    public BoonSlot slot;
    
    [Tooltip("Level dari Boon ini (1 atau 2)")]
    public int level = 1;
    [Tooltip("Boon referensi untuk level selanjutnya jika pemain sudah punya ini. Kosongkan jika ini Max Level.")]
    public BoonData nextLevelBoon;

    [TextArea(2, 4)]
    public string description;
    public Sprite icon;

    [Header("Statistik Modifier")]
    [Tooltip("Persentase tambahan kecepatan. Misal 0.2 = +20%")]
    public float attackSpeedBonus = 0f;
    public float movementSpeedBonus = 0f;
    
    [Tooltip("Persentase pengurangan damage. Misal 0.1 = -10% damage masuk")]
    public float damageReduction = 0f;
    
    [Tooltip("Jumlah HP regenerasi per detik")]
    public float healthRegen = 0f;

    [Tooltip("Efek spesial (misalnya elemen api atau racun, bisa diakses oleh sistem combat)")]
    public bool hasElementalEffect = false;
    public float extraStamina = 0f;
}
