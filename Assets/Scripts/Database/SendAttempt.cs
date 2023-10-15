using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

public class SendAttempt : MonoBehaviour
{

    public string gameID;
    public string time;

    string url;

    // Start is called before the first frame update
    void Start()
    {
        url = "https://hemo-cardiac.azurewebsites.net/hemo-submit-attempt.php";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSubmit(string GAMEid, string time)
    {
        gameID = GAMEid;
        this.time = time;
        Debug.Log("Submitting completed attempt..");

        StartCoroutine(SubmitAttempt());
    }

    public IEnumerator SubmitAttempt()
    {
        WWWForm form = new WWWForm();

        form.AddField("TimeSpent", time);
        form.AddField("Completed", 1);
        form.AddField("GAMEid", gameID);

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
                Debug.Log("Host submitted attempt");
                Debug.Log(send.downloadHandler.text);
            }
        }

    }
}