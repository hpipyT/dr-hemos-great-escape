using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RetrieveScores : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(GetData());
    }

    private IEnumerator GetData()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("https://com-tech-xr-php.azurewebsites.net/retrieve-scores.php"))
        {
            yield
            return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                // Get the returned data as a string
                string data = www.downloadHandler.text;
                // parse the data

                LeaderboardData boardData = JsonUtility.FromJson<LeaderboardData>(data);
                Debug.Log("First name:" + boardData.FirstName);
                Debug.Log("Last name:" + boardData.LastName);
                Debug.Log("SID:" + boardData.SID);
                // Debug.Log("Attempt:" + boardData.Attempt.DrHemo_attemptAID);
            }
        }
    }
}

[System.Serializable]
public class LeaderboardData
{
    public string FirstName;
    public string LastName;
    public int SID;
    // public Attempt DrHemo_attemptAID; // find way to retrieve nested JSON
}

// [System.Serializable]
// public class Attempt
// {
//     public bool Completed;
// }
