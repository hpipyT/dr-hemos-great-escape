using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // get ENV vars // they're set in Azure
        // System.GetEnvironmentVariable();

        // set in DBControl.cs *done already on GitHub

        // *on server    
        // reference config.php

        // In own PHP scripts, change PDO to mysqli_init() for convention
        // Unity.CS method OK
        // add formatting to output

        // Convention:
        // https://github.com/benigmatic/php-mysql-app/blob/main/index.php
    }

    void Awake()
    {
        GameObject.Find("EscapeGameManager").GetComponent<EscapeGameManager>().destroyOnLeave.Add(gameObject);
    }
}