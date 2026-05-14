using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Current Status")]
    public float currentHP;
    public float currentMaxHP;

    [Header("Combat Settings")]
    public float invulnerabilityTime = 1f;
    private float lastDamageTime = -10f;

    [Header("Game Over UI")]
    public GameObject hudCanvas;
    public GameObject gameOverCanvas;
    public Camera loadingCamera;

    [Header("Auto-Heal Settings")]
    public bool enableAutoHeal = true;
    public float healAmountPerSecond = 2f;
    public float healDelayAfterDamage = 5f;

    void Start()
    {
        currentHP = PlayerData.MaxHP;
        currentMaxHP = PlayerData.MaxHP;
    }

    void Update()
    {

        if (enableAutoHeal && currentHP > 0 && currentHP < currentMaxHP)
        {
            if (Time.time >= lastDamageTime + healDelayAfterDamage)
            {
                currentHP += healAmountPerSecond * Time.deltaTime;

                if (currentHP > currentMaxHP)
                {
                    currentHP = currentMaxHP;
                }

                PlayerData.HP = currentHP;
            }
        }
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

        if (hudCanvas != null) hudCanvas.SetActive(false);

        if (gameOverCanvas != null) gameOverCanvas.SetActive(true);

        if (loadingCamera != null) loadingCamera.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        gameObject.SetActive(false);
    }
}