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
        RESPAWN,
        HIT,
        PATH
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
            case Gamekit3D.Message.MessageType.RESPAWN:
                if (senderObj.name == "Ellen")
                {
                    Debug.Log("Respawned: " + senderObj.transform.position);
                    OnRespawnReceived(senderObj.transform.position);
                }
                else
                {

                }
                break;
            default:
                break;
        }
    }

    private void OnDamageReceived(Vector3 pos)
    {
        WWWForm form = new WWWForm();
        form.AddField("position_X", pos.x.ToString());
        form.AddField("position_Y", pos.y.ToString());
        form.AddField("position_Z", pos.z.ToString());

        StartCoroutine(Upload(form, FormType.DAMAGED)); 
    }

    private void OnDeadReceived(Vector3 pos)
    {
        WWWForm form = new WWWForm();
        form.AddField("position_X", pos.x.ToString());
        form.AddField("position_Y", pos.y.ToString());
        form.AddField("position_Z", pos.z.ToString());

        StartCoroutine(Upload(form, FormType.DEAD));
    }

    private void OnRespawnReceived(Vector3 pos)
    {
        WWWForm form = new WWWForm();
        form.AddField("position_X", pos.x.ToString());
        form.AddField("position_Y", pos.y.ToString());
        form.AddField("position_Z", pos.z.ToString());

        StartCoroutine(Upload(form, FormType.RESPAWN));
    }

    private void OnHitReceived(Vector3 pos)
    {
        WWWForm form = new WWWForm();
        form.AddField("position_X", pos.x.ToString().Replace(",", "."));
        form.AddField("position_Y", pos.y.ToString().Replace(",", "."));
        form.AddField("position_Z", pos.z.ToString().Replace(",", "."));

        StartCoroutine(Upload(form, FormType.HIT));
    }

    IEnumerator Upload(WWWForm form, FormType type)//, Action<uint> callback)
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

            case FormType.RESPAWN:
                www = UnityWebRequest.Post("https://citmalumnes.upc.es/~jannl/RespawnData.php", form);
                break;

            case FormType.HIT:
                www = UnityWebRequest.Post("https://citmalumnes.upc.es/~jannl/HitData.php", form);
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
            Debug.Log("Form upload complete!" + " Result--> " + www.result);
            string response = www.downloadHandler.text;
            Debug.Log("Server response: " + response);

            //int id;
            //bool result;
            //if (bool.TryParse(response, out result))//, out id))
            //{
            //    //callback.Invoke((uint)id);
            //    Debug.Log("SUCCESS: " + result);
            //
            //}
            //else
            //{
            //    Debug.LogError("Error: Couldn't parse the server response.");
            //    Debug.Log("Server response: " + response + " " + result);
            //    Debug.LogError(www.result);
            //}
        }
    }

    public static string StringNormalize(string input)
    {
        string normalized = input.Trim();
        normalized = System.Text.RegularExpressions.Regex.Replace(normalized, @"[^a-zA-Z0-9\s]", "");

        return normalized;
    }

}