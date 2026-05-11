using UnityEngine;

public class PortalSpawner : MonoBehaviour
{
    public GameObject portalPrefab;

    private float physicalMapWidth;
    private float physicalMapDepth;

    private void OnEnable()
    {
        BiomeGenerator.OnTerrainGenerated += SpawnPortal;
    }

    private void OnDisable()
    {
        BiomeGenerator.OnTerrainGenerated -= SpawnPortal;
    }

    void Start()
    {
        physicalMapWidth = TerrainGenerationData.mapWidth * TerrainGenerationData.meshScale;
        physicalMapDepth = TerrainGenerationData.mapHeight * TerrainGenerationData.meshScale;
    }

    void SpawnPortal()
    {
        bool spawned = false;
        float padding = 15f;
        int maxAttempts = 100;
        int attempts = 0;

        while (!spawned && attempts < maxAttempts)
        {
            attempts++;
            float randomX = Random.Range(padding, physicalMapWidth - padding);
            float randomZ = Random.Range(padding, physicalMapDepth - padding);

            Vector3 rayStart = new Vector3(randomX, 200f, randomZ);
            LayerMask groundLayer = LayerMask.GetMask("Ground");

            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 300f, groundLayer))
            {
                float waterHeight = TerrainGenerationData.waterLevel * TerrainGenerationData.heightMultiplier;

                if (hit.point.y <= waterHeight + 0.5f)
                    continue;

                Vector3 spawnPos = hit.point + Vector3.up * 1.5f;
                Instantiate(portalPrefab, spawnPos, Quaternion.identity);

                spawned = true;
                Debug.Log("Portal spawned at: " + spawnPos);
            }
        }

        if (!spawned)
        {
            Debug.LogWarning("Could not spawn portal!");
        }
    }
}