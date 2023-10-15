using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubmitByKeyboardEnter : MonoBehaviour
{
    // Start is called before the first frame update

    public Keyboard keyboard;
    public Button button;

    void Start()
    {
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (keyboard.isSubmitted)
            button.onClick.Invoke();
    }
}
