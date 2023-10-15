using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class InputSelection : MonoBehaviour
{
    public GameObject keyboard;
    public OVRManager oVRManager;
    public TMP_InputField[] inputFields;

    bool inputActive;

    void Start()
    {
        
    }

    public void AssignInputField(TMP_InputField field)
    {
        keyboard.GetComponent<Keyboard>().inputField = field;
    }
}
