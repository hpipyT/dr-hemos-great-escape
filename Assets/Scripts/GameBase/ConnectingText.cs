using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConnectingText : MonoBehaviour
{
    public TMP_Text text;

    const float DELAY = 0.3f;
    const string ORIGINAL_TEXT = "Connecting to server";

    int dotCount;
    float timeElapsed;

    private void Start()
    {
        dotCount = 0;
        text.text = ORIGINAL_TEXT;
        timeElapsed = 0f;
    }

    private void Update()
    {
        if (timeElapsed < DELAY)
        {
            timeElapsed += Time.deltaTime;
            return;
        }

        timeElapsed = 0f;

        if (dotCount < 3)
        {
            dotCount++;
            text.text += ".";
        }
        else
        {
            dotCount = 0;
            text.text = ORIGINAL_TEXT;
        }
    }
}
