using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsButtonPressed : MonoBehaviourPun
{

    public bool isPressed = false;
    public bool unityCallHintExperimental = false;


    public CreateAndJoinRooms roomManager;
    public string iden;

    bool roomTesting = false;



    void Update()
    {
        if (unityCallHintExperimental)
        {
            unityCallHintExperimental = false;
            IsButtonPressFork();
        }


        // hint button function
        if (isPressed && iden == "hint")
        {
            IsButtonPressFork();
        }
        // Answer button function
        else if (isPressed && iden == "answer")
        {
            IsButtonPressFork();
        }
        // quick method for testing create and join rooms
        else if (isPressed && roomTesting)
        {
            SetPress(false);
            if (iden == "create")
            {
                roomManager.CreateRoom();
            }
            else
            {
                roomManager.JoinRoom();
            }
        }
        else if(isPressed && iden == "tutorial")
        {
            IsButtonPressFork();
        }
        else if(isPressed && iden == "credits")
        {
            IsButtonPressFork();
        }

        if(isPressed)
        {
            SetPress(false);
        }
    }

    public void SetPress(bool i)
    {
        isPressed = i;
    }

    public void IsButtonPressFork()
    {
        if (iden == "answer")
        {
            gameObject.GetComponent<ButtonActivate>().SetButtonActive();
            return;
        }

        else if(iden == "credits")
        {
            gameObject.GetComponent<CreditScroller>().StartScrolling();
        }

        else 
        {
        if (GameState.Instance == null || GameState.Instance.OfflineMode)
        {
            RealButtonPress();
        }
        else
        {
            photonView.RPC("RealButtonPress", RpcTarget.All);
        }
        }
    }

    [PunRPC]
    public void RealButtonPress()
    {
        SetPress(false);
        if (iden == "hint")
        {
            // call another script attached to this object, *sound of falling tin cans*
            gameObject.GetComponent<ActivateHint>().SetHintActive();
        }        
        else if(iden =="tutorial")
        {
            gameObject.GetComponent<TutorialButtonActivate>().SetButtonActive();
        }

    }

}
