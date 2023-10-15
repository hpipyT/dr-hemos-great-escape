using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class CafeManager : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public StudentInfo studentInfo;
    public EscapeGameManager EGM;
    public Transform playerController;
    public GameObject locomotionSupport;
    public GameObject locomotionMenu;
    public SpawnPlayers playerSpawner;
    public Transform spawn;

    public GameObject[] hostButtons;

    public bool unityToHall = false;

    public int entranceUsed;

    void Start()
    {
        EGM = EscapeGameManager.Instance;
        spawn.position = EGM.spawnLocations.nextSpawn.position;
        spawn.rotation = EGM.spawnLocations.nextSpawn.rotation;
        playerController.transform.position = spawn.position;
        playerController.transform.rotation = spawn.rotation;

        studentInfo = GameObject.Find("PlayerData").GetComponent<StudentInfo>();

        playerSpawner.CreatePlayer();

        foreach (GameObject button in hostButtons)
        {
            button.SetActive(false);
        }
        
        GameState.Instance.roomComplete.AddListener(EnableButtons);
    }

    private void Update()
    {
        if (unityToHall)
        {
            unityToHall = false;
            Door1ToSecondFloorHall();
        }

        if(GameState.Instance.ActivePuzzle == null || GameState.Instance.ActivePuzzle.completed)
        {
            EnableButtons();
        }
    }

    public void EnableButtons()
    {
        if (PhotonNetwork.IsMasterClient)
        {
              foreach (GameObject button in hostButtons)
              {
                button.SetActive(true);
              }
        }
        
    }

    public void Door1ToSecondFloorHall()
    {
        Spawn temp = EGM.spawnLocations.hallway2Cafe1;
        EGM.gameObject.GetComponent<PhotonView>().RPC("UpdateSpawn", RpcTarget.All, temp.position.x, temp.position.y, temp.position.z, temp.rotation.y);
        StartCoroutine(Travel("LoadSecondFloorHall"));
    }

    public void Door2ToSecondFloorHall()
    {
        Spawn temp = EGM.spawnLocations.hallway2Cafe2;
        EGM.gameObject.GetComponent<PhotonView>().RPC("UpdateSpawn", RpcTarget.All, temp.position.x, temp.position.y, temp.position.z, temp.rotation.y);
        StartCoroutine(Travel("LoadSecondFloorHall"));
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
            EnableButtons();
        }
    }
}
