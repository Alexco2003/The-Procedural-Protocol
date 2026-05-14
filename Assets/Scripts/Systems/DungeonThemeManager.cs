using UnityEngine;

public class DungeonThemeManager : MonoBehaviour
{
    [Header("Dungeon Materials")]
    public Material[] possibleMaterials;

    public static Material currentThemeMaterial;

    private void Awake()
    {
        if (possibleMaterials != null && possibleMaterials.Length > 0)
        {
            int randomIndex = Random.Range(0, possibleMaterials.Length);
            currentThemeMaterial = possibleMaterials[randomIndex];

            Debug.Log($"The dungeon theme material has been set to: {currentThemeMaterial.name}");
        }
        else
        {
            Debug.LogWarning("Warning: No materials have been added to DungeonThemeManager!");
        }
    }
}