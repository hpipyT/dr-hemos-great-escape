using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class GetScores : MonoBehaviour
{
    public TextMeshProUGUI attempt1;
    public TextMeshProUGUI attempt2;
    public TextMeshProUGUI attempt3;

    public TextMeshProUGUI time1;
    public TextMeshProUGUI time2;
    public TextMeshProUGUI time3;

    string[] strings = new string[6];

    public void Awake()
    {
        OnGet();
    }

    public void OnGet()
    {
        Debug.Log("Retrieving previous runs");
        StartCoroutine(GetStudents());
    }

    private IEnumerator GetStudents()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("https://hemo-cardiac.azurewebsites.net/hemo-get-top-scores.php"))
        {
            yield
            return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                // reads json object of objects
                // 'items' must match name of StudentData[] in ScoreContainer
                ScoreContainer array = JsonUtility.FromJson<ScoreContainer>("{\"items\":" + www.downloadHandler.text + "}");

                // reads team names to evens, times to odds
                int i = 0;
                foreach (ScoreData entry in array.items)
                {
                    strings[i] = entry.TeamName;
                    strings[i + 1] = entry.TimeSpent.ToString();
                    Debug.Log(strings[i]);
                    i += 2;

                }

                attempt1.text = strings[0];
                attempt2.text = strings[2];
                attempt3.text = strings[4];

                time1.text = ConvertToMinutes(strings[1]);
                time2.text = ConvertToMinutes(strings[3]);
                time3.text = ConvertToMinutes(strings[5]);
            }
        }
    }

    private string ConvertToMinutes(string s)
    {
        if (float.TryParse(s, out float time))
        {
            // disgusting rounding error
            time = Mathf.Floor(time);

            int minutes = (int)time / 60;
            int seconds = (int)(time % 60);

            if (seconds == 0) return minutes + ":00";
            if (seconds / 10 > 0) return minutes + ":" + seconds;
            return minutes + ":0" + seconds;
        }
        else return "00:00";

    }

}

[System.Serializable]
public class ScoreData
{
    public string TeamName;
    public float TimeSpent;
    // public Attempt DrHemo_attemptAID; // find way to retrieve nested JSON
}

[System.Serializable]
public class ScoreContainer
{
    public ScoreData[] items;
}