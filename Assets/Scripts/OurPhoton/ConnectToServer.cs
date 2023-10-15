using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        StartCoroutine(AwaitConnection());
    }

    IEnumerator AwaitConnection()
    {
        PhotonNetwork.ConnectUsingSettings();
        yield return null;
    }

    // Update is called once per frame
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.LoadLevel("RoomSelection");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Connected to server");
    }
}
