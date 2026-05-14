using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WinMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI statsText;

    public void ContinueToMainMenu()
    {
        PlayerData.ResetStats();
        SceneManager.LoadScene("Scene_1");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}