using UnityEngine;
using System.Collections.Generic;

public class CoinGenerator : MonoBehaviour
{
    [Header("Coin Setup")]
    public GameObject coinPrefab;

    [Header("Spawn Settings")]
    public int minCoinsPerRoom = 10;
    public int maxCoinsPerRoom = 50;

    private void Start()
    {
        minCoinsPerRoom = CoinGenerationData.minCoinsPerRoom;
        maxCoinsPerRoom = CoinGenerationData.maxCoinsPerRoom;
    }
    private void OnEnable()
    {
        BSPDungeonGenerator.OnDungeonGenerated += SpawnCoins;
    }

    private void OnDisable()
    {
        BSPDungeonGenerator.OnDungeonGenerated -= SpawnCoins;
    }

    private void SpawnCoins()
    {
        BSPDungeonGenerator generator = Object.FindFirstObjectByType<BSPDungeonGenerator>();
        if (generator == null || generator.rooms.Count == 0) return;

        foreach (BSPDungeonGenerator.RoomData room in generator.rooms)
        { 
            // if (room.isSafeRoom) continue; 

            int coinsToSpawn = Random.Range(minCoinsPerRoom, maxCoinsPerRoom + 1);
            coinsToSpawn = Mathf.Min(coinsToSpawn, room.floorTiles.Count);

            for (int i = 0; i < coinsToSpawn; i++)
            {
                if (room.floorTiles.Count == 0) break;

                int randomTileIndex = Random.Range(0, room.floorTiles.Count);
                Vector2Int spawnTile = room.floorTiles[randomTileIndex];
                room.floorTiles.RemoveAt(randomTileIndex);

                Vector3 spawnPos = new Vector3(spawnTile.x * generator.tileSize, 1f, spawnTile.y * generator.tileSize);

                GameObject coinInstance = Instantiate(coinPrefab, spawnPos, Quaternion.identity);
                coinInstance.transform.parent = this.transform;

                if (coinInstance.GetComponent<CoinController>() == null)
                {
                    coinInstance.AddComponent<CoinController>();
                }
            }
        }
        Debug.Log("Coins spawned in all rooms.");
    }
}