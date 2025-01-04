using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AccessServerData : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(RetrieveData(Server.FormType.PATH));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator RetrieveData(Server.FormType type)
    {
        UnityWebRequest www = null;

        switch (type)
        {
            case Server.FormType.DAMAGED:
                www = UnityWebRequest.Get("https://citmalumnes.upc.es/~jannl/GetDamageData.php");
                break;

            case Server.FormType.DEAD:
                www = UnityWebRequest.Get("https://citmalumnes.upc.es/~jannl/GetDeathData.php");
                break;

            case Server.FormType.HIT:
                www = UnityWebRequest.Get("https://citmalumnes.upc.es/~jannl/GetHitData.php");
                break;

            case Server.FormType.PATH:
                www = UnityWebRequest.Get("https://citmalumnes.upc.es/~jannl/GetPathData.php");
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
            string rawData = www.downloadHandler.text.Replace('.', ',');
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
                Debug.Log("Position = " + posData);
            }
        }       
    }
}
