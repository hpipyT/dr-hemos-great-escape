using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Puzzle
{
    PhotonView pv;
    // notes: cafeteria, library, etc

    public bool loaded;
    public float runningTime;
    public float completionTime;
    public bool completed;
    public string room;
    public int idNo;
    public PuzzleStep currentPuzzleStep;
    public int puzzleStepNum;
    public PuzzleStep[] puzzleSteps;

    [SerializeField]
    public string[] puzzleStepPrefabs;
    public bool[] hintsTaken;

    public GameState gameState;

    [SerializeField]
    public string AnswerLetter;

    public PuzzleBaseObject puzzleBaseReference;

    public void Awake()
    {
        loaded = false;
        gameState = GameState.Instance;
    }
    public int GetPuzzleID()
    {
        return this.idNo;
    }

    public void SetPuzzleID(int number)
    {
        this.idNo = number;
    }

    public void CompletePuzzle()
    {
        if (GameState.Instance.OfflineMode)
        {
            completionHelper();
        }
        else
        {
            pv = puzzleBaseReference.photonView;
            pv.RPC("puzzleCompletion", RpcTarget.All);
        }
        gameState.CompletePuzzle();
    }

    public void completionHelper()
    {
        completionTime = runningTime;
        completed = true;
        for (int i = 0; i < puzzleSteps.Length; i++)
        {
            if (puzzleSteps[i].stepNo > 999)
            {
                currentPuzzleStep = puzzleSteps[i];
                currentPuzzleStep.activate();
                currentPuzzleStep.stepCompleted = true;
            }
        }
    }

    public void nextStep()
    {
        if (completed) return;

        if (puzzleStepNum == puzzleSteps.Length)
        {
            // will set completed to true
            CompletePuzzle();

        }
        else
        {
            //For the initial puzzle load, extra things may happen here.
            if (puzzleStepNum == -1)
            {
                Debug.Log("Loading in puzzle.");
            }
            else
            {
                Debug.Log("puzzle step done with - deactivating");
                currentPuzzleStep.stepCompleted = true;
                currentPuzzleStep.deactivate();
            }

            puzzleStepNum++;
            Debug.Log("moving to step " + puzzleStepNum + " of " + puzzleSteps.Length);
            currentPuzzleStep = puzzleSteps[puzzleStepNum];
            currentPuzzleStep.activate();
            if (currentPuzzleStep.stepNo > 999)
            {
                CompletePuzzle();
            }
            gameState.UpdateHintVisuals();
            Debug.Log("Moving onto step " + puzzleStepNum);
        }

    }

    public Puzzle(string name)
    {
        runningTime = 0;
        completionTime = -1;
        completed = false;
        room = name;
        idNo = 0;
        currentPuzzleStep = null;
        puzzleStepNum = -1;
        puzzleSteps = null;
        hintsTaken = null;
        gameState = GameState.Instance;
    }
}