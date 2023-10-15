using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using UnityEngine.Networking;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public Transform fieldParent;
    public GameObject roomNotFoundUI;
    // creators
    public InputField TeamName;
    // for joiners
    public InputField LobbyNum;

    StudentInfo student;
    public string GAMEid;

    public bool createUnityTestRoom = false;
    public bool joinUnityTestRoom = false;
    public string joinCode;

    bool roomJoinFailed = false;
    bool roomJoined = false;
    bool roomCreated = false;

    private void Start()
    {
        StartCoroutine(setupUI());
    }

    public void Update()
    {
        if (createUnityTestRoom)
        {
            createUnityTestRoom = false;
            TeamName.text = "UnityTestRoom";
            CreateRoom();
        }

        if (joinUnityTestRoom)
        {
            joinUnityTestRoom = false;
            LobbyNum.text = joinCode;
            JoinRoom();
        }

        if (roomJoinFailed)
        {
            roomJoinFailed = false;
            roomNotFoundUI.SetActive(true);
            GAMEid = "";
            LobbyNum.text = "";
        }

        student = GameObject.Find("PlayerData").GetComponent<StudentInfo>();
    }

    IEnumerator setupUI()
    {
        yield
        return new WaitForSecondsRealtime(1);

        UIBuilder.instance.AddLabel("Enter new team name:");
        UIBuilder.instance.AddTextField("");
        UIBuilder.instance.AddButton("Create", CreateRoom);
        UIBuilder.instance.AddLabel("");
        UIBuilder.instance.AddLabel("Enter room code:");
        UIBuilder.instance.AddTextField("");
        UIBuilder.instance.AddButton("Join", JoinRoom);
        UIBuilder.instance.Show();

        TeamName = fieldParent.GetChild(1).GetComponent<InputField>();
        LobbyNum = fieldParent.GetChild(5).GetComponent<InputField>();
    }

    public void JoinRoom()
    {
        Debug.Log("Attempting to join room");
        // set the GAMEid to the entered code
        if (LobbyNum == null || LobbyNum.text == "")
        {
            Debug.Log("LobbyNum shouldn't be empty");
            roomJoinFailed = true;
            return;
        }
        else
        {
            GAMEid = LobbyNum.text;
            student.SetGAMEid(GAMEid);

            StartCoroutine(AddPlayerToAttempt());

            StartCoroutine(AttemptingJoinRoom());

            roomJoinFailed = !PhotonNetwork.JoinRoom(GAMEid);
        }
    }

    IEnumerator AttemptingJoinRoom()
    {
        yield
        return new WaitForSecondsRealtime(2.0f);

        if (!roomJoined)
        {
            roomJoinFailed = true;
            Debug.Log("Removing player from attempt");
            StartCoroutine(RemovePlayerFromAttempt(student.SID, student.GAMEid));
        }
        else
        {
            //Debug.Log("Attempting to join room");

        }

    }

    public override void OnJoinedRoom()
    {
        roomNotFoundUI.SetActive(false);
        Debug.Log("Player joined room");
        roomJoined = true;

        // set Student's GAMEid
        student.SetGAMEid(GAMEid);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("LobbyScene");
        }
        else
        {

        }
    }

    public void CreateRoom()
    {
        if (roomCreated) return;

        roomCreated = true;

        // create attempt that returns an ID
        StartCoroutine(CreateNewAttempt());
    }

    IEnumerator CreateNewAttempt()
    {
        WWWForm form = new WWWForm();

        form.AddField("TeamName", TeamName.text);
        form.AddField("TimeSpent", 0);
        form.AddField("Completed", 0);
        form.AddField("SID1", student.GetSID());

        // form.AddField("SID1", int.Parse(sids[0].text));
        string url = "https://hemo-cardiac.azurewebsites.net/hemo-create-attempt.php";

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
                Debug.Log("Database: Created new attempt");
                Debug.Log("GAMEid: " + send.downloadHandler.text);
                GAMEid = send.downloadHandler.text;
                PhotonNetwork.CreateRoom(GAMEid);

                // put this GAMEid somewhere in the scene

            }
        }
    }

    // TODO: check for bugz
    IEnumerator AddPlayerToAttempt()
    {
        WWWForm form = new WWWForm();

        // TODO: get player count
        int playerCount = PhotonNetwork.CountOfPlayers;
        Debug.Log("Database: # of Players in the scene at AddPlayerToAttempt:" + playerCount);

        // Host on own will be SID[1]
        // subsequent joins are SID[playerCount]

        // UpdateAttempt

        form.AddField("TimeSpent", 0);
        form.AddField("Completed", 0);

        form.AddField("GAMEid", LobbyNum.text);

        form.AddField("SID", student.GetSID());
        // form.AddField("JoinNum", playerCount + 1);
        form.AddField("JoinNum", playerCount);

        string url = "https://hemo-cardiac.azurewebsites.net/hemo-update-attempt.php";
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
                Debug.Log("Added new SID to attempt");
                Debug.Log(send.downloadHandler.text);
            }
        }

        yield
        return null;
    }

    IEnumerator RemovePlayerFromAttempt(string sid, string aid)
    {
        WWWForm form = new WWWForm();

        // UpdateAttempt

        form.AddField("GAMEid", aid);
        form.AddField("SID", sid);

        string url = "https://hemo-cardiac.azurewebsites.net/hemo-remove-player.php";
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
                Debug.Log("Removed player from attempt");
                Debug.Log(send.downloadHandler.text);
            }
        }

        yield
        return null;
    }

}