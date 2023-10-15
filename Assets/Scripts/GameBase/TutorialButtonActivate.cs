using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialButtonActivate : MonoBehaviour
{

    // Remember to make sure "iden" in the IsButtonPressed script attached to this gameobject is set to "tutorial"
    
    public List<GameObject> slides;
    int activeIndex;
    

    public void SetButtonActive()
    {  
        nextSlide();
    }

    void nextSlide()
    {
        activeIndex++;
        if(activeIndex >= slides.Count)
        {
            activeIndex = 0;
        }
        for(int i = 0; i < slides.Count; i++)
        {
            if(i == activeIndex)
            {
                slides[i].SetActive(true);
            }
            else
            {
                slides[i].SetActive(false);
            }
        }
    }


    // Makes sure the step and or puzzle base are not null, so the button can actually work.
    private void Start()
    {
        if(slides == null)
        {
            slides = new List<GameObject>();
        }
        activeIndex = 0;
        /*if(puzzleBase == null)
        {
            if(relevantStep != null)
            {
                puzzleBase = relevantStep.parentPuzzle.puzzleBaseReference;
            }
            else
            {
                puzzleBase = GameObject.Find("PUZZLE BASE").GetComponent<PuzzleBaseObject>();
            }
        }*/
    }


    // get current step from thisPuzzle
    // get Puzzle class
    // PuzzleStep currentPuzzleStep

    // if this puzzle has a hint 

    // set hint as active 
}
