using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Photon.Pun;
using UnityEngine.InputSystem;

public class CreateAttempt : MonoBehaviour
{
    int maxStudents = 5;

    public TMP_InputField teamName; // change this to string and pass in the inputField.text
    public TMP_InputField sid1;
    public TMP_InputField sid2;
    public TMP_InputField sid3;
    public TMP_InputField sid4;
    public TMP_InputField sid5;
    public TMP_InputField time;

    public string url = "https://hemo-cardiac.azurewebsites.net/hemo-create-attempt.php"; // upload addStudent php

    // TMP_InputField[] sids;
    List<TMP_InputField> sids = new List<TMP_InputField>();

    public void OnSubmit()
    {
        // change to get student's SID from login 

        sids.Add(sid1);
        sids.Add(sid2);
        sids.Add(sid3);
        sids.Add(sid4);
        sids.Add(sid5);
        sids.Add(time);

        Debug.Log("Sending data");
        StartCoroutine(SendStudentData());
    }

    IEnumerator SendStudentData()
    {
        WWWForm form = new WWWForm();

        form.AddField("TeamName", teamName.text);
        form.AddField("TimeSpent", time.text);
        form.AddField("Completed", 1);

        for (int i = 0; i < maxStudents; i++)
        {
            // if Student# isn't empty
            if (sids[i].text != "")
            {
                form.AddField("SID" + (i + 1), int.Parse(sids[0].text));
            }
        }

        // form.AddField("SID1", int.Parse(sids[0].text));

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
                Debug.Log(send.downloadHandler.text);
            }
        }
    }
}