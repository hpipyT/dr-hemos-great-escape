using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class CreatePuzzleStep : MonoBehaviour
{
    public string AID;
    public string puzzleName;
    public string puzzleStep;
    public string hintsTaken;
    public string timeTaken;

    public StudentInfo student;

    public string url = "https://hemo-cardiac.azurewebsites.net/hemo-create-puzzleStep2.php"; // yes, its hemo-create-puzzleStep2
    public void OnSubmit(PuzzleStepData data)
    {
        student = GameObject.Find("PlayerData").GetComponent<StudentInfo>();
        Debug.Log("Sending data for CreatePuzzleStep");

        this.AID = data.AID;
        this.puzzleName = data.roomName;
        this.puzzleStep = data.stepNo.ToString();
        this.hintsTaken = data.hintsTaken;
        this.timeTaken = data.runningTime.ToString();

        StartCoroutine(SendStudentData());
    }

    IEnumerator SendStudentData()
    {
        WWWForm form = new WWWForm();

        form.AddField("GAMEid", AID);
        form.AddField("PuzzleName", puzzleName);
        form.AddField("PuzzleStep", puzzleStep);
        form.AddField("HintsTaken", hintsTaken);
        form.AddField("TimeTaken", timeTaken);

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
                Debug.Log("Setting PUZid in Player data to : ");
                Debug.Log(send.downloadHandler);
                student.PUZid = send.downloadHandler.text;
            }
        }
    }
}