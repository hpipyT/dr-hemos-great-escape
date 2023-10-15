using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using OculusSampleFramework;
using OVRTouchSample;

public class PhotonPlayerManager : MonoBehaviour
{
    public PhotonPlayerPasser passer;

    public Transform player;
    public GameObject avatar;

    public Hand handL;
    public Hand handR;
    public Animator netLeftHandAni;
    public Animator netRightHandAni;

    public Transform networkLeft;
    public Transform networkRight;
    public Transform localLeft;
    public Transform localRight;

    public SkinnedMeshRenderer leftMesh;
    public SkinnedMeshRenderer rightMesh;

    bool myView;

    private void Start()
    {
        myView = GetComponent<PhotonView>().IsMine;
        passer = GameObject.Find("PhotonPlayerPasser").GetComponent<PhotonPlayerPasser>();

        player = passer.player;
        handL = passer.handL;
        handR = passer.handR;
        localLeft = passer.leftHand;
        localRight = passer.rightHand;

        if (!myView)
        {
            leftMesh.enabled = true;
            rightMesh.enabled = true;
            avatar.SetActive(true);
            //GetComponent<CapsuleCollider>().enabled = true;
        }
        else
        {
            handL.networkHand = netLeftHandAni;
            handR.networkHand = netRightHandAni;
        }
    }

    private void Update()
    {
        if (!myView)
            return;

        transform.position = player.position;
        transform.rotation = player.rotation;

        //for (int i = 0; i < leftHandAni.parameterCount; i++)
        //{
        //    netLeftHandAni.SetFloat(i, leftHandAni.GetFloat(i));
        //    netRightHandAni.SetFloat(i, rightHandAni.GetFloat(i));
        //}

        networkLeft.position = localLeft.position;
        networkLeft.rotation = localLeft.rotation;
        networkRight.position = localRight.position;
        networkRight.rotation = localRight.rotation;
    }
}
