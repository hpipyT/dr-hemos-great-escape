using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroscopyUI : MonoBehaviour
{    
    void Start()
    {
        StartCoroutine(setupUI());
    }

    IEnumerator setupUI()
    {
        yield return new WaitForSecondsRealtime(1);

        UIBuilder.instance.AddButton("Quit Game", QuitGame);
        UIBuilder.instance.Show();
    }

    void QuitGame()
    {
        EscapeGameManager.Instance.QuitGame(false);
    }
}
