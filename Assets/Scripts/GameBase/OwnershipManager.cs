using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using OculusSampleFramework;

public class OwnershipManager : MonoBehaviourPun
{
    const int PICKUPS = 8;

    private PhotonView view = null;
    private DistanceGrabbable grab = null;

    private void transferOwnership()
    {
        if (view.IsMine)
        {
            return;
        }

        view.RequestOwnership();
        return;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != PICKUPS)
        {
            return;
        }

        view = other.GetComponent<PhotonView>();
        grab = other.GetComponent<DistanceGrabbable>();
        
        if (view == null || grab == null)
        {
            return;
        }

        if (grab.isGrabbed)
        {
            return;
        }

        transferOwnership();
    }
}
