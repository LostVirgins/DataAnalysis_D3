using Gamekit3D;
using Gamekit3D.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using static UnityEditor.PlayerSettings;

public class Server : MonoBehaviour, IMessageReceiver
{
    [SerializeField] Damageable playerDamageableScript;
    [SerializeField] Damageable[] enemyDamageableScript;
    [SerializeField] Damageable[] boxDamageableScript;

    public enum FormType
    {
        DAMAGED,
        DEAD,
        HIT,
        PATH
    }

    IEventWriter eventWriter;
    float time = 0;

    void Start()
    {
        eventWriter = new JsonEventWriter(@"C:\Users\Hekbas\CITM\4A\Data_Analysis\DataAnalysis_D3\DataAnalysis_D3\Assets\Heatmap\ParticleSystem\game_data.txt", true);
    }

    void Update()
    {
        time += Time.deltaTime;

        if (time > 0.2)
        {
            time = 0;

            if (eventWriter.IsWriterAvailable())
            {
                HeatmapEvent hmEvent = new HeatmapEvent("m_position", playerDamageableScript.transform.position); 
                eventWriter.SaveEvent(hmEvent);
                //Debug.Log("Added position: " + playerDamageableScript.transform.position);
            }
        }
    }

    void OnEnable()
    {
        playerDamageableScript.onDamageMessageReceivers.Add(this);

        foreach (var script in enemyDamageableScript)
        {
            script.onDamageMessageReceivers.Add(this);
        }

        foreach (var script in boxDamageableScript)
        {
            script.onDamageMessageReceivers.Add(this);
        }
    }
    void OnDisable()
    {
        playerDamageableScript.onDamageMessageReceivers.Remove(this);

        foreach (var script in enemyDamageableScript)
        {
            script.onDamageMessageReceivers.Remove(this);
        }

        foreach (var script in boxDamageableScript)
        {
            script.onDamageMessageReceivers.Remove(this);
        }
    }

    void Start()
    {
        StartCoroutine(GetPathData());
    }

    public void OnReceiveMessage(Gamekit3D.Message.MessageType type, object sender, object msg)
    {
        //GameObject senderObj = sender as GameObject;
        MonoBehaviour senderObj = sender as MonoBehaviour;

        switch (type)
        {
            case Gamekit3D.Message.MessageType.DAMAGED:
                if (senderObj.name == "Ellen")
                {
                    Debug.Log("Damaged: " + senderObj.transform.position);
                    OnDamageReceived(senderObj.transform.position);
                }
                else
                {
                    Debug.Log("Hit: " + senderObj.transform.position);
                    OnHitReceived(senderObj.transform.position);
                }
                break;
            case Gamekit3D.Message.MessageType.DEAD:
                if (senderObj.name == "Ellen")
                {
                    Debug.Log("Dead: " + senderObj.transform.position);
                    OnDeadReceived(senderObj.transform.position);
                }
                else
                {
                    Debug.Log("Hit: " + senderObj.transform.position);
                    OnHitReceived(senderObj.transform.position);
                }
                break;
            default:
                break;
        }
    }

    private void OnDamageReceived(Vector3 pos)
    {
        WWWForm form = new WWWForm();
        form.AddField("position_X", pos.x.ToString().Replace(",", "."));
        form.AddField("position_Y", pos.y.ToString().Replace(",", "."));
        form.AddField("position_Z", pos.z.ToString().Replace(",", "."));

        StartCoroutine(Upload(form, FormType.DAMAGED)); 
    }
    private void OnDeadReceived(Vector3 pos)
    {
        WWWForm form = new WWWForm();
        form.AddField("position_X", pos.x.ToString().Replace(",", "."));
        form.AddField("position_Y", pos.y.ToString().Replace(",", "."));
        form.AddField("position_Z", pos.z.ToString().Replace(",", "."));

        StartCoroutine(Upload(form, FormType.DEAD));
    }
    private void OnHitReceived(Vector3 pos)
    {
        WWWForm form = new WWWForm();
        form.AddField("position_X", pos.x.ToString().Replace(",", "."));
        form.AddField("position_Y", pos.y.ToString().Replace(",", "."));
        form.AddField("position_Z", pos.z.ToString().Replace(",", "."));

        StartCoroutine(Upload(form, FormType.HIT));
    }
    IEnumerator GetPathData()
    {
        Vector3 playerPos = playerDamageableScript.gameObject.transform.position;

        WWWForm form = new WWWForm();
        form.AddField("position_X", playerPos.x.ToString().Replace(",", "."));
        form.AddField("position_Y", playerPos.y.ToString().Replace(",", "."));
        form.AddField("position_Z", playerPos.z.ToString().Replace(",", "."));
        
        StartCoroutine(Upload(form, FormType.PATH));

        yield return new WaitForSeconds(1);

        StartCoroutine(GetPathData());
    }

    IEnumerator Upload(WWWForm form, FormType type)
    {
        UnityWebRequest www = null;
        switch (type)
        {
            case FormType.DAMAGED:
                www = UnityWebRequest.Post("https://citmalumnes.upc.es/~jannl/DamageData.php", form);                
                    break;

            case FormType.DEAD:
                www = UnityWebRequest.Post("https://citmalumnes.upc.es/~jannl/DeadData.php", form);                
                break;

            case FormType.HIT:
                www = UnityWebRequest.Post("https://citmalumnes.upc.es/~jannl/HitData.php", form);
                break;
            
            case FormType.PATH:
                www = UnityWebRequest.Post("https://citmalumnes.upc.es/~jannl/PathData.php", form);
                break;

            default:
                Debug.LogError("Incorrect MessageType.type");
                break;
        }

        if (www != null) yield return www.SendWebRequest();
        else
        {
            Debug.LogError("Couldn't Send Web Request, www = null");
            yield break;
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error + " Result--> " + www.result);
        }
        else
        {
            string response = www.downloadHandler.text;
            Debug.Log("Form " + type + " upload complete!" + " Result--> " + www.result + " Response = " + response);
        }
    }
}