using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class CreatePuzzleAnswer : MonoBehaviour
{
    public string SID;
    public string PUZID;
    public string answer;

    public string url = "https://hemo-cardiac.azurewebsites.net/hemo-create-answer.php"; // upload addStudent php

    public void OnSubmit(PuzzleAnswer data)
    {
        SID = data.SID;
        PUZID = data.PUZid;
        answer = data.answer;

        Debug.Log("Sending data");
        StartCoroutine(SendStudentData());
    }

    IEnumerator SendStudentData()
    {
        WWWForm form = new WWWForm();

        form.AddField("SID", SID);
        form.AddField("PUZSTEPid", PUZID);
        form.AddField("Answer", answer);

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
                Debug.Log("Sent request successfully");
                Debug.Log(send.downloadHandler);
            }
        }
    }
}