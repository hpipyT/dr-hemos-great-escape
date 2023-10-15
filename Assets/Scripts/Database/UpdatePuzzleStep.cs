using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class UpdatePuzzleStep : MonoBehaviour
{

    public string aid;
    public string time;
    public string hints;

    public string url = "https://hemo-cardiac.azurewebsites.net/hemo-update-puzzleStep.php"; // upload addStudent php

    public void OnSubmit(PuzzleStepData data)
    {
        // send = DataExporter.data
        this.aid = data.PUZid;
        this.time = data.runningTime.ToString();
        // 1 if step complete 0 if incomplete, redundant
        this.hints = data.hintsTaken;

        Debug.Log("Updating PuzzleStep");
        StartCoroutine(UpdateStep());
    }

    IEnumerator UpdateStep()
    {
        WWWForm form = new WWWForm();

        form.AddField("PUZSTEPid", aid);
        form.AddField("TimeTaken", time);
        form.AddField("HintsTaken", hints);

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