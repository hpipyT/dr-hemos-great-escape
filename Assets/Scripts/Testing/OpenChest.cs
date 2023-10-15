using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenChest : MonoBehaviour
{
    public Animator animator;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Key")
        {
            animator.SetTrigger("Open Chest");
        }
    }
}
