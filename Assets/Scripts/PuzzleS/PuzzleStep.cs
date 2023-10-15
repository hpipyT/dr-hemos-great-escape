using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PuzzleStep : MonoBehaviourPun
{

    public enum AnswerType
    {
        MultipleChoice,
        FillInBlank,
        Objective
    }

    public float runningTime;

    public float timePenalty = 0;

    public bool stepCompleted;

    public bool unitySkipStepExperimental = false;

    public Hint[] hints;
    public string question;
    public bool isHint;
    public bool isActive;
    public AnswerType answerType;
    public string[] solutionSet;
    public PuzzleObject[] targetObjects;

    public List<string> incorrectAnswers;

    public int stepNo;

    public Puzzle parentPuzzle;

    // DB
    bool dbEnabled;

    bool wrongAnswer = false;

    public GameObject db;
    public DataExporter exporter;

    // Object Collision
    public void objectiveComplete()
    {

        if (GameState.Instance.OfflineMode)
        {
            realObjectiveComplete();
        }
        else
        {
            photonView.RPC("realObjectiveComplete", RpcTarget.All);
        }
    }

    [PunRPC]
    public void realObjectiveComplete()
    {

        if (isHint)
        {
            stepCompleted = true;
            //this.deactivate();
        }
        else
        {

            bool dbEnabled = GameState.Instance.dbActive;
            GameObject db = GameObject.Find("DBAccess");

            if (dbEnabled && stepNo != GameState.Instance.ActivePuzzle.puzzleSteps.Length && stepNo > 0)
            {
                try
                {
                    exporter = db.GetComponent<DataExporter>();
                    PuzzleStepData data = exporter.GetCurrentPuzzleStepData();

                    Debug.Log("Updated PuzzleStep in DB");
                    db.GetComponent<UpdatePuzzleStep>().OnSubmit(data);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
                // update PuzzleStep with GAMEid = ?, PuzzleName = ?, PuzzleStep = ?

            }

            if (GameState.Instance.ActivePuzzle.currentPuzzleStep == this)
            {
                parentPuzzle.puzzleBaseReference.CorrectSound();
                GameState.Instance.ActivePuzzle.nextStep();
                if (incorrect != null)
                {
                    incorrect.Disappear();
                }

            }
            // calls Puzzle.nextStep()
        }

    }
    public void SolutionComplete()
    {
        if (incorrect != null)
        {
            incorrect.Disappear();
        }

        realObjectiveComplete();
    }

    string prevAns = "";

    [PunRPC]
    public void IncorrectAnswer(string answerGiven, float timePen)
    {
        if (answerGiven == "" || answerGiven == prevAns)
        {
            return;
        }
        incorrectAnswers.Add(answerGiven);
        prevAns = answerGiven;
        if (incorrect != null)
        {
            incorrect.Appear();
        }
        runningTime += timePen;
        GameState.Instance.ActivePuzzle.runningTime += timePen;
        GameState.Instance.totalRunTime += timePen;
    }

    public bool SpecialStringCompare(string a, string b)
    {
        string aAgain = a.Trim().Normalize();
        string bAgain = b.Trim().Normalize();

        char[] aAgainAgain = aAgain.ToCharArray();
        char[] bAgainAgain = bAgain.ToCharArray();

        Debug.Log("Comparing " + aAgainAgain.ToString() + " and " + bAgainAgain.ToString() + "\nOf lengths: " + aAgainAgain.Length + " and " + bAgainAgain.Length);
        if (a == b || a.Trim() == b.Trim() || a.Equals(b) || String.Compare(a, b) == 0 || String.Compare(a.Trim(), b.Trim()) == 0)
        {
            return true;
        }
        else if (aAgain == bAgain || aAgain.Trim() == bAgain.Trim() || aAgain.Equals(bAgain) || String.Compare(aAgain, bAgain) == 0)
        {
            return true;
        }
        else if (aAgainAgain == bAgainAgain)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    GameObject IncorrectPrefab;

    public IncorrectHelper incorrect = null;

    public void solutionAttempt(string answerGiven)
    {
        if (GameState.Instance.OfflineMode)
        {
            solutionAttemptHelper(answerGiven);
        }
        else
        {
            StartCoroutine(WaitForVerifySolution(answerGiven));
            photonView.RPC("solutionAttemptHelper", RpcTarget.All, answerGiven);
        }
    }

    public IEnumerator WaitForVerifySolution(string answerGiven)
    {
        yield
        return new WaitForSecondsRealtime(1.5f);

        // database
        dbEnabled = GameState.Instance.dbActive;
        // Send answer attempt to database
        if (dbEnabled && wrongAnswer)
        {
            wrongAnswer = false;
            bool dbEnabled = GameState.Instance.dbActive;
            GameObject db = GameObject.Find("DBAccess");

            StudentInfo student = GameObject.Find("PlayerData").GetComponent<StudentInfo>();

            if (student.justSubmitted)
            {
                student.justSubmitted = false;
                try
                {
                    // create PuzzleAnswer in database 
                    exporter = db.GetComponent<DataExporter>();
                    PuzzleAnswer data = exporter.GetPuzzleAnswerData(answerGiven);

                    db.GetComponent<CreatePuzzleAnswer>().OnSubmit(data);
                    Debug.Log("Created PuzzleAnswer in DB");
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }

        }
        yield
        return null;
    }

    [PunRPC]
    public bool solutionAttemptHelper(string answerGiven)
    {
        Debug.Log("Attempting solution " + answerGiven);
        if (answerGiven.Length > 1) answerGiven = answerGiven.Substring(0, answerGiven.Length - 1);
        answerGiven = answerGiven.Normalize();

        if (!isActive) return false;

        switch (answerType)
        {
            case AnswerType.MultipleChoice:
                if (answerGiven.ToLower() == solutionSet[0].ToLower())
                {
                    Debug.Log("Trying solution: " + answerGiven + "\nFor question: " + this.question + "\nIntended answer: " + solutionSet[0]);
                    Debug.Log("Correct choice!");
                    SolutionComplete();
                    return true;
                }
                break;
            case AnswerType.FillInBlank:
                foreach (string solution in solutionSet)
                {
                    Debug.Log("Trying solution: " + answerGiven + "\nFor question: " + this.question + "\nIntended answer: " + solution);
                    if (solution.ToLower() == answerGiven.ToLower())
                    {
                        Debug.Log("Correct solution!");
                        SolutionComplete();
                        return true;
                    }
                    else
                    {
                        Debug.Log("Incorrect solution...");
                        wrongAnswer = true;

                    }
                }
                break;
            case AnswerType.Objective:
                Debug.Log("Error: solutionAttempt() is called when answer type is objective.");
                return false;
            default:
                break;
        }

        IncorrectAnswer(answerGiven, timePenalty);

        return false;
    }

    Hint[] GetHints()
    {
        return this.gameObject.GetComponentsInChildren<Hint>();
    }

    [PunRPC]
    public void activate()
    {
        bool dbEnabled = GameState.Instance.dbActive;
        GameObject db = GameObject.Find("DBAccess");

        if (incorrect != null)
        {
            incorrect.Disappear();
        }
        // skip the buffer step at the end 
        if (dbEnabled && GameState.Instance.ActivePuzzle != null && stepNo != GameState.Instance.ActivePuzzle.puzzleSteps.Length && stepNo > 0 && stepNo < 100)
        {
            try
            {
                DataExporter exporter = db.GetComponent<DataExporter>();
                // exporter.gameState = GameObject.Find("GameState").GetComponent<GameState>();

                db.GetComponent<CreatePuzzleStep>().OnSubmit(exporter.GetCurrentPuzzleStepData());

            }
            catch (Exception e)
            {
                Debug.Log(e);
                Debug.Log("Failed to create data exporter");
            }
            // create PuzzleStep in database 
        }

        if (hints == null)
        {
            hints = GetHints();
        }
        runningTime = 0;
        incorrectAnswers = new List<string>();
        isActive = true;
        if (targetObjects != null)
        {
            foreach (PuzzleObject puzzleObject in targetObjects)
            {
                if (puzzleObject != null)
                {
                    puzzleObject.ActivateObject();
                }
            }
        }
        for (int i = 0; i < hints.Length; i++)
        {
            hints[i].hintNo = i;
        }

    }

    public void AddPuzzleObject(PuzzleObject addedObj)
    {
        PuzzleObject[] newList = new PuzzleObject[targetObjects.Length + 1];
        for (int i = 0; i < targetObjects.Length; i++)
        {
            newList[i] = targetObjects[i];
        }
        newList[targetObjects.Length] = addedObj;
    }

    // notes: disable the puzzle
    [PunRPC]
    public void deactivate()
    {
        isActive = false;
        for (int i = 0; i < hints.Length; i++)
        {
            if (hints[i].hintTaken && hints[i].hintActive)
            {
                hints[i].DeactivateHint();
            }
        }

        for (int i = 0; i < targetObjects.Length; i++)
        {
            targetObjects[i].DeactivateObject();
        }
    }

    public PuzzleStep(string question, string solution, AnswerType ansType)
    {
        this.question = question;
        this.answerType = ansType;
        this.stepCompleted = false;
        this.hints = null;
        this.isHint = false;
        this.isActive = false;
        this.solutionSet = new string[1] {
      solution
    };
    }

    public PuzzleStep(string question, PuzzleObject[] solutionObj)
    {
        this.question = question;
        this.answerType = AnswerType.Objective;
        this.stepCompleted = false;
        this.hints = null;
        this.isHint = false;
        this.isActive = false;
        this.targetObjects = solutionObj;
    }

    [PunRPC]
    public void TriggerAnimation(string paramName, Animator animator)
    {
        animator.SetBool(paramName, true);
    }

    [PunRPC]
    public void AltTriggerAnimation(string paramName, string objName)
    {
        Animator anim = null;
        Debug.Log("trying to trigger " + paramName + " for " + objName);
        List<Animator> anims = this.gameObject.GetComponentsInChildren<Animator>().ToList();
        Debug.Log("there are " + anims.Count + " anims to find");
        for (int i = 0; i < anims.Count; i++)
        {
            if (anims[i] == null)
            {
                continue;
            }
            if (anims[i].gameObject.name == objName)
            {
                anim = anims[i];
                anim.SetBool(paramName, true);
            }
        }

        if (anim == null)
        {
            for (int i = 0; i < targetObjects.Length; i++)
            {
                if (targetObjects[i] == null)
                {
                    continue;
                }
                if (targetObjects[i].name == objName)
                {
                    anim = targetObjects[i].GetComponentInChildren<Animator>();
                    if (anim != null)
                    {
                        anim.SetBool(paramName, true);
                        Debug.Log(anim);
                    }
                    else
                    {
                        Debug.Log(targetObjects[i].name + " had no anim in children??");
                    }
                }
            }

            for (int i = 0; i < hints.Length; i++)
            {
                for (int j = 0; j < hints[i].puzObjs.Count; j++)
                {
                    if (hints[i].puzObjs.ElementAt(j) == null)
                    {
                        continue;
                    }
                    if (hints[i].puzObjs.ElementAt(j).name == objName)
                    {
                        anim = hints[i].puzObjs.ElementAt(j).GetComponentInChildren<Animator>();
                        if (anim != null)
                        {
                            anim.SetBool(paramName, true);
                            Debug.Log(anim);
                        }
                        else
                        {
                            Debug.Log(hints[i].puzObjs.ElementAt(j).name + " had no anim in children??");
                        }
                    }
                }
            }
        }

        if (anim != null)
        {
            Debug.Log("Triggering animation " + paramName);
            anim.SetBool(paramName, true);
        }
        else
        {
            Debug.Log("THE ANIMATOR WAS NULL!!");
        }

    }

    public void Update()
    {
        if (unitySkipStepExperimental)
        {
            unitySkipStepExperimental = false;
            objectiveComplete();
        }
    }

}