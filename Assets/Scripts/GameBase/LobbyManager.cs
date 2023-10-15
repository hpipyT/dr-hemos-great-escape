using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public StudentInfo studentInfo;
    public EscapeGameManager EGM;
    public Transform playerController;
    public GameObject locomotionSupport;
    public GameObject locomotionMenu;
    public SpawnPlayers playerSpawner;
    public GameObject roomCodeUIBase;
    public TMP_Text roomCode;
    public Transform spawn;

    public ElevatorController elevator;
    public GameObject[] hostButtons;
    public GameObject exitHospitalButton;

    public bool elevatorFromUnity = false;
    public bool microscopyFromUnity = false;

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        EGM = EscapeGameManager.Instance;
        spawn.position = EGM.spawnLocations.nextSpawn.position;
        spawn.rotation = EGM.spawnLocations.nextSpawn.rotation;
        playerController.transform.position = spawn.position;
        playerController.transform.rotation = spawn.rotation;

        studentInfo = GameObject.Find("PlayerData").GetComponent<StudentInfo>();
        roomCode.text = studentInfo.GAMEid;

        playerSpawner.CreatePlayer();

        if (PhotonNetwork.IsMasterClient)
        {
            foreach (GameObject button in hostButtons)
            {
                button.SetActive(true);
            }

            if (EGM.gameCompleted)
            {
                exitHospitalButton.SetActive(true);
            }
        }
    }

    void Update()
    {
        if (elevatorFromUnity)
        {
            elevatorFromUnity = false;
            ToSecondFloorHall();
        }

        if (microscopyFromUnity)
        {
            microscopyFromUnity = false;
            ToMicroscopyLab();
        }
    }

    public void ToSecondFloorHall()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        elevator.Open();
        Spawn temp = EGM.spawnLocations.hallway2Elevator;
        EGM.gameObject.GetComponent<PhotonView>().RPC("UpdateSpawn", RpcTarget.All, temp.position.x, temp.position.y, temp.position.z, temp.rotation.y);
        StartCoroutine(TakeElevator("LoadSecondFloorHall"));
    }

    public void ToMicroscopyLab()
    {
        EGM.gameObject.GetComponent<PhotonView>().RPC("LoadMicroscopyLab", RpcTarget.MasterClient);
    }

    IEnumerator TakeElevator(string destination)
    {
        yield return new WaitForSecondsRealtime(3);

        EGM.gameObject.GetComponent<PhotonView>().RPC(destination, RpcTarget.MasterClient);
    }

    void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (GameObject button in hostButtons)
            {
                button.SetActive(true);
            }
        }
    }
}
