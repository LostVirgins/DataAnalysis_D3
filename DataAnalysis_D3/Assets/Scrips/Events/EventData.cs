using System.Collections.Generic;

/// <summary>
/// Container for event data (name, positions and status in heatmap), used for heatmap visualization
/// </summary>
public class EventData
{
    public string m_eventName;

    public List<MergedPositionEvent> m_positions = new();

    /// <summary>
    /// Should this event be used for heatmap visualisation
    /// </summary>
    public bool m_isVisible = false;
}