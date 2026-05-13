using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Current Status")]
    public float currentHP;
    public float currentMaxHP;

    [Header("Combat Settings")]
    public float invulnerabilityTime = 1f;
    private float lastDamageTime = -10f;

    void Start()
    {
        currentHP = PlayerData.HP;
        currentMaxHP = PlayerData.MaxHP;
    }

    public void TakeDamage(float amount)
    {
        if (Time.time < lastDamageTime + invulnerabilityTime)
            return;

        float reducedDamage = amount - (PlayerData.Armor * 0.1f);
        reducedDamage = Mathf.Max(reducedDamage, 1f);

        currentHP -= reducedDamage;
        PlayerData.HP = currentHP;

        lastDamageTime = Time.time;

        Debug.Log($"Player has taken {reducedDamage} damage. Current HP: {currentHP}/{currentMaxHP}");

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