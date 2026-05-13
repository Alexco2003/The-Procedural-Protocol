using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Text Inputs (Limited via code)")]
    public TMP_InputField widthInput;
    public TMP_InputField heightInput;
    public TMP_InputField heightMultiplierInput;
    public TMP_InputField noiseScaleInput;
    public TMP_InputField numberOfPlantsInput;
    public TMP_InputField numberOfItemsInput;
    public TMP_InputField dungeonSizeInput;
    public TMP_InputField roomComplexityInput;
    public TMP_InputField roomMinSizeInput;

    [Header("Text Inputs (Seeds)")]
    public TMP_InputField elevationSeedInput;
    public TMP_InputField moistureSeedInput;

    [Header("Sliders (Limited via Unity)")]
    public Slider octavesSlider;
    public Slider persistenceSlider;
    public Slider waterLevelSlider;

    [Header("Scene Settings")]
    public string sceneToLoad = "Scene_2";

    void Start()
    {
        if (widthInput) widthInput.text = TerrainGenerationData.mapWidth.ToString();
        if (heightInput) heightInput.text = TerrainGenerationData.mapHeight.ToString();
        if (heightMultiplierInput) heightMultiplierInput.text = TerrainGenerationData.heightMultiplier.ToString();
        if (noiseScaleInput) noiseScaleInput.text = TerrainGenerationData.noiseScale.ToString();
        if (numberOfPlantsInput) numberOfPlantsInput.text = PlantsGenerationData.numberOfPlants.ToString(); 
        if (numberOfItemsInput) numberOfItemsInput.text = ItemGenerationData.numberOfItems.ToString();
        if (dungeonSizeInput) dungeonSizeInput.text = DungeonData.dungeonSize.ToString();
        if (roomComplexityInput) roomComplexityInput.text = DungeonData.roomComplexity.ToString();
        if (roomMinSizeInput) roomMinSizeInput.text = DungeonData.minRoomSize.ToString();

        if (elevationSeedInput) elevationSeedInput.text = "";
        if (moistureSeedInput) moistureSeedInput.text = "";

        if (octavesSlider) octavesSlider.value = TerrainGenerationData.octaves;
        if (persistenceSlider) persistenceSlider.value = TerrainGenerationData.persistence;
        if (waterLevelSlider) waterLevelSlider.value = TerrainGenerationData.waterLevel;
    }

    public void OnGenerateWorldClicked()
    {
        if (int.TryParse(widthInput.text, out int w)) TerrainGenerationData.mapWidth = Mathf.Clamp(w, 50, 300);
        if (int.TryParse(heightInput.text, out int h)) TerrainGenerationData.mapHeight = Mathf.Clamp(h, 50, 300);

        if (float.TryParse(heightMultiplierInput.text, out float hm)) TerrainGenerationData.heightMultiplier = Mathf.Clamp(hm, 5f, 60f);
        if (float.TryParse(noiseScaleInput.text, out float ns)) TerrainGenerationData.noiseScale = Mathf.Clamp(ns, 0.5f, 15f);

        if (int.TryParse(numberOfPlantsInput.text, out int np)) PlantsGenerationData.numberOfPlants = Mathf.Clamp(np, 10, 200);
        if (int.TryParse(numberOfItemsInput.text, out int ni)) ItemGenerationData.numberOfItems = Mathf.Clamp(ni, 10, 100);
        if (int.TryParse(dungeonSizeInput.text, out int ds)) DungeonData.dungeonSize = Mathf.Clamp(ds, 20, 100);
        if (int.TryParse(roomComplexityInput.text, out int rc)) DungeonData.roomComplexity = 30 - Mathf.Clamp(rc, 9, 25);
        if (int.TryParse(roomMinSizeInput.text, out int rms)) DungeonData.minRoomSize = Mathf.Clamp(rms, 3, 8);
        if (DungeonData.roomComplexity < DungeonData.minRoomSize + 2) DungeonData.roomComplexity = DungeonData.minRoomSize + 2;

        if (float.TryParse(elevationSeedInput.text, out float eSeed)) TerrainGenerationData.elevationSeed = eSeed;
        else TerrainGenerationData.elevationSeed = -1f;

        if (float.TryParse(moistureSeedInput.text, out float mSeed)) TerrainGenerationData.moistureSeed = mSeed;
        else TerrainGenerationData.moistureSeed = -1f;

        if (octavesSlider) TerrainGenerationData.octaves = Mathf.RoundToInt(octavesSlider.value);
        if (persistenceSlider) TerrainGenerationData.persistence = persistenceSlider.value;
        if (waterLevelSlider) TerrainGenerationData.waterLevel = waterLevelSlider.value;

        Debug.Log($"Generating world with updated parameters. " +
                  $"Width: {TerrainGenerationData.mapWidth}, Height: {TerrainGenerationData.mapHeight}, " +
                  $"Height Multiplier: {TerrainGenerationData.heightMultiplier}, Noise Scale: {TerrainGenerationData.noiseScale}, " +
                  $"Elevation Seed: {(TerrainGenerationData.elevationSeed == -1f ? "Random" : TerrainGenerationData.elevationSeed.ToString())}, " +
                  $"Moisture Seed: {(TerrainGenerationData.moistureSeed == -1f ? "Random" : TerrainGenerationData.moistureSeed.ToString())}, " +
                  $"Octaves: {TerrainGenerationData.octaves}, Persistence: {TerrainGenerationData.persistence}, Water Level: {TerrainGenerationData.waterLevel}" +
                  $"Number of Plants: {PlantsGenerationData.numberOfPlants}." +
                  $"Number of Items: {ItemGenerationData.numberOfItems}." +
                  $"Dungeon Size: {DungeonData.dungeonSize}." +
                  $"Room Complexity: {DungeonData.roomComplexity}." +
                  $"Room Min Size: {DungeonData.minRoomSize}." +
                  $"Loading scene: {sceneToLoad}");
        SceneManager.LoadScene(sceneToLoad);
    }
}