using UnityEngine;

public class PlantsGenerator : MonoBehaviour
{
    [Header("Plant Prefabs")]
    public GameObject[] plantTypes;

    [Header("Generation Settings")]
    public int numberOfPlants = 50;
    public float mapWidth = 100f;
    public float mapDepth = 100f;

    void Start()
    {
        Invoke("SpawnPlants", 0.5f);
    }

    public void SpawnPlants()
    {
        for (int i = 0; i < numberOfPlants; i++)
        {
            float randomX = Random.Range(0, mapWidth);
            float randomZ = Random.Range(0, mapDepth);

            Vector3 rayStart = new Vector3(randomX, 200f, randomZ);
            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit))
            {
                float waterHeight = TerrainGenerationData.waterLevel * TerrainGenerationData.heightMultiplier;

                if (hit.point.y <= waterHeight + 0.5f)
                {
                    continue;
                }

                GameObject prefabToSpawn = plantTypes[Random.Range(0, plantTypes.Length)];

                Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

                GameObject newPlant = Instantiate(prefabToSpawn, hit.point, randomRotation);
                newPlant.transform.parent = this.transform;

                float scale = Random.Range(0.15f, 0.4f);
                newPlant.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }
}