using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class SpawnPlayers : MonoBehaviourPunCallbacks
{
    public Transform spawn;

    public void CreatePlayer()
    {
        GameObject spawned = PhotonNetwork.Instantiate("NetworkPlayer", spawn.position, spawn.rotation);
    }
}

