using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    LineRenderer lr;
    // Start is called before the first frame update
    void Start()
    {
        lr = gameObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        lr.SetPosition(0, gameObject.transform.position);

        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.position, transform.TransformDirection(Vector3.forward), out hit))
        {
            Debug.Log("Hit");
            if (hit.collider != null) lr.SetPosition(1, hit.point);
        }

    }
}