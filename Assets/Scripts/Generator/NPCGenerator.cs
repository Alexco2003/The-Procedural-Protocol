using System.Collections.Generic;
using UnityEngine;

public class NPCGenerator : MonoBehaviour
{
    [Header("Enemy Prefabs By Class")]
    public GameObject[] warriorPrefabs;
    public GameObject[] paladinPrefabs;
    public GameObject[] assassinPrefabs;
    public GameObject[] roguePrefabs;

    [Header("Spawn Settings")]
    public int minEnemiesPerRoom = 1;
    public int maxEnemiesPerRoom = 3;

    private List<string> firstNames = new List<string>
    {
        "Kael", "Lyra", "Grom", "Sylas", "Elara", "Thorin", "Aria", "Fenrir", "Darius", "Vex"
    };

    private List<string> lastNames = new List<string>
    {
        "Shadow", "Whisper", "Iron", "Blood", "Stone", "Storm", "Swift", "Gloom"
    };

    private static int nextNpcId = 1;

    private void OnEnable()
    {
        BSPDungeonGenerator.OnDungeonGenerated += SpawnEnemiesInDungeon;
    }

    private void OnDisable()
    {
        BSPDungeonGenerator.OnDungeonGenerated -= SpawnEnemiesInDungeon;
    }

    private void SpawnEnemiesInDungeon()
    {
        BSPDungeonGenerator generator = Object.FindFirstObjectByType<BSPDungeonGenerator>();
        if (generator == null || generator.rooms.Count == 0) return;

        foreach (BSPDungeonGenerator.RoomData room in generator.rooms)
        {

            if (room.isSafeRoom) continue;

            int enemiesToSpawn = Random.Range(minEnemiesPerRoom, maxEnemiesPerRoom + 1);

            enemiesToSpawn = Mathf.Min(enemiesToSpawn, room.floorTiles.Count);

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                NPCData npcData = GenerateRandomNPC();
                GameObject prefab = GetPrefabForClass(npcData.npcClass);

                if (prefab == null) continue;

                int randomTileIndex = Random.Range(0, room.floorTiles.Count);
                Vector2Int spawnTile = room.floorTiles[randomTileIndex];

                room.floorTiles.RemoveAt(randomTileIndex);

                Vector3 spawnPos = new Vector3(spawnTile.x * generator.tileSize, 1.5f, spawnTile.y * generator.tileSize);

                GameObject npcInstance = Instantiate(prefab, spawnPos, Quaternion.identity);
                npcInstance.transform.parent = this.transform;

                NPCController controller = npcInstance.AddComponent<NPCController>();
                controller.data = npcData;
            }
        }

        Debug.Log("Enemies spawned in dungeon.");
    }

    public NPCData GenerateRandomNPC()
    {
        NPCData newNPC = new NPCData();
        newNPC.id = nextNpcId++;
        newNPC.npcName = $"{firstNames[Random.Range(0, firstNames.Count)]} {lastNames[Random.Range(0, lastNames.Count)]}";

        newNPC.npcClass = (NPCClass)Random.Range(0, System.Enum.GetValues(typeof(NPCClass)).Length);

        AssignBaseStats(newNPC);
        return newNPC;
    }

    private void AssignBaseStats(NPCData npc)
    {
        switch (npc.npcClass)
        {
            case NPCClass.Warrior: // Balanced: DMG, AR, SPD, HP
                npc.hp = Mathf.RoundToInt(Random.Range(100f, 150f));
                npc.damage = Mathf.RoundToInt(Random.Range(15f, 25f));
                npc.armor = Mathf.RoundToInt(Random.Range(10f, 20f));
                npc.moveSpeed = Mathf.Round(Random.Range(3f, 4.5f) * 10f) / 10f;
                break;
            case NPCClass.Paladin: // Tank: High HP, High AR, Low DMG, Low SPD
                npc.hp = Mathf.RoundToInt(Random.Range(180f, 250f));
                npc.damage = Mathf.RoundToInt(Random.Range(8f, 15f));
                npc.armor = Mathf.RoundToInt(Random.Range(25f, 40f));
                npc.moveSpeed = Mathf.Round(Random.Range(2f, 3f) * 10f) / 10f;
                break;
            case NPCClass.Assassin: // Glass Cannon: Low HP, Low AR, High DMG, High SPD
                npc.hp = Mathf.RoundToInt(Random.Range(50f, 80f));
                npc.damage = Mathf.RoundToInt(Random.Range(35f, 55f));
                npc.armor = Mathf.RoundToInt(Random.Range(2f, 5f));
                npc.moveSpeed = Mathf.Round(Random.Range(6f, 8.5f) * 10f) / 10f;
                break;
            case NPCClass.Rogue: // Skirmisher: Med HP, Low AR, Med/High DMG, High SPD
                npc.hp = Mathf.RoundToInt(Random.Range(80f, 110f));
                npc.damage = Mathf.RoundToInt(Random.Range(20f, 35f));
                npc.armor = Mathf.RoundToInt(Random.Range(5f, 12f));
                npc.moveSpeed = Mathf.Round(Random.Range(5f, 6.5f) * 10f) / 10f;
                break;
        }
        npc.maxHp = npc.hp;
    }

    private GameObject GetPrefabForClass(NPCClass npcClass)
    {
        switch (npcClass)
        {
            case NPCClass.Warrior: return warriorPrefabs.Length > 0 ? warriorPrefabs[Random.Range(0, warriorPrefabs.Length)] : null;
            case NPCClass.Paladin: return paladinPrefabs.Length > 0 ? paladinPrefabs[Random.Range(0, paladinPrefabs.Length)] : null;
            case NPCClass.Assassin: return assassinPrefabs.Length > 0 ? assassinPrefabs[Random.Range(0, assassinPrefabs.Length)] : null;
            case NPCClass.Rogue: return roguePrefabs.Length > 0 ? roguePrefabs[Random.Range(0, roguePrefabs.Length)] : null;
            default: return null;
        }
    }
}