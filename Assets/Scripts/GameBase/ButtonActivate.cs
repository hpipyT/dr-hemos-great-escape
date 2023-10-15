using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActivate : MonoBehaviour
{

    // Remember to make sure "iden" in the IsButtonPressed script attached to this gameobject is set to "answer"

    // Set if for some reason there is a button that does not correspond to a puzzle step entry box.
    public PuzzleBaseObject puzzleBase;

    // Set this to the step this button is inputting an answer for.
    public PuzzleStep relevantStep;

    public string answer = "";

    public void SetButtonActive()
    {
        PuzzleStep step = GetCurrentStep();

        // If the answer is not empty

        if (answer != "" && step != null)
        {
            step.solutionAttempt(answer);
        }
    }

    // puzzlebase has the current puzzle thisPuzzle
    // check each puzzlestep if active
    // List<PuzzleStep> AllPuzzleSteps;

    public PuzzleStep GetCurrentStep()
    {

        return GameState.Instance.ActivePuzzle.currentPuzzleStep;

        // Yeah i know, this was probably the play from teh start.
        if (relevantStep != null)
        {
            return relevantStep;
        }

        // TODO: WILL FAIL, BECAUSE PUZZLE BASE IS NOT GETTING SET DYNAMICALLY.
        // FOR THE MEANTIME, SET IT MANUALLY
        foreach (PuzzleStep step in GameState.Instance.ActivePuzzle.puzzleSteps)
        {
            if (step.isActive)
            {
                return step;
            }
        }
        Debug.Log("No active step, how'd you get here?");
        return null;
    }

    // Makes sure the step and or puzzle base are not null, so the button can actually work.
    private void Start()
    {
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