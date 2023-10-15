using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class InputFieldFormatting : MonoBehaviour
{

    public TextMeshProUGUI inputOne;
    public TextMeshProUGUI inputTwo;
    public TMPro.TMP_InputField inputField;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ValidateInput(TMPro.TMP_InputField field)
    {
        string entry = field.text;
        entry = new string(entry.Where(c => char.IsDigit(c)).ToArray());
        Debug.Log("Cleaned entry is " + entry + ", len:" + entry.Length.ToString());

        if (entry.Length > 4)
        {
            entry = entry.Substring(0, 4);
        }

        field.text = entry;

        if (entry.Length > 0)
        {
            inputOne.text = entry.Substring(0, Mathf.Min(2, entry.Length));
        }
        else
        {
            inputOne.text = "";
        }
        if (entry.Length > 2)
        {
            inputTwo.text = entry.Substring(2, Mathf.Min(2, entry.Length - 2));
        }
        else
        {
            inputTwo.text = "";
        }

    }
}