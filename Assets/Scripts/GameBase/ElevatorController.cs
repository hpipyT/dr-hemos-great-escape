using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ElevatorController : MonoBehaviourPun
{
    public void Open()
    {
        GetComponent<PhotonView>().RPC("OpenForAll", RpcTarget.All);
    }

    [PunRPC]
    public void OpenForAll()
    {
        GetComponent<Animator>().SetBool("Open", true);
    }
}
