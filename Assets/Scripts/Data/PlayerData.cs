using UnityEngine;

public static class PlayerData
{

    public static float HP = 100f;
    public static float MaxHP = 100f;
    public static float Armor = 10f;
    public static float Damage = 20f;

    public static void ResetStats()
    {
        HP = 100f;
        MaxHP = 100f;
        Armor = 10f;
        Damage = 20f;

    }
}