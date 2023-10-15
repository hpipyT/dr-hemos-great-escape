using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RenameKey : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject key;
    void Start()
    {
        key = transform.parent.parent.gameObject;
        TextMeshProUGUI keycap = GetComponent<TextMeshProUGUI>();
        if (keycap && key)
            keycap.text = key.name;
    }
}
