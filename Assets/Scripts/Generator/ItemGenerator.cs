using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    [Header("Generation Setup")]
    public int numberOfItemsToSpawn = 30;
    private float physicalMapWidth;
    private float physicalMapDepth;

    [Header("Item Prefabs By Type")]
    public GameObject[] axePrefabs;
    public GameObject[] bowPrefabs;
    public GameObject[] macePrefabs;
    public GameObject[] shieldPrefabs;
    public GameObject[] spearPrefabs;
    public GameObject[] staffPrefabs;
    public GameObject[] wandPrefabs;

    public List<string> itemPrefixes = new List<string>
    {
        "Rusty", "Shining", "Cursed", "Ancient", "Glimmering", "Broken",
        "Bronze", "Iron", "Steel", "Silver", "Gold", "Mithril", "Obsidian",
        "Wooden", "Leather", "Heavy", "Light", "Masterwork", "Mystic", "Savage"
    };

    private void OnEnable()
    {
        BiomeGenerator.OnTerrainGenerated += SpawnWorldItems;
    }

    private void OnDisable()
    {
        BiomeGenerator.OnTerrainGenerated -= SpawnWorldItems;
    }

    void Start()
    {
        physicalMapWidth = TerrainGenerationData.mapWidth * TerrainGenerationData.meshScale;
        physicalMapDepth = TerrainGenerationData.mapHeight * TerrainGenerationData.meshScale;
    }

    private void SpawnWorldItems()
    {
        float padding = 5f;
        int maxAttempts = 200;

        for (int i = 0; i < numberOfItemsToSpawn; i++)
        {
            int attempts = 0;
            bool spawned = false;

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
                    if (hit.point.y <= waterHeight + 0.5f) continue;

                    SpawnSingleItem(hit.point);
                    spawned = true;
                }
            }
        }
    }

    private void SpawnSingleItem(Vector3 spawnPoint)
    {
        ItemType randomType = (ItemType)Random.Range(0, System.Enum.GetValues(typeof(ItemType)).Length);
        GameObject prefabToSpawn = GetRandomPrefabForType(randomType);

        if (prefabToSpawn == null) return;

        Vector3 finalPos = spawnPoint + Vector3.up * 1f;
        Quaternion randomRot = Quaternion.Euler(Random.Range(-30f, 30f), Random.Range(0f, 360f), Random.Range(-30f, 30f));

        GameObject itemInstance = Instantiate(prefabToSpawn, finalPos, randomRot);
        itemInstance.transform.parent = this.transform;

        ItemData data = GenerateLogicalStats(randomType);

        Color randomTint = new Color(Random.value, Random.value, Random.value, 1f);
        MeshRenderer[] renderers = itemInstance.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in renderers)
        {
            if (r.material.HasProperty("_BaseColor"))
                r.material.SetColor("_BaseColor", randomTint);
            else if (r.material.HasProperty("_Color"))
                r.material.color = randomTint;
        }

        CollectibleItem logicScript = itemInstance.AddComponent<CollectibleItem>();
        logicScript.data = data;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) logicScript.player = playerObj.transform;
    }

    private ItemData GenerateLogicalStats(ItemType type)
    {
        ItemData item = new ItemData();
        item.type = type;
        item.rarity = GetRandomRarity();
        item.itemName = $"{itemPrefixes[Random.Range(0, itemPrefixes.Count)]} {type}";

        float mult = 1f;
        if (item.rarity == ItemRarity.Uncommon) mult = 2f;
        if (item.rarity == ItemRarity.Rare) mult = 4f;
        if (item.rarity == ItemRarity.Epic) mult = 7f;
        if (item.rarity == ItemRarity.Legendary) mult = 12f;

        List<int> availableStats = new List<int> { 0, 1, 2, 3, 4, 5 };

        switch (type)
        {
            case ItemType.Shield:
                item.armorBoost = Mathf.Round(Random.Range(2f, 4f) * mult);
                item.maxHpBoost = Mathf.Round(Random.Range(10f, 20f) * mult);
                availableStats.Remove(1);
                availableStats.Remove(0);
                break;
            case ItemType.Axe:
            case ItemType.Mace:
                item.damageBoost = Mathf.Round(Random.Range(3f, 6f) * mult);
                item.maxHpBoost = Mathf.Round(Random.Range(5f, 10f) * mult);
                availableStats.Remove(2);
                availableStats.Remove(0);
                break;
            case ItemType.Spear:
                item.damageBoost = Mathf.Round(Random.Range(3f, 5f) * mult);
                item.dashForceBoost = Mathf.Round(Random.Range(1f, 3f) * mult);
                availableStats.Remove(2);
                availableStats.Remove(4);
                break;
            case ItemType.Bow:
                item.damageBoost = Mathf.Round(Random.Range(2f, 4f) * mult);
                item.moveSpeedBoost = Mathf.Round(Random.Range(0.2f, 0.5f) * mult * 10f) / 10f;
                availableStats.Remove(2);
                availableStats.Remove(3);
                break;
            case ItemType.Staff:
            case ItemType.Wand:
                item.damageBoost = Mathf.Round(Random.Range(2f, 5f) * mult);
                item.dashCooldownReduction = Mathf.Round(Random.Range(0.05f, 0.1f) * mult * 100f) / 100f;
                availableStats.Remove(2);
                availableStats.Remove(5);
                break;
        }

        int extraBoosts = 0;
        if (item.rarity == ItemRarity.Epic) extraBoosts = 1;
        if (item.rarity == ItemRarity.Legendary) extraBoosts = 2;

        for (int i = 0; i < extraBoosts; i++)
        {
            if (availableStats.Count == 0) break;

            int randomIndex = Random.Range(0, availableStats.Count);
            int chosenStat = availableStats[randomIndex];

            availableStats.RemoveAt(randomIndex);

            ApplySpecificBonusStat(item, chosenStat, mult);
        }

        return item;
    }

    private void ApplySpecificBonusStat(ItemData item, int statIndex, float mult)
    {
        switch (statIndex)
        {
            case 0:
                item.maxHpBoost += Mathf.Round(Random.Range(5f, 15f) * mult);
                break;
            case 1:
                item.armorBoost += Mathf.Round(Random.Range(1f, 3f) * mult);
                break;
            case 2:
                item.damageBoost += Mathf.Round(Random.Range(2f, 5f) * mult);
                break;
            case 3:
                item.moveSpeedBoost += Mathf.Round(Random.Range(0.1f, 0.3f) * mult * 10f) / 10f;
                break;
            case 4:
                item.dashForceBoost += Mathf.Round(Random.Range(0.5f, 1.5f) * mult);
                break;
            case 5:
                item.dashCooldownReduction += Mathf.Round(Random.Range(0.02f, 0.05f) * mult * 100f) / 100f;
                break;
        }

    }

    private ItemRarity GetRandomRarity()
    {
        float roll = Random.Range(0f, 10f);
        if (roll <= 3f) return ItemRarity.Legendary;
        if (roll <= 10f) return ItemRarity.Epic;
        if (roll <= 25f) return ItemRarity.Rare;
        if (roll <= 50f) return ItemRarity.Uncommon;
        return ItemRarity.Common;
    }

    private GameObject GetRandomPrefabForType(ItemType type)
    {
        switch (type)
        {
            case ItemType.Axe: return axePrefabs.Length > 0 ? axePrefabs[Random.Range(0, axePrefabs.Length)] : null;
            case ItemType.Bow: return bowPrefabs.Length > 0 ? bowPrefabs[Random.Range(0, bowPrefabs.Length)] : null;
            case ItemType.Mace: return macePrefabs.Length > 0 ? macePrefabs[Random.Range(0, macePrefabs.Length)] : null;
            case ItemType.Shield: return shieldPrefabs.Length > 0 ? shieldPrefabs[Random.Range(0, shieldPrefabs.Length)] : null;
            case ItemType.Spear: return spearPrefabs.Length > 0 ? spearPrefabs[Random.Range(0, spearPrefabs.Length)] : null;
            case ItemType.Staff: return staffPrefabs.Length > 0 ? staffPrefabs[Random.Range(0, staffPrefabs.Length)] : null;
            case ItemType.Wand: return wandPrefabs.Length > 0 ? wandPrefabs[Random.Range(0, wandPrefabs.Length)] : null;
            default: return null;
        }
    }
}