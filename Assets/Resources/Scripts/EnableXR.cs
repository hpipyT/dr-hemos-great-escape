using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnableXR : MonoBehaviour
{
    public PhotonView myPV;
    public GameObject[] xRObjects;

    // Start is called before the first frame update
    void Start()
    {
        if (myPV.IsMine)
        {
            foreach (GameObject xRObject in xRObjects)
            {
                xRObject.SetActive(true);
            }
        }
    }
}
