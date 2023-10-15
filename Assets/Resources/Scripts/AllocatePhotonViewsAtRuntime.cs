using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AllocatePhotonViewsAtRuntime : MonoBehaviour
{
    public PhotonView[] objectsToSync;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (PhotonView objectToSync in objectsToSync)
            {
                PhotonNetwork.AllocateViewID(objectToSync);
            }
        }
    }
}   
