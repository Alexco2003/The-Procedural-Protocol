using UnityEngine;
using System;

public class PlantsGenerator : MonoBehaviour
{
    [Header("Plant Prefabs")]
    public GameObject[] plantTypes;

    [Header("Generation Settings")]
    public int numberOfPlants = 50;

    private float physicalMapWidth;
    private float physicalMapDepth;

    public static event Action OnWorldGenerated;

    void Start()
    {

        physicalMapWidth = TerrainGenerationData.mapWidth * TerrainGenerationData.meshScale;
        physicalMapDepth = TerrainGenerationData.mapHeight * TerrainGenerationData.meshScale;

        numberOfPlants = PlantsGenerationData.numberOfPlants;

        Invoke("SpawnPlants", 0.5f);
    }

    public void SpawnPlants()
    {

        float padding = 2f;

        for (int i = 0; i < numberOfPlants; i++)
        {
            float randomX = UnityEngine.Random.Range(padding, physicalMapWidth - padding);
            float randomZ = UnityEngine.Random.Range(padding, physicalMapDepth - padding);

            Vector3 rayStart = new Vector3(randomX, 200f, randomZ);

            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit))
            {
                float waterHeight = TerrainGenerationData.waterLevel * TerrainGenerationData.heightMultiplier;

                if (hit.point.y <= waterHeight + 0.5f)
                {
                    continue;
                }

                if (hit.collider.name.StartsWith("Wall_") || hit.collider.name == "GiantWalls")
                {
                    continue;
                }

                GameObject prefabToSpawn = plantTypes[UnityEngine.Random.Range(0, plantTypes.Length)];
                Quaternion randomRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0f, 360f), 0);

                GameObject newPlant = Instantiate(prefabToSpawn, hit.point, randomRotation);
                newPlant.transform.parent = this.transform;

                float scale = UnityEngine.Random.Range(0.15f, 0.4f);
                newPlant.transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        if (OnWorldGenerated != null)
        {
            OnWorldGenerated.Invoke();
        }
    }
}