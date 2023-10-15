using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RemovePlayer : MonoBehaviour
{

    public void RemoveThem(string sid, string aid)
    {
        Debug.Log("Removing player from attempt");
        StartCoroutine(RemovePlayerFromAttempt(sid, aid));
    }

    // Start is called before the first frame update
    IEnumerator RemovePlayerFromAttempt(string sid, string aid)
    {
        WWWForm form = new WWWForm();

        // UpdateAttempt

        form.AddField("GAMEid", aid);
        form.AddField("SID", sid);

        string url = "https://hemo-cardiac.azurewebsites.net/hemo-remove-player.php";
        using (var send = UnityWebRequest.Post(url, form))
        {
            yield return send.SendWebRequest();

            if (send.result != UnityWebRequest.Result.Success)
            {
                print(send.error);
                Debug.Log("Uh oh, error");
            }
            else
            {
                Debug.Log("Removed player from attempt");
                Debug.Log(send.downloadHandler.text);
            }
        }


        yield return null;
    }
}
