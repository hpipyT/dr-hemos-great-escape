using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PhotonPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject playerPrefab;
    PhotonView myPV;

    void Start()
    {
        Vector3 spawnPosition = new Vector3(0,0.5f,0);
        myPV = GetComponent<PhotonView>();
        if(myPV.IsMine)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
        }
    }
}
