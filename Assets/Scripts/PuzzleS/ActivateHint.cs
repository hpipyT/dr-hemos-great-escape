using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateHint : MonoBehaviour
{
    public PuzzleBaseObject puzzleBase;
    public GameState gameState;

    public void Start()
    {
        gameState = GameState.Instance;
    }

    public void SetHintActive()
    {
        PuzzleStep step = GetCurrentStep();

        Debug.Log("step is " + step.stepNo);
        Debug.Log("is hints null?" + step.hints.Equals(null));
        bool allHintsActive = true;
        for (int i = 0; i < step.hints.Length; i++)
        {
            if (!step.hints[i].hintTaken)
            {
                allHintsActive = false;
            }
        }

        if (allHintsActive)
        {
            return;
        }
        // if the step has hints
        if (step.hints != null && step.hints.Length > 0)
        {
            // set the most recent, unused hint as used
            for (int i = 0; i < step.hints.Length; i++)
            {
                if (step.hints[i].hintTaken == false)
                {
                    //step.hints[i].hintTaken = true;
                    step.hints[i].ActivateHint();
                    Debug.Log("Hint activated");

                    break;
                }
                else
                {
                    step.hints[i].DeactivateHint();
                }
            }
        }
        else Debug.Log("No hints for this step");
    }

    // puzzlebase has the current puzzle thisPuzzle
    // check each puzzlestep if active
    // List<PuzzleStep> AllPuzzleSteps;

    public PuzzleStep GetCurrentStep()
    {

        if (gameState.ActivePuzzle.currentPuzzleStep != null)
        {
            return gameState.ActivePuzzle.currentPuzzleStep;
        }
        if (puzzleBase != null)
        {
            Debug.Log("Puzzle base real AF");
            for (int i = 0; i < puzzleBase.AllPuzzleSteps.Count; i++)
            {
                if (puzzleBase.AllPuzzleSteps[i].stepNo == gameState.ActivePuzzle.puzzleStepNum)
                {
                    return puzzleBase.AllPuzzleSteps[i];
                }
            }

            return puzzleBase.thisPuzzle.currentPuzzleStep;
        }
        else if (gameState.ActivePuzzle.puzzleBaseReference != null)
        {
            for (int i = 0; i < gameState.ActivePuzzle.puzzleBaseReference.AllPuzzleSteps.Count; i++)
            {
                if (gameState.ActivePuzzle.puzzleBaseReference.AllPuzzleSteps[i].stepNo == gameState.ActivePuzzle.puzzleStepNum)
                {
                    return gameState.ActivePuzzle.puzzleBaseReference.AllPuzzleSteps[i];
                }
            }

        }

        Debug.Log("No active step, how'd you get here?");
        return null;
    }

}