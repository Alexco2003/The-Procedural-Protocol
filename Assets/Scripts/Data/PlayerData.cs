using UnityEngine;

public static class PlayerData
{

    public static float HP = 100f;
    public static float MaxHP = 100f;
    public static float Armor = 10f;
    public static float Damage = 20f;

    public static float MoveSpeed = 7f;
    public static float DashForce = 30f;
    public static float DashCooldown = 3.5f;

    public static int enemiesDefeated = 0;
    public static int coinsCollected = 0;

    public static void ResetStats()
    {
        HP = 100f;
        MaxHP = 100f;
        Armor = 10f;
        Damage = 20f;

        MoveSpeed = 7f;
        DashForce = 30f;
        DashCooldown = 3.5f;
        coinsCollected = 0;

    }
}