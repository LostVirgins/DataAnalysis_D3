using UnityEngine;

public class HeatmapVisualizer
{
    private readonly HeatmapController.Settings m_settings;
    private HeatmapParticleSystem m_heatmapParticleSystem;
    private float[,,] m_particleColorValues;

    public HeatmapVisualizer(HeatmapController.Settings settings)
    {
        m_settings = settings;
        m_heatmapParticleSystem = new HeatmapParticleSystem();
    }

    /// <summary>
    /// Creates and configures particle system for heatmap visualisation.
    /// </summary>
    /// <param name="parent">Object that will contain ParticleSystem component</param>
    public void InitializeParticleSystem(GameObject parent)
    {
        if (parent.GetComponent<ParticleSystem>() != null)
        {
            Debug.Log("There is particle system present on parent object already!");
            return;
        }

        if (m_settings.particleMaterial == null)
        {
            Debug.LogError("Particle material is not defined in settings!");
            return;
        }

        m_heatmapParticleSystem.InitializeParticleSystem(parent, m_settings);
    }

    /// <summary>
    /// Initializes particle array and populates it with particles with default color value.
    /// </summary>
    public void InitializeParticleArray()
    {
        m_heatmapParticleSystem.CreateParticleArray(m_settings);

        Vector3Int sizeInParticles = m_heatmapParticleSystem.GetSizeOfParticleSystemInParticles();
        m_particleColorValues = new float[sizeInParticles.x, sizeInParticles.y, sizeInParticles.z];

        UpdateParticlesInParticleSystem();
    }

    /// <summary>
    /// Adds all positions from eventData to heatmap (by calculating new color values for affected particles)
    /// </summary>
    public void AddEventToHeatMap(EventData eventData)
    {
        foreach (MergedPositionEvent eventPosition in eventData.m_positions)
            AddOnePositionToHeatmap(eventPosition);
    }

    /// <summary>
    /// Resets color value of all particles to default (0f)
    /// </summary>
    public void ResetParticlesColor()
    {
        Vector3Int sizeInParticles = m_heatmapParticleSystem.GetSizeOfParticleSystemInParticles();
        m_particleColorValues = new float[sizeInParticles.x, sizeInParticles.y, sizeInParticles.z];
    }

    public void UpdateParticlesInParticleSystem()
    {
        m_heatmapParticleSystem.UpdateParticlesInParticleSystem(m_particleColorValues, m_settings);
    }

    private void AddOnePositionToHeatmap(MergedPositionEvent eventPosition)
    {
        Vector3Int eventPositionInParticleGrid = m_heatmapParticleSystem.ConvertGlobalPositionToParticleGrid(eventPosition.m_position, m_settings);
        Vector3Int sizeInParticles = m_heatmapParticleSystem.GetSizeOfParticleSystemInParticles();

        // calculate bounds in which particles can be affected by eventPosition
        Vector3Int minBound = CalculateMinBound(eventPositionInParticleGrid);
        Vector3Int maxBound = CalculateMaxBound(eventPositionInParticleGrid, sizeInParticles);

        // checking all particles in this bounds, and updating their color value depending on distance
        for (int x = minBound.x; x <= maxBound.x; x += 1)
        {
            for (int y = minBound.y; y <= maxBound.y; y += 1)
            {
                for (int z = minBound.z; z <= maxBound.z; z += 1)
                {
                    if (IsInBoundsOfParticleArray(x, y, z, sizeInParticles))
                        UpdateColorAddValue(new Vector3Int(x, y, z), eventPositionInParticleGrid, eventPosition);
                }
            }
        }
    }

    private void UpdateColorAddValue(Vector3Int particlePositionInGrid, Vector3Int eventPositionInParticleGrid, MergedPositionEvent eventPosition)
    {
        float distance = CalculateDistanceBetweenTwoParticleGridPoints(particlePositionInGrid, eventPositionInParticleGrid);

        if (distance < m_settings.maxColoringDistance)
        {
            // calculate colorAddValue, depending on how close is distance to maxColoringDistance
            float colorAddValue = m_settings.colorMultiplier * (1 - distance / m_settings.maxColoringDistance);
            m_particleColorValues[particlePositionInGrid.x, particlePositionInGrid.y, particlePositionInGrid.z] += colorAddValue * eventPosition.m_multiplier;
        }
    }

    private float CalculateDistanceBetweenTwoParticleGridPoints(Vector3Int point1, Vector3Int point2)
    {
        float distanceSquare = (point1.x - point2.x) * (point1.x - point2.x) + (point1.z - point2.z) * (point1.z - point2.z);

        if (!m_settings.ignoreYforColoring)
            distanceSquare += (point1.y - point2.y) * (point1.y - point2.y);

        return Mathf.Sqrt(distanceSquare) * m_settings.particleDistance;
    }

    private bool IsInBoundsOfParticleArray(int x, int y, int z, Vector3Int sizeInParticles)
    {
        return x >= 0 && z >= 0 && y >= 0 && x < sizeInParticles.x && y < sizeInParticles.y & z < sizeInParticles.z;
    }

    private Vector3Int CalculateMinBound(Vector3Int positionInParticleGrid)
    {
        Vector3Int min = new();

        min.x = (int)(positionInParticleGrid.x - m_settings.maxColoringDistance / m_settings.particleDistance - 1);
        min.z = (int)(positionInParticleGrid.z - m_settings.maxColoringDistance / m_settings.particleDistance - 1);

        if (m_settings.ignoreYforColoring)
            min.y = 0;
        else
            min.y = (int)(positionInParticleGrid.y - m_settings.maxColoringDistance / m_settings.particleDistance - 1);

        return min;
    }

    private Vector3Int CalculateMaxBound(Vector3Int positionInParticleGrid, Vector3Int sizeInParticles)
    {
        Vector3Int max = new();

        max.x = (int)(positionInParticleGrid.x + m_settings.maxColoringDistance / m_settings.particleDistance + 1);
        max.z = (int)(positionInParticleGrid.z + m_settings.maxColoringDistance / m_settings.particleDistance + 1);

        if (m_settings.ignoreYforColoring)
            max.y = sizeInParticles.y - 1;
        else
            max.y = (int)(positionInParticleGrid.y + m_settings.maxColoringDistance / m_settings.particleDistance + 1);

        return max;
    }
}
