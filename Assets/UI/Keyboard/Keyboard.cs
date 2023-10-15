using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class Keyboard : MonoBehaviourPun
{
    public TMP_InputField inputField;
    XRBaseInteractable selection;

    public bool isSubmitted = false;
    public bool submitTrueTest = false;
    public bool submitFalseTest = false;

    public void getInputField(TMP_InputField input)
    {
        if (inputField)
            inputField = input;
    }

    public void OnHoverEntered(HoverEnterEventArgs args)
    {
        for (int i = 0; i < 100; i++)
            Debug.Log("In panel");
        Debug.Log($"{args.interactorObject} hovered over {args.interactableObject}", this);
    }


    // TODO: GOTTA RPC-IZE THIS MF SOOOO BAD. GOTTA DO IT 
    public void AddToInput(string c)
    {
        if (inputField != null)
        {
            if (c == "Space")
                c = " ";
            inputField.text += c;
            Debug.Log("inputField: " + inputField.text);
            //inputField.onValueChanged.Invoke(inputField.text);
        }


    }

 
    public void Backspace()
    {
        if (inputField != null)
        {
            if (inputField.text.Length > 0)
            {
                inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
            }
        }
    }

    public void Submit()
    {
        if(GameState.Instance.OfflineMode)
        {
        SubmitHelper();
        }
        else
        {
            // flag submitter as such for sending info to db
            GameObject.Find("PlayerData").GetComponent<StudentInfo>().justSubmitted = true;
            // then
            photonView.RPC("SubmitHelper", RpcTarget.All);
        }
    }

    [PunRPC]
    public void SubmitHelper()
    {
        isSubmitted = true;
        StartCoroutine(AnswerTimer());
        inputField.onEndEdit.Invoke(inputField.text);
    }

    public IEnumerator AnswerTimer()
    {
        yield return new WaitForSeconds(2f);
        isSubmitted = false;
    }

    public void Update()
    {
        if (submitTrueTest)
        {
            inputField.text = "VASO-OCCLUSIVE CRISIS";
            Submit();
        }

        if (submitFalseTest)
        {
            submitFalseTest = false;
            isSubmitted = false;
            inputField.text = "wrong answer test";
            Submit();
        }
    }

}
