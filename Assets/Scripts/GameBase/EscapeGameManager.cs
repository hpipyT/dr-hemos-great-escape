using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class EscapeGameManager : MonoBehaviourPunCallbacks
{
    public static EscapeGameManager Instance { get; private set; }
    public SpawnLocations spawnLocations;

    public bool gameStarted = false;
    public bool gameCompleted = false;

    public bool gameQuit = false;
    public bool gameQuitRUSure = false;
    public bool removeFromAttempt = false;
    public List<GameObject> destroyOnLeave;

    public bool teleportLocomotion = true;

    private void Awake()
    {
        if (EscapeGameManager.Instance != null && EscapeGameManager.Instance != this)
        {
            Destroy(this);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }

    private void Update()
    {
        if (gameQuit && gameQuitRUSure)
        {
            gameQuit = false;
            gameQuitRUSure = false;
            QuitGame(removeFromAttempt);
        }
    }

    void Start()
    {
        PhotonView pv = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void LoadSecondFloorHall()
    {
        if (!gameStarted)
        {
            gameStarted = true;
        }

        PhotonNetwork.LoadLevel("SecondFloor");
    }

    [PunRPC]
    public void LoadLobby()
    {
        PhotonNetwork.LoadLevel("LobbyScene");
    }

    [PunRPC]
    public void LoadCafeteria()
    {
        PhotonNetwork.LoadLevel("CafeteriaPuzzles");
    }

    [PunRPC]
    public void LoadLibrary()
    {
        PhotonNetwork.LoadLevel("LibraryPuzzles");
    }

    [PunRPC]
    public void LoadMicroscopyLab()
    {
        PhotonNetwork.LoadLevel("MicroscopyLab");
    }

    public void QuitGame(bool removeFromDB)
    {
        if (removeFromDB)
        {
            removeFromDB = false;
            // db
            StudentInfo student = GameObject.Find("PlayerData").GetComponent<StudentInfo>();
            GameObject db = GameObject.Find("DBAccess");
            db.GetComponent<RemovePlayer>().RemoveThem(student.SID, student.GAMEid);
        }

        Application.Quit();
    }

    [PunRPC]
    public void UpdateSpawn(float x, float y, float z, float rotY)
    {
        spawnLocations.nextSpawn = new Spawn(new Vector3(x, y, z), new Quaternion(0, rotY, 0, 0));
    }
}
