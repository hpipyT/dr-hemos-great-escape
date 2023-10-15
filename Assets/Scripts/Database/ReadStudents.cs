using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ReadStudents : MonoBehaviour
{
    public void OnGet()
    {
        StartCoroutine(GetStudents());
    }

    private IEnumerator GetStudents()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("https://hemo-cardiac.azurewebsites.net/read-students.php"))
        {
            yield
            return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                // 'items' must match name of StudentData[] in DataContainer
                DataContainer array = JsonUtility.FromJson<DataContainer>("{\"items\":" + www.downloadHandler.text + "}");

                foreach (StudentData entry in array.items)
                {
                    Debug.Log("SID:" + entry.SID + "Section: " + entry.Section + "\nLoggedIn: " + entry.LoggedIn);

                }
            }
        }
    }
}

[System.Serializable]
public class StudentData
{
    public string SID;
    public string Section;
    public string LoggedIn;
    // public Attempt DrHemo_attemptAID; // find way to retrieve nested JSON
}

[System.Serializable]
public class DataContainer
{
    public StudentData[] items;
}