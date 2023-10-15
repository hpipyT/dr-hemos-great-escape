using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class UpdateAttempt : MonoBehaviour
{

    public string gameID;
    public string sid;
    public string time;
    public int playerCount;

    public string url = "https://hemo-cardiac.azurewebsites.net/update-hemo-attempt.php"; // upload addStudent php

    public void Start()
    {
        OnSubmit(gameID, "10");
    }

    public void OnSubmit(string GAMEid, string time)
    {
        gameID = GAMEid;
        this.time = time;
        Debug.Log("Updating attempt");
        StartCoroutine(AddPlayerToAttempt());
    }

    IEnumerator AddPlayerToAttempt()
    {
        WWWForm form = new WWWForm();

        // TODO: get player count
        playerCount = 2;

        // Host on own will be SID[1]
        // subsequent joins are SID[playerCount]

        // UpdateAttempt

        form.AddField("TimeSpent", 0);
        form.AddField("Completed", 0);

        form.AddField("GAMEid", gameID);

        form.AddField("SID", int.Parse(sid));
        // form.AddField("JoinNum", playerCount + 1);
        form.AddField("JoinNum", playerCount);

        string url = "https://hemo-cardiac.azurewebsites.net/hemo-update-attempt.php";
        using (var send = UnityWebRequest.Post(url, form))
        {
            yield
            return send.SendWebRequest();

            if (send.result != UnityWebRequest.Result.Success)
            {
                print(send.error);
                Debug.Log("Uh oh, error");
            }
            else
            {
                Debug.Log("Added new SID to attempt");
                Debug.Log(send.downloadHandler.text);
            }
        }

        yield
        return null;
    }
}