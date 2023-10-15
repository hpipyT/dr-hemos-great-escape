using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ReadStudent : MonoBehaviour
{
    DataContainer studentData;
    public void OnGetStudent()
    {
        //Debug.Log("Starting Get");
        StartCoroutine(GetStudent());
        //StartCoroutine(GetStudent());
    }

    private IEnumerator GetStudent()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("https://com-tech-xr-php.azurewebsites.net/read-students.php"))
        {
            yield
            return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Before");
                // JsonUtility.FromJson<DataContainer>(www.downloadHandler.text, studentData);
                DataContainer array = JsonUtility.FromJson<DataContainer>("{\"FirstName\":" + www.downloadHandler.text + "}");
                Debug.Log("After");
                Debug.Log(www.downloadHandler.text);

                // TODO: formatting

                /*                foreach (StudentData entry in array.items)
                        {
                            Debug.Log("First name:" + entry.FirstName);
                            Debug.Log("Last name:" + entry.LastName);
                            Debug.Log("SID:" + entry.SID);
                        }
        */
                // Debug.Log("Attempt:" + boardData.Attempt.DrHemo_attemptAID);
            }
        }
    }
}