using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string itemName;
    public ItemType type;
    public ItemRarity rarity;

    // --- BOOSTS ---
    public float hpBoost;
    public float maxHpBoost;
    public float armorBoost;
    public float damageBoost;

    public float moveSpeedBoost;
    public float dashForceBoost;
    public float dashCooldownReduction;

    public ItemData() { }
}