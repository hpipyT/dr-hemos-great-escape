using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnSyncedObjects : MonoBehaviourPunCallbacks
{
    public GameObject[] syncedObjects;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject syncedObject in syncedObjects)
        {
            PhotonNetwork.Instantiate(syncedObject.name, Vector3.zero, Quaternion.identity);
        }
    }
}
