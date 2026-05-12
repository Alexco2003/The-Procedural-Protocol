using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Current Status")]
    public float currentHP;
    public float currentMaxHP;

    void Start()
    {
        currentHP = PlayerData.HP;
        currentMaxHP = PlayerData.MaxHP;
    }

    public void TakeDamage(float amount)
    {
        float reducedDamage = amount - (PlayerData.Armor * 0.1f);
        reducedDamage = Mathf.Max(reducedDamage, 1f);

        currentHP -= reducedDamage;

        PlayerData.HP = currentHP;

        Debug.Log($"Player has taken {reducedDamage} damage. HP left: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player has died.");

    }
}