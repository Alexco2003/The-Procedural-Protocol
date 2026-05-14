using UnityEngine;

public class DayNightManager : MonoBehaviour
{
    public static DayNightManager Instance;

    public enum TimePhase { Day, Normal, Night }

    [Header("Current Phase")]
    public TimePhase currentPhase;
    [Range(0, 24)] public float timeOfDay = 12f;

    [Header("Time Settings")]
    public float timeMultiplier = 0.5f;

    [Header("Lighting Settings")]
    public Light sunLight;
    public Gradient sunColor;
    public Gradient ambientColor;
    public AnimationCurve sunIntensity;

    void Awake()
    {
        if (Instance == null) Instance = this;

        SetupDefaultLighting();
    }

    void Update()
    {
        timeOfDay += Time.deltaTime * timeMultiplier;
        if (timeOfDay >= 24f) timeOfDay %= 24f;

        UpdatePhase();
        UpdateLighting();
    }

    void UpdatePhase()
    {
        if (timeOfDay >= 8f && timeOfDay < 16f)
        {
            currentPhase = TimePhase.Day;
        }
        else if (timeOfDay >= 16f && timeOfDay < 24f)
        {
            currentPhase = TimePhase.Normal;
        }
        else
        {
            currentPhase = TimePhase.Night;
        }
    }

    void UpdateLighting()
    {
        if (sunLight == null) return;

        float timePercent = timeOfDay / 24f;

        sunLight.transform.localRotation = Quaternion.Euler((timePercent * 360f) - 90f, 170f, 0f);

        sunLight.color = sunColor.Evaluate(timePercent);
        sunLight.intensity = sunIntensity.Evaluate(timePercent);

        RenderSettings.ambientLight = ambientColor.Evaluate(timePercent);
    }

    void SetupDefaultLighting()
    {

        if (sunColor == null || sunColor.colorKeys.Length <= 1)
        {
            sunColor = new Gradient();
            GradientColorKey[] colors = new GradientColorKey[5];
            colors[0] = new GradientColorKey(Color.black, 0.0f);               
            colors[1] = new GradientColorKey(new Color(1f, 0.5f, 0f), 0.25f);    
            colors[2] = new GradientColorKey(new Color(1f, 0.9f, 0.6f), 0.5f);   
            colors[3] = new GradientColorKey(new Color(1f, 0.3f, 0f), 0.75f);    
            colors[4] = new GradientColorKey(Color.black, 1.0f);                 

            GradientAlphaKey[] alphas = { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) };
            sunColor.SetKeys(colors, alphas);
        }

        if (ambientColor == null || ambientColor.colorKeys.Length <= 1)
        {
            ambientColor = new Gradient();
            GradientColorKey[] colors = new GradientColorKey[5];
            colors[0] = new GradientColorKey(new Color(0.05f, 0.05f, 0.15f), 0.0f); 
            colors[1] = new GradientColorKey(new Color(0.2f, 0.2f, 0.3f), 0.25f);   
            colors[2] = new GradientColorKey(new Color(0.6f, 0.6f, 0.6f), 0.5f);   
            colors[3] = new GradientColorKey(new Color(0.3f, 0.2f, 0.2f), 0.75f);   
            colors[4] = new GradientColorKey(new Color(0.05f, 0.05f, 0.15f), 1.0f);

            GradientAlphaKey[] alphas = { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) };
            ambientColor.SetKeys(colors, alphas);
        }

        if (sunIntensity == null || sunIntensity.keys.Length <= 1)
        {
            sunIntensity = new AnimationCurve();
            sunIntensity.AddKey(new Keyframe(0.0f, 0.0f));   
            sunIntensity.AddKey(new Keyframe(0.25f, 0.2f));  
            sunIntensity.AddKey(new Keyframe(0.5f, 1.2f));   
            sunIntensity.AddKey(new Keyframe(0.75f, 0.2f));  
            sunIntensity.AddKey(new Keyframe(1.0f, 0.0f));   
        }
    }
}