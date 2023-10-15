using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public EscapeGameManager EGM;

    public Transform fieldParent;
    public InputField userLogin;
    public InputField userPassword;

    public bool unityLogin = false;

    string uri = "https://hemo-cardiac.azurewebsites.net/login.php?var1=";

    private void Start()
    {
        StartCoroutine(setupUI());
    }

    public void OnLogin()
    {
        Debug.Log("Starting Get");
        StartCoroutine(LoginStudent());
        //StartCoroutine(GetStudent());
    }

    public void Update()
    {
        if (unityLogin)
        {
            unityLogin = false;
            OnLogin();
        }

    }

    private IEnumerator LoginStudent()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(uri + userLogin.text + "&var2=" + userPassword.text))
        {
            yield
            return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {

                if (www.downloadHandler.text.Contains("objects"))
                {
                    LoginData data = JsonUtility.FromJson<LoginData>("{\"SID\":\"" + www.downloadHandler.text + "\"}");
                    Debug.Log(www.downloadHandler.text);
                    Debug.Log(data.SID);
                }
                else
                {
                    LoginData data = JsonUtility.FromJson<LoginData>(www.downloadHandler.text);
                    Debug.Log(www.downloadHandler.text);

                    // Set PlayerData SID 
                    StudentInfo student = GameObject.Find("PlayerData").GetComponent<StudentInfo>();
                    student.SetSID(data.SID);

                    Debug.Log("StudentInfo populated");
                    SceneManager.LoadScene("ConnectToServer");

                }

            }
        }
    }

    IEnumerator setupUI()
    {
        yield
        return new WaitForSecondsRealtime(1);

        UIBuilder.instance.AddLabel("Login");
        UIBuilder.instance.AddLabel("Username:");
        UIBuilder.instance.AddTextField("123@aol.com");
        UIBuilder.instance.AddLabel("Password:");
        UIBuilder.instance.AddTextField("123");
        UIBuilder.instance.AddButton("Login", OnLogin);
        UIBuilder.instance.Show();

        userLogin = fieldParent.GetChild(2).GetComponent<InputField>();
        userPassword = fieldParent.GetChild(4).GetComponent<InputField>();
    }
}

[System.Serializable]
public class LoginData
{
    public string SID;
    public string GAMEid;
}