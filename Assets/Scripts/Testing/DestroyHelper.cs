using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyHelper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DestroyEscapeGameManager(GameObject me)
    {
        Destroy(me);
        // absolutely feel free to turn this on if you want no DestroyHelper spam
        // coooould lag and GameObject.Find() in EscapeGameManager could pick this up and null reference
        // Destroy(gameObject, 0.1f);
        SceneManager.LoadScene("LoginScene");
    }

    public IEnumerator DestroyIt(GameObject me)
    {
        Debug.Log("Destroying EscapeGameManager");
        Destroy(me);
        Debug.Log("EscapeGameManager Destroyed");
        yield
        return null;
        Destroy(this);
    }
    public void LoadHomeScreen()
    {
        StartCoroutine(ProceedToNextScene());
    }

    public IEnumerator ProceedToNextScene()
    {
        Debug.Log("Waiting to load Login...");
        yield
        return new WaitForSeconds(1.0f);
        Debug.Log("Loading Login");
        SceneManager.LoadScene("LoginScene");
        Debug.Log("Login Loaded");
        Destroy(this);
        yield
        return null;
    }

}