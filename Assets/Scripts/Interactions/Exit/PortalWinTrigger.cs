using UnityEngine;
using TMPro;

public class PortalWinTrigger : MonoBehaviour
{
    [Header("UI & Camera References")]
    public GameObject hudCanvas;
    public GameObject winCanvas;
    public Camera loadingCamera;
    public TextMeshProUGUI statsText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            GameObject playerObj = other.transform.root.gameObject;
            ActivateWinState(playerObj);
        }
    }

    private void ActivateWinState(GameObject player)
    {
        player.SetActive(false);

        if (hudCanvas != null) hudCanvas.SetActive(false);
        if (winCanvas != null) winCanvas.SetActive(true);

        if (loadingCamera != null) loadingCamera.gameObject.SetActive(true);

        if (statsText != null)
        {
            statsText.text = $"<color=#FFA500>Killed Enemies:</color> {PlayerData.enemiesDefeated}\n" +
                             $"<color=#FFD700>Coins Collected:</color> {PlayerData.coinsCollected}";
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Win.");
    }
}