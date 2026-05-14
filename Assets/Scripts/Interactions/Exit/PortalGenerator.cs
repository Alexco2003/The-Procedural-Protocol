using UnityEngine;
using System.Collections.Generic;
using UnityEngine.VFX;
using TMPro;

public class PortalGenerator : MonoBehaviour
{
    [Header("Portal Setup")]
    public GameObject portalPrefab;

    [Header("Win UI References")]
    public GameObject winCanvas;
    public TextMeshProUGUI winStatsText;

    private void OnEnable()
    {
        BSPDungeonGenerator.OnDungeonGenerated += SpawnExitPortal;
    }

    private void OnDisable()
    {
        BSPDungeonGenerator.OnDungeonGenerated -= SpawnExitPortal;
    }

    private void SpawnExitPortal()
    {
        BSPDungeonGenerator generator = Object.FindFirstObjectByType<BSPDungeonGenerator>();
        if (generator == null || generator.rooms.Count <= 1) return;

        List<BSPDungeonGenerator.RoomData> validRooms = new List<BSPDungeonGenerator.RoomData>();
        foreach (var room in generator.rooms)
        {
            if (!room.isSafeRoom && room.floorTiles.Count > 0)
                validRooms.Add(room);
        }

        if (validRooms.Count == 0) return;

        BSPDungeonGenerator.RoomData exitRoom = validRooms[Random.Range(0, validRooms.Count)];
        Vector2Int spawnTile = exitRoom.floorTiles[Random.Range(0, exitRoom.floorTiles.Count)];
        exitRoom.floorTiles.Remove(spawnTile);

        Vector3 spawnPos = new Vector3(spawnTile.x * generator.tileSize, 1f, spawnTile.y * generator.tileSize);
        GameObject portalInstance = Instantiate(portalPrefab, spawnPos, Quaternion.identity);

        Portal oldScript = portalInstance.GetComponent<Portal>();
        if (oldScript != null) Destroy(oldScript);

        PortalWinTrigger winTrigger = portalInstance.AddComponent<PortalWinTrigger>();

        DungeonPlayerSpawner spawner = Object.FindFirstObjectByType<DungeonPlayerSpawner>();
        if (spawner != null)
        {
            winTrigger.hudCanvas = spawner.HUDCanvas;
            winTrigger.loadingCamera = spawner.loadingCamera;
        }

        winTrigger.winCanvas = this.winCanvas;
        winTrigger.statsText = this.winStatsText;

        VisualEffect vfx = portalInstance.GetComponent<VisualEffect>();
        if (vfx != null)
        {
            if (vfx.HasVector4("Particle color"))
            {
                vfx.SetVector4("Particle color", Color.green);
            }
            else if (vfx.HasVector4("ParticleColor"))
            {
                vfx.SetVector4("ParticleColor", Color.green);
            }
            else
            {
                vfx.SetVector4("Particle color", Color.green);
            }
        }
    }
}