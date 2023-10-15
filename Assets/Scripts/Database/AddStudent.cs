using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class AddStudent : MonoBehaviour
{
    public TMP_InputField firstName; // change this to string and pass in the inputField.text
    public TMP_InputField sid;
    public TMP_InputField classSection;
    public string url = "https://hemo-cardiac.azurewebsites.net/add-student.php"; // upload addStudent php

    public void OnSubmit()
    {
        Debug.Log("Sending data");
        StartCoroutine(SendStudentData());
    }

    IEnumerator SendStudentData()
    {
        WWWForm form = new WWWForm();

        form.AddField("FirstName", firstName.text);
        form.AddField("SID", int.Parse(sid.text));
        form.AddField("ClassSection", int.Parse(classSection.text));

        Debug.Log(firstName.text);
        Debug.Log(sid.text);
        Debug.Log(classSection.text);
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