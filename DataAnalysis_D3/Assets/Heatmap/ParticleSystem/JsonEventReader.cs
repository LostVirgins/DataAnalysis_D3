using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonEventReader : IEventReader
{
    private readonly string path;

    private readonly bool hasFileToRead;

    public JsonEventReader(string path)
    {
        this.path = path;
        hasFileToRead = Startup();
    }

    bool IEventReader.ReaderIsAvailable()
    {
        return hasFileToRead;
    }

    List<EventData> IEventReader.ReadEvents()
    {
        Dictionary<string, EventData> events = new();

        using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (BufferedStream bs = new(fs))
        using (StreamReader sr = new(bs))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                HeatmapEvent hmEvent = JsonUtility.FromJson<HeatmapEvent>(line);

                if (hmEvent.m_eventName != null)
                    AddBaseEventToEventData(hmEvent, events);
                else
                    Debug.Log("line is invalid : " + line);
            }
        }

        return new List<EventData>(events.Values);
    }

    private void AddBaseEventToEventData(HeatmapEvent hmEvent, Dictionary<string, EventData> events)
    {
        EventData currentLineEvent;

        // if event name is not in the EventData list, new EventData should be created
        if (!events.TryGetValue(hmEvent.m_eventName, out currentLineEvent))
        {
            currentLineEvent = new();
            currentLineEvent.m_eventName = hmEvent.m_eventName;
            events.Add(hmEvent.m_eventName, currentLineEvent);
        }

        MergedPositionEvent eventPosition = new();
        eventPosition.m_position = hmEvent.m_position;

        currentLineEvent.m_positions.Add(eventPosition);
    }

    private bool Startup()
    {
        if (File.Exists(path))
        {
            return true;
        }
        else
        {
            Debug.LogError("Invalid path, no file found: " + path);
            return false;
        }
    }
}