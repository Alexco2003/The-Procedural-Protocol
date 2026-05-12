using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject cameraHolderPrefab;

    [Header("Loading Screen Setup")]
    public GameObject loadingCanvas;
    public GameObject HUDCanvas;
    public Camera loadingCamera;


    private float physicalMapWidth;
    private float physicalMapDepth;

    private bool isTerrainReady = false;
    private bool arePlantsReady = false;

    private void OnEnable()
    {
        BiomeGenerator.OnTerrainGenerated += TerrainFinished;
        PlantsGenerator.OnPlantsGenerated += PlantsFinished;
    }

    private void OnDisable()
    {
        BiomeGenerator.OnTerrainGenerated -= TerrainFinished;
        PlantsGenerator.OnPlantsGenerated -= PlantsFinished;
    }

    void Start()
    {

        if (loadingCanvas != null) loadingCanvas.SetActive(true);
        if (HUDCanvas != null) HUDCanvas.SetActive(false);
        if (loadingCamera != null) loadingCamera.gameObject.SetActive(true);

        physicalMapWidth = TerrainGenerationData.mapWidth * TerrainGenerationData.meshScale;
        physicalMapDepth = TerrainGenerationData.mapHeight * TerrainGenerationData.meshScale;
    }

    private void PlantsFinished()
    {
        arePlantsReady = true;
        Debug.Log("Plants generation complete.");
        CheckIfWorldIsReady();
    }

    private void TerrainFinished()
    {
        isTerrainReady = true;
        Debug.Log("Terrain generation complete.");
        CheckIfWorldIsReady();
    }

    private void CheckIfWorldIsReady()
    {
        if (isTerrainReady && arePlantsReady)
        {
            Debug.Log("World generation complete. Spawning player and camera.");
            SpawnPlayerAndCamera();
        }
    }

    void SpawnPlayerAndCamera()
    {
        bool spawned = false;
        float padding = 5f;
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

                Vector3 spawnPos = hit.point + Vector3.up * 2f;
                GameObject playerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

                GameObject camHolderInstance = Instantiate(cameraHolderPrefab, spawnPos, Quaternion.identity);

                MoveCamera moveCamScript = camHolderInstance.GetComponent<MoveCamera>();
                moveCamScript.cameraPosition = playerInstance.transform.Find("CameraPos");

                PlayerCam pCamScript = camHolderInstance.GetComponentInChildren<PlayerCam>();
                pCamScript.orientation = playerInstance.transform.Find("Orientation");

                spawned = true;
            }
        }

        if (!spawned)
        {
            Debug.LogError("Failed to spawn player after " + maxAttempts + " attempts. Please check the terrain and ensure there are valid spawn points.");
        }

        if (loadingCanvas != null) loadingCanvas.SetActive(false);
        if (HUDCanvas != null) HUDCanvas.SetActive(true);
        if (loadingCamera != null) loadingCamera.gameObject.SetActive(false);
    }
}