using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ButtonPress : MonoBehaviourPun
{

    GameObject button;

    [SerializeField]
    public float InvulSeconds = 5f;

    bool readyToPressAgain = true;

    float finalYPos;
    float baseYPos;

    PhotonView pv;

    private void Start()
    {
        button = gameObject.transform.parent.GetChild(1).gameObject;
        baseYPos = button.transform.position.y;
        finalYPos = baseYPos - .07f;
        alreadyMoving = false;
        depressing = false;
    }
    // pressing the button
    private void OnTriggerEnter(Collider other)
    {
        if(!readyToPressAgain || alreadyMoving)
        {            
            return;
        }

        if(pressed)
        {
            return;
        }

        if (other.name != "Button" && other.gameObject.tag == "Player")
        {
            ButtonPressFork();
        }
    }

    public void ButtonPressFork()
    {
        pv = GetComponent<PhotonView>();
        if(GameState.Instance == null || GameState.Instance.OfflineMode || pv == null)
        {
            pubButtonPress();
        }
        else
        {
            Debug.Log("Schneep schnorp");
            //pubButtonPress();
            pv.RPC("pubButtonPress", RpcTarget.All);
        }
    }


    [PunRPC]
    public void pubButtonPress()
    {
        Debug.Log("Schneep schnorp2");
        transform.parent.gameObject.GetComponent<IsButtonPressed>().SetPress(true);
        StartCoroutine(ButtonMove());
        StartCoroutine(WaitForRepress());
    }


        bool alreadyMoving;
        bool depressing;
        public float moveSpeed = .15f;

       IEnumerator ButtonMove()
        {

        float distance = finalYPos - button.transform.position.y;
        Vector3 newPos = button.transform.position;

        if(distance > 0)
        {
            yield break;
        }
        else if ((newPos.y + distance) < finalYPos)
        {
            yield break;
        }
        alreadyMoving = true;

        float newDuration = Mathf.Abs(distance / moveSpeed);
        float time = 0;
        while(time < newDuration)
        {
            newPos = button.transform.position;
            newPos.y += -1 * Mathf.Abs(moveSpeed * Time.fixedDeltaTime);
            if(newPos.y < finalYPos)
            {
                alreadyMoving = false;
                yield break;
            }
            time += Time.fixedDeltaTime;
            button.transform.position = newPos;
            yield return new WaitForFixedUpdate();
        }
        alreadyMoving = false;
        yield break;
    }

    void FixedUpdate()
    {
        if(!alreadyMoving && !depressing)
        {
            if(button.transform.position.y < baseYPos)
            {
                if(Mathf.Abs(button.transform.position.y - baseYPos) > 0.02)
                {
                    keyUnmoveHelper();
                }
                
            }
        }
    }

    bool pressed = false;

    [PunRPC]
    public void keyUnmoveHelper()
    {
        StartCoroutine(KeyUnmove());
    }
    public void startKeyUnmove()
    {
        if(GameState.Instance.OfflineMode)
        {
        keyUnmoveHelper();
        }
        else
        {
        photonView.RPC("keyUnmoveHelper", RpcTarget.All);
        }
    }
    IEnumerator KeyUnmove()
    {
        Debug.Log("unmoving");
        //while(alreadyMoving)
      //  {
        //    yield return new WaitForFixedUpdate();
       // }
        float distance = baseYPos - button.transform.position.y;
                
        Vector3 newPos = button.transform.position;

        if(distance < 0)
        {
            yield break;
        }
        else if ((newPos.y + distance) > baseYPos)
        {
            yield break;
        }
        depressing = true;
        float duration = Mathf.Abs(distance / moveSpeed);
        float currentTime = 0;
        while(currentTime < duration)
        {
            newPos = button.transform.position;
            newPos.y += (moveSpeed * Time.fixedDeltaTime);
            if(newPos.y > baseYPos)
            {
                depressing = false;
                yield break;
            }
            currentTime += Time.fixedDeltaTime;
            button.transform.position = newPos;
            yield return new WaitForFixedUpdate();
        }

        depressing = false;
        pressed = false;
        yield break;
    }


    IEnumerator WaitForRepress()
    {
        readyToPressAgain = false;
        yield return new WaitForSeconds(InvulSeconds);
        readyToPressAgain = true;
    }

    // unpressing the button
    private void OnTriggerExit(Collider other)
    {
        if (other.name != "Button"  && other.gameObject.tag == "Player")
        {
            // get button object
            //GameObject button = gameObject.transform.parent.GetChild(1).gameObject;

            // translate button up
            //button.transform.position = new Vector3(button.transform.position.x, button.transform.position.y + 0.05f, button.transform.position.z);

            // unset Button state to Pressed
            transform.parent.gameObject.GetComponent<IsButtonPressed>().SetPress(false);
        }
    }

}
