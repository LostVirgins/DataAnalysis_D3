using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls Heatmap controller GUI in Editor window
/// </summary>
[CustomEditor(typeof(HeatmapController))]
public class Heatmap_UI : Editor
{
    private HeatmapController heatmapController;

    private SerializedObject serializedGradient;

    string fileName;

    public override void OnInspectorGUI()
    {
        heatmapController = (HeatmapController)target;

        AddMethodButtons();
        
        GUI.enabled = true; // Prevent buttons 'enabled' setting to affect other elements below them

        AddEventsSelect();
        AddNormalSettings();
        AddAdvancedSettings();

        // gradient changes must be applied at the end of OnInspectorGUI
        serializedGradient.ApplyModifiedProperties();
    }

    private void AddMethodButtons()
    {
        GUILayout.Label("\nActions", EditorStyles.boldLabel);

        AddLoadEventsButton();
        AddInitializeParticleSystemButton();
        AddGenerateHeatmapButton();
        AddResetHeatmapButton();
    }

    private void AddLoadEventsButton()
    {
        GUI.enabled = heatmapController.IsLoadEventsActive();
        if (GUILayout.Button(new GUIContent("Load events from file")))
        {
            if (Application.isPlaying)
                heatmapController.LoadEvents();
        }
    }

    private void AddResetHeatmapButton()
    {
        GUI.enabled = heatmapController.IsResetHeatmapActive();
        if (GUILayout.Button(new GUIContent("Reset heatmap values", "Resets heatmap color values to default values")))
        {
            if (Application.isPlaying)
                heatmapController.ResetHeatmap();
        }
    }

    private void AddInitializeParticleSystemButton()
    {
        GUI.enabled = heatmapController.IsInitializeParticleSystemActive();
        if (GUILayout.Button(new GUIContent("Initialize particle system", "Initializes particle system and prepares particle array")))
        {
            if (Application.isPlaying)
                heatmapController.InitializeParticleSystem();
        }
    }

    private void AddGenerateHeatmapButton()
    {
        GUI.enabled = heatmapController.IsAddEventToHeatMapActive();
        if (GUILayout.Button(new GUIContent("Generate heatmap", "Calculates color of particles in particle system using data from selected events")))
        {
            if (Application.isPlaying)
                heatmapController.AddSelectedEventsToHeatmap();
        }
    }

    private void AddEventsSelect()
    {
        GUILayout.Label("\nEvents", EditorStyles.boldLabel);
        GUILayout.Label("Choose events to visualize on heatmap\n");

        if (heatmapController.events != null)
        {
            foreach (EventData eventData in heatmapController.events)
                eventData.m_isVisible = EditorGUILayout.Toggle(eventData.m_eventName, eventData.m_isVisible);
        }
    }

    private void AddNormalSettings()
    {
        GUILayout.Label("\nSettings", EditorStyles.boldLabel);
        heatmapController.settings.particleDistance = EditorGUILayout.Slider(new GUIContent("Particle Distance", "Smaller distance - improved visuals and precision. Bigger distance - improved performance"), heatmapController.settings.particleDistance, 0.1F, 5F);
        heatmapController.settings.particleSize = EditorGUILayout.Slider(new GUIContent("Particle Size", "(in Unity units)"), heatmapController.settings.particleSize, 0.01F, 15F);
        heatmapController.settings.colorMultiplier = EditorGUILayout.Slider(new GUIContent("Color Multiplier", "Defines how much one position will change color value of particles near it"), heatmapController.settings.colorMultiplier, 0, 1F);
        heatmapController.settings.maxColoringDistance = EditorGUILayout.Slider(new GUIContent("Color Distance", "Max distance in which event position will affect color of particles"), heatmapController.settings.maxColoringDistance, 0.01F, 15F);

        serializedGradient = new SerializedObject(target);
        SerializedProperty colorGradient = serializedGradient.FindProperty("settings.gradient");
        EditorGUILayout.PropertyField(colorGradient, true, null);
    }

    private void AddAdvancedSettings()
    {
        GUILayout.Label("\nAdvanced Settings", EditorStyles.boldLabel);
        heatmapController.settings.colorCutoff = EditorGUILayout.Slider(new GUIContent("Color Cutoff", "Hides all particles with smaller color value (deactivated if set to 0)"), heatmapController.settings.colorCutoff, 0F, 1.01F);
        heatmapController.settings.heightInParticles = EditorGUILayout.IntSlider(new GUIContent("Particle System Height", "(With 0 value height is calculated depending on collider height)"), heatmapController.settings.heightInParticles, 0, 50);
        heatmapController.settings.ignoreYforColoring = EditorGUILayout.Toggle(new GUIContent("Ignore Height (2D)", "(If true color will be calculated only depending on X and Z axes)"), heatmapController.settings.ignoreYforColoring);

        GUILayout.Label("\nEvent Data File", EditorStyles.boldLabel);

        heatmapController.settings.data = (TextAsset)EditorGUILayout.ObjectField(heatmapController.settings.data, typeof(TextAsset), true);
        heatmapController.settings.pathForReadingData = AssetDatabase.GetAssetPath(heatmapController.settings.data);

        GUILayout.Label("\nParticle Material", EditorStyles.boldLabel);
        heatmapController.settings.particleMaterial = (Material)EditorGUILayout.ObjectField(heatmapController.settings.particleMaterial, typeof(Material), true);
    }
}
