using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IncorrectHelper : MonoBehaviour
{
    public Animation anim;
    public Image box;
    public TextMeshProUGUI text;

    int incorrectStep = -1;
    // Start is called before the first frame update
    void Start()
    {
        if (anim == null)
        {
            anim = this.GetComponent<Animation>();
        }
        if (box == null)
        {
            box = this.GetComponent<Image>();
        }
        if (text == null)
        {
            text = this.GetComponentInChildren<TextMeshProUGUI>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (anim.isPlaying || box.color != Color.clear)
        {
            if (GameState.Instance.ActivePuzzle.currentPuzzleStep.stepNo != incorrectStep)
            {
                anim.Stop();
                box.color = Color.clear;
                text.color = Color.clear;
            }
        }
    }

    public void Disappear()
    {
        anim.Stop();
        box.color = Color.clear; ;
        text.color = Color.clear;
    }

    public void Appear()
    {
        incorrectStep = GameState.Instance.ActivePuzzle.currentPuzzleStep.stepNo;
        anim.Play();
    }
}