using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AccessServerData : MonoBehaviour
{
    public static AccessServerData Instance;
    IEventWriter eventWriter;

    void Start()
    {
        Instance = this;
        eventWriter = new JsonEventWriter(@"C:\Users\Hekbas\CITM\4A\Data_Analysis\DataAnalysis_D3\DataAnalysis_D3\Assets\Heatmap\ParticleSystem\game_server.txt", true);
    }

    void Update() {}

    public IEnumerator RetrieveData(Server.FormType type)
    {
        List<HeatmapEvent> hmEvents = new List<HeatmapEvent>();
        UnityWebRequest www = null;
        string eventName = null;

        switch (type)
        {
            case Server.FormType.PATH:
                www = UnityWebRequest.Get("https://citmalumnes.upc.es/~jannl/GetPathData.php");
                eventName = "Position";
                break;

            case Server.FormType.ATTACK:
                www = UnityWebRequest.Get("https://citmalumnes.upc.es/~jannl/GetHitData.php");
                eventName = "Attack";
                break;

            case Server.FormType.DAMAGED:
                www = UnityWebRequest.Get("https://citmalumnes.upc.es/~jannl/GetDamageData.php");
                eventName = "Damaged";
                break;

            case Server.FormType.DEATH:
                www = UnityWebRequest.Get("https://citmalumnes.upc.es/~jannl/GetDeathData.php");
                eventName = "Death";
                break;

            default:
                break;
        }

        if (www != null) yield return www.SendWebRequest();
        else
        {
            Debug.LogError("Couldn't Send Web Request, www = null");
            yield break;
        }

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) Debug.Log(www.error);
        else
        {
            string rawData = www.downloadHandler.text;
            rawData = rawData.TrimEnd('/');
            string[] posRawData = rawData.Split('/');

            //Debug.Log(posRawData.Length + " last pos = " + posRawData[posRawData.Length -1]);

            for (int i = 0; i < posRawData.Length; i += 3)
            {
                Vector3 posData = new (
                    float.Parse(posRawData[i]),
                    float.Parse(posRawData[i + 1]),
                    float.Parse(posRawData[i + 2])
                );
                //Debug.Log("Position = " + posData);

                hmEvents.Add(new HeatmapEvent(eventName, posData));
            }
        }

        eventWriter.SaveEvents(hmEvents);
    }
}
