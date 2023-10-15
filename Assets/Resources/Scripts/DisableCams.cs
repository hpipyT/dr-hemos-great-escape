using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DisableCams : MonoBehaviour
{
    public PhotonView myPV;
    public GameObject[] toDisable;
    // Start is called before the first frame update
    void Start()
    {
        if(myPV.IsMine)
        {
            // set local player
        }
        if(!myPV.IsMine)
        {
            foreach (GameObject item in toDisable)
            {
                item.SetActive(false);
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
