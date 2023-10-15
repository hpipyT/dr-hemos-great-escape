using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.XR.CoreUtils;

public class NetworkPlayer : MonoBehaviour
{
    private Transform headRig;
    private Transform leftHandRig;
    private Transform rightHandRig;

    private PhotonView photonView;

    public Transform head;
    public Transform leftHand;
    public Transform rightHand;
    
    public GameObject rig;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        XROrigin rig = FindObjectOfType<XROrigin>();
        headRig = rig.transform.Find("Camera Offset/Main Camera");
        leftHandRig = rig.transform.Find("Camera Offset/LeftHand Controller");
        rightHandRig = rig.transform.Find("Camera Offset/RightHand Controller");


        if (photonView.IsMine)
        {
            head.gameObject.SetActive(false);
            leftHand.gameObject.SetActive(false);
            rightHand.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            MapPosition(head, headRig);
            MapPosition(leftHand, leftHandRig);
            MapPosition(rightHand, rightHandRig);
        }
    }

    void MapPosition(Transform target, Transform rigTransform)
    {
        
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }
}
