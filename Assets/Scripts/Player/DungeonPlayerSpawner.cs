using UnityEngine;

public class DungeonPlayerSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject cameraHolderPrefab;

    [Header("Loading Screen Setup")]
    public GameObject loadingCanvas;
    public GameObject HUDCanvas;
    public Camera loadingCamera;
    public float loadingScreenDelay = 2.5f;

    [Header("Game Over Setup")]
    public GameObject gameOverCanvas;

    private bool isDungeonReady = false;
    // private bool areEnemiesReady = false; 
    // private bool areItemsReady = false;   

    public void Start()
    {
        PlayerData.enemiesDefeated = 0;
    }

    private void OnEnable()
    {
        BSPDungeonGenerator.OnDungeonGenerated += DungeonFinished;
    }

    private void OnDisable()
    {
        BSPDungeonGenerator.OnDungeonGenerated -= DungeonFinished;
    }

    void Awake()
    {
        if (loadingCanvas != null) loadingCanvas.SetActive(true);
        if (HUDCanvas != null) HUDCanvas.SetActive(false);
        if (loadingCamera != null) loadingCamera.gameObject.SetActive(true);
    }

    private void DungeonFinished()
    {
        isDungeonReady = true;
        Debug.Log("Dungeon generated.");
        CheckIfWorldIsReady();
    }


    private void CheckIfWorldIsReady()
    {
        if (isDungeonReady)
        {
            Debug.Log("All Systems Init. Spawning player and camera...");
            SpawnPlayerAndCamera();
        }
    }

    void SpawnPlayerAndCamera()
    {
        BSPDungeonGenerator generator = Object.FindFirstObjectByType<BSPDungeonGenerator>();

        if (generator == null || generator.rooms.Count == 0)
        {
            Debug.LogError("Dungeon generator not found or no rooms generated! Cannot spawn player.");
            return;
        }

        int randomIndex = Random.Range(0, generator.rooms.Count);
        BSPDungeonGenerator.RoomData safeRoom = generator.rooms[randomIndex];
        safeRoom.isSafeRoom = true;

        Vector2Int spawnTile = safeRoom.centerTile;

        safeRoom.floorTiles.Remove(spawnTile);

        Vector3 spawnPos = new Vector3(spawnTile.x * generator.tileSize, 2f, spawnTile.y * generator.tileSize);

        GameObject playerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

        PlayerStats pStats = playerInstance.GetComponent<PlayerStats>();
        if (pStats != null)
        {
            pStats.hudCanvas = HUDCanvas;
            pStats.gameOverCanvas = gameOverCanvas;
            pStats.loadingCamera = loadingCamera;
        }

        GameObject camHolderInstance = Instantiate(cameraHolderPrefab, spawnPos, Quaternion.identity);

        MoveCamera moveCamScript = camHolderInstance.GetComponent<MoveCamera>();
        moveCamScript.cameraPosition = playerInstance.transform.Find("CameraPos");

        PlayerCam pCamScript = camHolderInstance.GetComponentInChildren<PlayerCam>();
        pCamScript.orientation = playerInstance.transform.Find("Orientation");

        Invoke("FinishLoadingScreen", loadingScreenDelay);
    }

    void FinishLoadingScreen()
    {
        if (loadingCanvas != null) loadingCanvas.SetActive(false);
        if (HUDCanvas != null) HUDCanvas.SetActive(true);
        if (loadingCamera != null) loadingCamera.gameObject.SetActive(false);
        Debug.Log("Loading over.");
    }
}