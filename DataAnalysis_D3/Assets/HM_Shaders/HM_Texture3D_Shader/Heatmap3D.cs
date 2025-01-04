using UnityEngine;

public class Heatmap3D : MonoBehaviour
{
    public static Heatmap3D Instance;

    [Header("Heatmap Settings")]
    public Material heatmapMaterial;        // Material using the volumetric heatmap shader
    public int gridSize = 32;               // Resolution of the 3D texture
    public Vector3 worldBounds;             // World space bounds
    public float intensityThreshold = 0.1f; // Intensity threshold for visibility
    public float stepSize = 0.01f;          // Raymarching step size

    private Texture3D heatmapTexture;       // 3D texture storing heatmap data
    private float[,,] heatmapData;          // Internal array to manage heatmap values

    void Start()
    {
        Instance = this;
        worldBounds = new Vector3(100, 100, 100);

        InitializeHeatmap();    // Initialize the heatmap texture and data
        UpdateMaterial();       // Assign the texture and parameters to the material
    }

    void InitializeHeatmap()
    {
        // Create the internal data array
        heatmapData = new float[gridSize, gridSize, gridSize];

        // Create the Texture3D
        heatmapTexture = new Texture3D(gridSize, gridSize, gridSize, TextureFormat.RFloat, false);
        heatmapTexture.wrapMode = TextureWrapMode.Clamp;
        heatmapTexture.filterMode = FilterMode.Bilinear;

        ClearHeatmap();
    }

    public void AddPosition(Vector3 position)
    {
        // Convert world space position to texture space (0 to 1)
        Vector3 normalizedPos = new Vector3(
            Mathf.InverseLerp(-worldBounds.x / 2, worldBounds.x / 2, position.x),
            Mathf.InverseLerp(-worldBounds.y / 2, worldBounds.y / 2, position.y),
            Mathf.InverseLerp(-worldBounds.z / 2, worldBounds.z / 2, position.z)
        );

        // Convert normalized position to grid indices
        int x = Mathf.Clamp((int)(normalizedPos.x * gridSize), 0, gridSize - 1);
        int y = Mathf.Clamp((int)(normalizedPos.y * gridSize), 0, gridSize - 1);
        int z = Mathf.Clamp((int)(normalizedPos.z * gridSize), 0, gridSize - 1);

        heatmapData[x, y, z] += 1.0f;
        UpdateTexture();
    }

    void UpdateTexture()
    {
        // Convert the heatmap data to a flattened array for the texture
        Color[] textureColors = new Color[gridSize * gridSize * gridSize];
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    float intensity = heatmapData[x, y, z];
                    int index = x + gridSize * (y + gridSize * z);
                    textureColors[index] = new Color(intensity, 0, 0, 0); // Single-channel intensity
                }
            }
        }

        // Apply the color data to the Texture3D
        heatmapTexture.SetPixels(textureColors);
        heatmapTexture.Apply();
    }

    void UpdateMaterial()
    {
        heatmapMaterial.SetTexture("_HeatmapTexture", heatmapTexture);
        heatmapMaterial.SetFloat("_Threshold", intensityThreshold);
        heatmapMaterial.SetFloat("_StepSize", stepSize);
    }

    public void ClearHeatmap()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                for (int z = 0; z < gridSize; z++)
                    heatmapData[x, y, z] = 0f;
            }
        }

        UpdateTexture();
    }

    void OnDrawGizmos()
    {
        // Visualize the bounds of the heatmap in the editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, worldBounds);
    }
}