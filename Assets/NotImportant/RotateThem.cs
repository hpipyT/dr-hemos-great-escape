using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateThem : MonoBehaviour
{

    Vector3 spin;
    float spintensity = 2.0f;
    
    int clicksToLaunch = 3;
    float disToOrbit = 1.0f;

    bool hasReachedOrbit = false;
    bool isHovering = false;

    void Start()
    {
        spin = new Vector3(0.0f,0.0f,spintensity);
    }

    void Update()
    {
        gameObject.transform.Rotate(spin, Space.Self);

        // this is gonna turn into a yandere dev 20 line if statement
        if (spin.z > Mathf.Pow(spintensity, clicksToLaunch) && !(hasReachedOrbit))
            BlastOff();

        else if (hasReachedOrbit)
        {
            Hover();
        }
        else if (isHovering)
        {
            ;
        }
    }

    void OnMouseDown()
    {
        spin.z *= 2;
        Debug.Log("aiwuhdlia! !!");
    }

    void BlastOff()
    {
        gameObject.transform.Translate(0.0f,0.0f,0.00075f, Space.Self);
        disToOrbit -= 0.001f;
        if(disToOrbit <= 0)
            hasReachedOrbit = true;
    }

    void Hover()
    {
        if (isHovering == false)
            isHovering = true;

        // parameters
        float moveScale = 0.00025f;
        // float timeScale = 5.0f;

        // hover starts jittery because start position depends on time
        float bleh = moveScale * Mathf.Clamp(Mathf.Sin(Time.time), -1.0f, 1.0f);

        gameObject.transform.Translate(0.0f, 0.0f, bleh, Space.Self);
    }
}
