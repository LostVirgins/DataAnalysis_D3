using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonEventWriter : IEventWriter
{
    private readonly bool createFileIfNonFound;
    private readonly string path;

    private readonly bool hasFileToWrite;

    public JsonEventWriter(string path, bool createFileIfNonFound)
    {
        this.path = path;
        this.createFileIfNonFound = createFileIfNonFound;
        hasFileToWrite = Startup();
    }

    bool IEventWriter.IsWriterAvailable()
    {
        return hasFileToWrite;
    }

    bool IEventWriter.SaveEvent(HeatmapEvent baseEvent)
    {
        if (!hasFileToWrite || !File.Exists(path)) return false;

        StreamWriter writer = new StreamWriter(path, true);

        try
        {
            writer.WriteLine(ComposeJsonString(baseEvent));
            return true;
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            return false;
        }
        finally
        {
            writer.Close();
        }
    }

    bool IEventWriter.SaveEvents(List<HeatmapEvent> baseEvents)
    {
        if (!hasFileToWrite || !File.Exists(path)) return false;

        StreamWriter writer = new StreamWriter(path, true);

        try
        {
            foreach (HeatmapEvent baseEvent in baseEvents)
                writer.WriteLine(ComposeJsonString(baseEvent));

            return true;
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            return false;
        }
        finally
        {
            writer.Close();
        }
    }

    private bool CreateFile(string path)
    {
        if (!createFileIfNonFound) return false;

        try
        {
            File.Create(path);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            return false;
        }
    }

    private string ComposeJsonString(HeatmapEvent baseEvent)
    {
        return JsonUtility.ToJson(baseEvent);
    }

    private bool Startup()
    {
        if (File.Exists(path)) return true;
        if (!createFileIfNonFound) return false;

        return CreateFile(path);
    }
}