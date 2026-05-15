using UnityEngine;
using TMPro;

public class UIManager1 : MonoBehaviour
{
    [Header("UI Text References")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI armorText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI dashText;
    public TextMeshProUGUI enemiesDefeatedText;
    public TextMeshProUGUI coinsText;

    private PlayerStats playerStats;
    private PlayerMovement playerMovement;

    void Update()
    {

        if (playerStats == null || playerMovement == null)
        {
            FindPlayerReferences();
            return;
        }

        UpdateUI();
    }

    void FindPlayerReferences()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerStats = player.GetComponent<PlayerStats>();
            playerMovement = player.GetComponent<PlayerMovement>();
        }
    }


    void UpdateUI()
    {
        hpText.text = $"<color=#FF4444>HP:</color> {Mathf.Round(playerStats.currentHP)} / <color=#FF0000>{PlayerData.MaxHP}</color>";

        armorText.text = $"<color=#AAAAAA>Armor:</color> {PlayerData.Armor}";

        damageText.text = $"<color=#FFaa00>Damage:</color> {PlayerData.Damage}";

        speedText.text = $"<color=#00FF00>Speed:</color> {playerMovement.moveSpeed:F1}";

        dashText.text = $"<color=#00FFFF>Dash Force:</color> {playerMovement.dashForce} | <color=#FF00FF>CD:</color> {playerMovement.dashCooldown:F1}s";

        if (enemiesDefeatedText != null && PlayerData.enemiesDefeated != -1)
        {
            enemiesDefeatedText.text = $"<color=#FFA500>Kills:</color> {PlayerData.enemiesDefeated}";
        }

        if (coinsText != null)
        {
            coinsText.text = $"<color=#FFD700>Coins:</color> {PlayerData.coinsCollected}";
        }
    }
}