using UnityEngine;
using System.Collections.Generic;

public class TrapGenerator : MonoBehaviour
{
    [Header("Trap Setup")]
    public GameObject[] trapPrefabs;

    [Header("Spawn Settings")]
    public int minTrapsPerRoom = 0;
    public int maxTrapsPerRoom = 10;

    private void OnEnable()
    {
        BSPDungeonGenerator.OnDungeonGenerated += SpawnTraps;
    }

    private void OnDisable()
    {
        BSPDungeonGenerator.OnDungeonGenerated -= SpawnTraps;
    }

    private void SpawnTraps()
    {
        BSPDungeonGenerator generator = Object.FindFirstObjectByType<BSPDungeonGenerator>();
        if (generator == null || generator.rooms.Count == 0) return;

        foreach (BSPDungeonGenerator.RoomData room in generator.rooms)
        {
            if (room.isSafeRoom) continue;

            int trapsToSpawn = Random.Range(minTrapsPerRoom, maxTrapsPerRoom + 1);

            trapsToSpawn = Mathf.Min(trapsToSpawn, room.floorTiles.Count / 5);

            for (int i = 0; i < trapsToSpawn; i++)
            {
                if (room.floorTiles.Count == 0) break;

                int randomTileIndex = Random.Range(0, room.floorTiles.Count);
                Vector2Int spawnTile = room.floorTiles[randomTileIndex];

                room.floorTiles.RemoveAt(randomTileIndex);

                Vector3 spawnPos = new Vector3(spawnTile.x * generator.tileSize, 0.02f, spawnTile.y * generator.tileSize);

                GameObject prefabToSpawn = trapPrefabs[Random.Range(0, trapPrefabs.Length)];
                GameObject trapInstance = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
                trapInstance.transform.parent = this.transform;

   
                trapInstance.AddComponent<ColorTrap>();
            }
        }
        Debug.Log("Traps have been placed.");
    }
}