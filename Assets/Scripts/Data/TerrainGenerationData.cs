using UnityEngine;
public static class TerrainGenerationData
{
    // parameters for terrain generation
    public static int mapWidth = 100;
    public static int mapHeight = 100;
    public static float meshScale = 1f;
    public static float heightMultiplier = 35f;
    public static float waterLevel = 0.35f;

    public static float noiseScale = 1.8f;
    public static int octaves = 6;
    public static float persistence = 0.5f;
    public static float lacunarity = 2f;

    public static float elevationSeed = -1f;
    public static float moistureSeed = -1f;
}