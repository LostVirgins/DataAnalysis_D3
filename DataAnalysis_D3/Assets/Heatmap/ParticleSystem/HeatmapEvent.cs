using UnityEngine;

/// <summary>
/// Base event is used to save and load(read) data for heatmap.
/// </summary>
[System.Serializable]
class HeatmapEvent
{
    /// <summary>
    /// Descriptive and unique name of event 
    /// </summary>
    public string m_eventName;

    /// <summary>
    /// Position of event in world space
    /// </summary>
    public Vector3 m_position;

    public HeatmapEvent(string EventName, Vector3 Position)
    {
        m_eventName = EventName;
        m_position = Position;
    }
}
