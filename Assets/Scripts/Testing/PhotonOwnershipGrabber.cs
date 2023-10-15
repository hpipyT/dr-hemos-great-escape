using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonOwnershipGrabber : MonoBehaviourPun
{
    const int PICKUP_LAYER = 8;

    public PhotonView myView;

    private Collider heldObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == PICKUP_LAYER)
        {
            var objectView = other.GetComponent<PhotonView>();
            if (objectView == null)
            {
                return;
            }

            var syncedObject = other.GetComponent<SyncedObject>();
            if (syncedObject == null)
            {
                return;
            }

            if (other.GetComponent<SyncedObject>().canTransfer)
            {
                other.GetComponent<PhotonView>().RequestOwnership();
                heldObject = other;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == heldObject) heldObject.GetComponent<SyncedObject>().canTransfer = true;
    }
}