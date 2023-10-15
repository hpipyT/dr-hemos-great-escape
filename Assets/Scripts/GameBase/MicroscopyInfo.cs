using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MicroscopyInfo : MonoBehaviour
{
    public TextMeshProUGUI timeField;
    public TextMeshProUGUI hintsField;
    public CreditScroller credits;
    // Start is called before the first frame update
    void Start()
    {
        if (credits != null)
        {
            credits.StartScrolling();
        }
        UpdateFieldData();
    }

    public void UpdateFieldData()
    {
        TimeSpan t = TimeSpan.FromSeconds(GameState.Instance.totalRunTime);
        TimeSpan testSpan = TimeSpan.FromSeconds(GameState.Instance.totalRunTime);
        int minutes = testSpan.Minutes;
        int seconds = testSpan.Seconds;

        string displayString = testSpan.ToString("mm':'ss");

        Debug.Log("Total time to complete was " + t);

        timeField.text = displayString;

        int hintsUsed = GameState.Instance.AllHintsTaken().Count;

        hintsField.text = hintsUsed.ToString();
    }
}