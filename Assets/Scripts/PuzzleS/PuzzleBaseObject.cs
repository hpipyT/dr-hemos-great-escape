using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PuzzleBaseObject : MonoBehaviourPun
{
    [SerializeField]
    public Puzzle thisPuzzle;

    [SerializeField]
    string roomName;

    [SerializeField]
    string SolutionLetter;

    public List<PuzzleObject> AllPuzzleObjects;

    GameState gameState;

    // made this public for hint activation
    public List<PuzzleStep> AllPuzzleSteps;

    PhotonView pv;

    public IncorrectHelper incorrect;

    public AudioClip correct;

    public AudioSource noisePlayer;

    public void AddPuzzleObject(PuzzleObject puzObj)
    {
        AllPuzzleObjects.Add(puzObj);
    }

    public PuzzleStep GetPuzzleStepByNumber(int number)
    {
        for (int i = 0; i < AllPuzzleSteps.Count; i++)
        {
            if (AllPuzzleSteps[i].stepNo == number)
            {
                return AllPuzzleSteps[i];
            }
        }

        return null;
    }

    public int totalHintCount()
    {
        int total = 0;
        for (int i = 0; i < AllPuzzleSteps.Count; i++)
        {
            total += AllPuzzleSteps[i].hints.Length;
        }

        return total;

    }

    public void CorrectSound()
    {
        noisePlayer.PlayOneShot(correct);
    }

    public void LoadPuzzle()
    {
        if (this.thisPuzzle != null)
        {
            thisPuzzle.runningTime = 0;
            thisPuzzle.completionTime = -1;
            thisPuzzle.completed = false;
            thisPuzzle.currentPuzzleStep = null;
            thisPuzzle.puzzleStepNum = -1;
            thisPuzzle.puzzleSteps = LoadPuzzleSteps(thisPuzzle).ToArray();
            thisPuzzle.hintsTaken = new bool[totalHintCount()];
            thisPuzzle.gameState = gameState;
            thisPuzzle.puzzleBaseReference = this;
            thisPuzzle.loaded = true;
        }
        else
        {
            thisPuzzle = new Puzzle(roomName);
            thisPuzzle.runningTime = 0;
            thisPuzzle.completionTime = -1;
            thisPuzzle.completed = false;
            thisPuzzle.currentPuzzleStep = null;
            thisPuzzle.puzzleStepNum = -1;
            thisPuzzle.puzzleSteps = LoadPuzzleSteps(thisPuzzle).ToArray();
            thisPuzzle.hintsTaken = new bool[totalHintCount()];
            thisPuzzle.gameState = gameState;
            thisPuzzle.puzzleBaseReference = this;
            thisPuzzle.loaded = true;
        }
        AllPuzzleSteps = thisPuzzle.puzzleSteps.ToList();

        thisPuzzle.AnswerLetter = SolutionLetter;

    }

    public void LoadStepsFromChildren()
    {
        List<PuzzleStep> kidSteps = this.gameObject.GetComponentsInChildren<PuzzleStep>().ToList();
    }

    public Hint[] LoadHintsFromChildren(PuzzleStep Step)
    {
        Hint[] hints = Step.gameObject.GetComponentsInChildren<Hint>();
        if (hints == null || hints.Length == 0) hints = new Hint[0];

        for (int i = 0; i < hints.Length; i++)
        {
            if (hints[i].puzStep == null)
            {
                hints[i].puzStep = Step;
            }
        }
        return hints;

    }

    // Start is called before the first frame update
    void Start()
    {
        AllPuzzleObjects = new List<PuzzleObject>();

        if (gameState == null)
        {
            gameState = GameState.Instance;
        }

        if (thisPuzzle == null || thisPuzzle != gameState.GetPuzzle(roomName))
        {
            thisPuzzle = gameState.GetPuzzle(roomName);
        }

        thisPuzzle = gameState.GetPuzzle(roomName);

        // Broad("PuzzleLoaded", this);
        LoadPuzzle();
        GameState.Instance.SetPuzzle(thisPuzzle);
        gameState.EscapeObject = this.gameObject;
        Debug.Log("BASE OBJ LOADED YAY");
        // TODO: SOMETHING ELSE SHOULD DO THIS
        GameState.Instance.StartPuzzle();
    }

    [SerializeField]
    public string answerAttempt;

    string prevText = "";

    public void UpdateText(TMPro.TMP_InputField input)
    {
        return;
        /*
                if(input.text == prevText)
                {
                    return;
                }

                if(GameState.Instance.OfflineMode)
                {
                    textUpdateHelp(input.gameObject.name, input.text);
                }
                else
                {
                    photonView.RPC("textUpdateHelp", RpcTarget.Others, input.gameObject.name, input.text);
                }*/
    }

    [PunRPC]
    public void textUpdateHelp(string inputName, string text)
    {
        TMPro.TMP_InputField input = GameObject.Find(inputName).GetComponent<TMPro.TMP_InputField>();
        input.text = text;
        prevText = text;
    }

    public void TryAnswer(TMPro.TMP_InputField input)
    {
        this.thisPuzzle.currentPuzzleStep.solutionAttempt(input.textComponent.text);
    }

    public void TryChoice(string choice)
    {
        this.thisPuzzle.currentPuzzleStep.solutionAttempt(choice);
    }

    [PunRPC]
    public void puzzleCompletion()
    {
        thisPuzzle.completionHelper();
    }

    [PunRPC]
    public List<PuzzleStep> LoadPuzzleSteps(Puzzle parentPuzzle)
    {
        List<PuzzleStep> steps = gameObject.GetComponentsInChildren<PuzzleStep>().ToList();
        List<PuzzleStep> realSteps = new List<PuzzleStep>();
        for (int i = 0; i < steps.Count; i++)
        {
            if (!steps[i].isHint)
            {
                realSteps.Add(steps[i]);
            }
        }

        steps = realSteps;
        Debug.Log("steps count is " + steps.Count);
        if (parentPuzzle == null || parentPuzzle.puzzleSteps == null)
        {
            for (int i = 0; i < steps.Count; i++)
            {
                if (steps[i].isHint)
                {
                    continue;
                }
                steps[i].parentPuzzle = parentPuzzle;
                steps[i].incorrect = incorrect;
                steps[i].hints = LoadHintsFromChildren(steps[i]);
            }
            steps.OrderBy(i => i.stepNo);
            return steps;

        }

        for (int i = 0; i < steps.Count; i++)
        {
            if (steps[i].isHint)
            {
                continue;
            }
            if (parentPuzzle.puzzleSteps[i] != null)
            {
                steps[i] = parentPuzzle.puzzleSteps[i];
            }
            else
            {
                steps[i].parentPuzzle = parentPuzzle;
                steps[i].incorrect = incorrect;
                steps[i].hints = LoadHintsFromChildren(steps[i]);
            }
        }

        steps.OrderBy(i => i.stepNo);

        return steps;
    }

    public PuzzleObject GetPuzzleObject(PuzzleStep step)
    {
        PuzzleObject obj = (PuzzleObject)AllPuzzleObjects.Where(x => x.step == step);
        return obj;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameState.Instance.ActivePuzzle == null)
        {
            GameState.Instance.SetPuzzle(thisPuzzle);
        }
    }

}