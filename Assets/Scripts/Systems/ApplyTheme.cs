using UnityEngine;

public class ApplyTheme : MonoBehaviour
{
    void Start()
    {
        if (DungeonThemeManager.currentThemeMaterial != null)
        {
            MeshRenderer rend = GetComponent<MeshRenderer>();

            if (rend != null)
            {
                rend.material = DungeonThemeManager.currentThemeMaterial;
            }
        }
    }
}