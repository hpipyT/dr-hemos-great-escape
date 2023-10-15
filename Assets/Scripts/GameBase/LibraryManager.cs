using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LibraryManager : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public StudentInfo studentInfo;
    public EscapeGameManager EGM;
    public Transform playerController;
    public GameObject locomotionSupport;
    public GameObject locomotionMenu;
    public SpawnPlayers playerSpawner;
    public Transform spawn;

    public GameObject[] hostButtons;

    public AudioSource ambient;
    public PuzzleBaseObject puzBase;

    public bool unityToHall = false;

    void Start()
    {
        studentInfo = GameObject.Find("PlayerData").GetComponent<StudentInfo>();
        EGM = EscapeGameManager.Instance;

        playerSpawner.CreatePlayer();


            foreach (GameObject button in hostButtons)
            {
                button.SetActive(false);
            }
        

        
        puzBase = GameObject.Find("PUZZLE BASE").GetComponent<PuzzleBaseObject>();
        GameState.Instance.roomComplete.AddListener(EnableButtons);
        ambient = EscapeGameManager.Instance.gameObject.GetComponentInChildren<AudioSource>();
        //ambient = GameObject.Find("Ambient").GetComponent<AudioSource>();
        
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

    private void Update()
    {
        if (unityToHall)
        {
            unityToHall = false;
            ToSecondFloorHall();
        }
    }

    void FixedUpdate()
    {
        if(puzBase == null || !puzBase.thisPuzzle.loaded)
        {
            return;
        }

        if(ambient == null)
        {
            ambient = EscapeGameManager.Instance.GetComponentInChildren<AudioSource>();
        }

        if(puzBase.thisPuzzle.currentPuzzleStep.stepNo == 4 && puzBase.thisPuzzle.currentPuzzleStep.hints[0].hintActive)
        {
            ambient.volume = 0f;
        }
        else if(puzBase.thisPuzzle.currentPuzzleStep.stepNo == 6 && puzBase.thisPuzzle.currentPuzzleStep.hints[0].hintActive)
        {
            ambient.volume = 0f;
        }
        else if(puzBase.thisPuzzle.currentPuzzleStep.stepNo >= 1000)
        {
            ambient.volume = 0f;
            if(GameState.Instance.isRunning)
            {
                GameState.Instance.SetRunning(false);
            }
        }
        else if(ambient.volume == 0f)
        {
            ambient.volume = .25f;
        }

        if(GameState.Instance.ActivePuzzle == null || GameState.Instance.ActivePuzzle.completed)
        {
            EnableButtons();
        }
    }

    public void ToSecondFloorHall()
    {
        GameState.Instance.SetRunning(true);
        ambient.enabled = true;
        ambient.volume = .25f;
        Spawn temp = EGM.spawnLocations.hallway2Library;
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
