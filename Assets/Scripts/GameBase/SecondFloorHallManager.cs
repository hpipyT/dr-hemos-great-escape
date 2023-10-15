using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class SecondFloorHallManager : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public StudentInfo studentInfo;
    public EscapeGameManager EGM;
    public Transform playerController;
    public GameObject locomotionSupport;
    public GameObject locomotionMenu;
    public SpawnPlayers playerSpawner;
    public Transform spawn;

    public GameObject[] hostButtons;

    public bool unityLoadLibrary = false;
    public bool unityLoadCafeteria = false;


    bool loadingScene = false;

    public ElevatorController elevator;

    void Start()
    {
        EGM = EscapeGameManager.Instance;
        spawn.position = EGM.spawnLocations.nextSpawn.position;
        spawn.rotation = EGM.spawnLocations.nextSpawn.rotation;
        playerController.transform.position = spawn.position;
        playerController.transform.rotation = spawn.rotation;

        loadingScene = false;
        studentInfo = GameObject.Find("PlayerData").GetComponent<StudentInfo>();

        playerSpawner.CreatePlayer();

        foreach (GameObject button in hostButtons)
        {
            button.SetActive(false);
        }

            CheckButtons();
        
    }

    public void CheckButtons()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (GameObject button in hostButtons)
            {
                button.SetActive(true);
            }
        
        for(int i = 0; i < GameState.Instance.AllPuzzles.Count; i++)
        {
            if(GameState.Instance.AllPuzzles[i] == null || GameState.Instance.AllPuzzles[i].completed != true)
            {
                continue;
            }

            foreach(GameObject button in hostButtons)
            {
                if(button.name == GameState.Instance.AllPuzzles[i].room)
                {
                    Debug.Log("setting " + button.name + " button to deactivated");
                    button.SetActive(false);
                }
            }
        }
        }
    }

    public void Update()
    {
        if (unityLoadLibrary)
        {
            unityLoadLibrary = false;
            ToLibrary();
        }

        if (unityLoadCafeteria)
        {
            unityLoadCafeteria = false;
            ToCafeteriaDoor1();
        }

    }

    public void ToLobby()
    {
        if(!loadingScene)
        {
            loadingScene = true;
            elevator.Open();
            Spawn temp = EGM.spawnLocations.lobbyElevator;
            EGM.gameObject.GetComponent<PhotonView>().RPC("UpdateSpawn", RpcTarget.All, temp.position.x, temp.position.y, temp.position.z, temp.rotation.y);
            StartCoroutine(TakeElevator("LoadLobby"));
        }
    }

    public void ToCafeteriaDoor1()
    {
        if(!loadingScene)
        {
            loadingScene = true;
            Spawn temp = EGM.spawnLocations.cafe1;
            EGM.gameObject.GetComponent<PhotonView>().RPC("UpdateSpawn", RpcTarget.All, temp.position.x, temp.position.y, temp.position.z, temp.rotation.y);
            StartCoroutine(Travel("LoadCafeteria"));
        }
    }

    public void ToCafeteriaDoor2()
    {
        if (!loadingScene)
        {
            loadingScene = true;
            Spawn temp = EGM.spawnLocations.cafe2;
            EGM.gameObject.GetComponent<PhotonView>().RPC("UpdateSpawn", RpcTarget.All, temp.position.x, temp.position.y, temp.position.z, temp.rotation.y);
            StartCoroutine(Travel("LoadCafeteria"));
        }
    }

    public void ToLibrary()
    {
        if(!loadingScene)
        {
            loadingScene = true;
            EGM.gameObject.GetComponent<PhotonView>().RPC("LoadLibrary", RpcTarget.MasterClient);
        }
    }

    IEnumerator TakeElevator(string destination)
    {
        yield return new WaitForSecondsRealtime(3);

        EGM.gameObject.GetComponent<PhotonView>().RPC(destination, RpcTarget.MasterClient);
    }

    IEnumerator Travel(string locationName)
    {
        yield return new WaitForSecondsRealtime(1.0f);
        EGM.gameObject.GetComponent<PhotonView>().RPC(locationName, RpcTarget.MasterClient);
    }

    void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            CheckButtons();
        }
    }
}
