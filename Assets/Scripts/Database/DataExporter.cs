using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleAnswer
{

    public string answer;
    public string SID;
    public string PUZid;

    public PuzzleAnswer(string answer, string puzID, string stepID)
    {
        this.answer = answer;
        this.SID = stepID;
        this.PUZid = puzID.ToString();
    }

    public PuzzleAnswer(string answer)
    {
        this.answer = answer;
        /*        this.SID = student.SID;
            this.PUZid = student.PUZid;*/

    }

    public List<PuzzleAnswer> AllStepAnswers(PuzzleStep step)
    {
        List<PuzzleAnswer> allAnswers = new List<PuzzleAnswer>();
        for (int i = 0; i < step.incorrectAnswers.Count; i++)
        {
            allAnswers.Add(new PuzzleAnswer(step.incorrectAnswers[i], step.stepNo.ToString(), step.parentPuzzle.idNo.ToString()));
        }

        return allAnswers;
    }

}

public class PuzzleStepData
{
    public string hintsTaken;
    public float runningTime;
    public bool isComplete;
    public int stepNo;
    public string AID;
    public string PUZid;
    public string roomName;
    public StudentInfo student = GameObject.Find("PlayerData").GetComponent<StudentInfo>();

    public PuzzleStepData(PuzzleStep step)
    {
        this.AID = student.GAMEid;
        this.PUZid = student.PUZid;
        roomName = step.parentPuzzle.room;

        /*        // if step is last step, value is cycled to -100. set it to last step's actual val
                    if(step.stepNo < 0)
                    {
                        stepNo = step.parentPuzzle.puzzleSteps.Length;
                    }
                    else
                    {
                        stepNo = step.stepNo;
                    }*/
        stepNo = step.stepNo;

        hintsTaken = GetHintTakenCount(step.hints).ToString();
        runningTime = step.runningTime;

        isComplete = step.stepCompleted;
        //puzzleID = step.parentPuzzle.idNo;

        // create puzzlestep, return puzzlestep ID assign to PlayerData
    }

    int GetHintTakenCount(Hint[] hints)
    {
        int hintsTaken = 0;
        for (int i = 0; i < hints.Length; i++)
        {
            if (hints[i].hintTaken) hintsTaken++;
        }

        return hintsTaken;
    }

}

public class PuzzleAttemptData
{
    string roomName;
    int puzzleID;
    bool isComplete;
    float completeTime;

    public PuzzleAttemptData(string roomName, int puzzleID, bool isComplete, float runningTime)
    {
        this.roomName = roomName;
        this.puzzleID = puzzleID;
        this.isComplete = isComplete;
        this.completeTime = runningTime;
    }

    public PuzzleAttemptData(Puzzle puzzle)
    {
        this.roomName = puzzle.room;
        this.puzzleID = puzzle.idNo;
        this.isComplete = puzzle.completed;
        this.completeTime = puzzle.completionTime;
    }
}

public class EscapeData
{
    float totalRunTime;
    int roomsCompleted;
    int totalHintsUsed;

    public EscapeData(GameState gameState)
    {
        totalRunTime = gameState.totalRunTime;
    }
}

public class FormattedData
{

    float TotalTime;

    float TimeSpent;
    bool isCompleted;
    int puzzleID;

    List<int> hintsTaken;

    int stepID;
    List<string> answers;
    public FormattedData(Puzzle puzzleData)
    {
        TimeSpent = puzzleData.completionTime;
        isCompleted = puzzleData.completed;
        puzzleID = puzzleData.idNo;
        answers = new List<string>();
    }

    public void GetDataFromSteps(Puzzle puzzleData)
    {
        for (int i = 0; i < puzzleData.puzzleSteps.Length; i++)
        {
            if (puzzleData.puzzleSteps[i].stepNo < 0)
            {
                continue;
            }
            answers.AddRange(GetAnswersGiven(puzzleData.puzzleSteps[i]));
        }
    }

    public List<string> GetAnswersGiven(PuzzleStep step)
    {
        List<string> answersGiven = new List<string>();
        for (int i = 0; i < step.incorrectAnswers.Count; i++)
        {
            answersGiven.Add(step.incorrectAnswers[i]);
        }

        return null;
    }

    public List<string> GetHintsTaken(Puzzle puzzleData)
    {
        List<string> hintsTaken = new List<string>();
        for (int i = 0; i < puzzleData.puzzleSteps.Length; i++)
        {

        }
        return null;
    }

    public FormattedData(string roomName)
    {

    }
}
// Constructor
public class DataExporter : MonoBehaviour
{

    public static DataExporter Instance;
    // db functions
    public GameObject db;
    public StudentInfo student;

    public GameState gameState;
    public UnityEvent importantGameUpdate;
    // Start is called before the first frame update

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    void Start()
    {
        db = GameObject.Find("DBAccess");
        student = GameObject.Find("PlayerData").GetComponent<StudentInfo>();

        gameState = GameState.Instance;
        importantGameUpdate.AddListener(gameState.CompletePuzzle);
        importantGameUpdate.AddListener(gameState.ActivateHint);
    }

    void UpdateData()
    {

    }

    public List<PuzzleStepData> GetAllPuzzleStepData()
    {
        List<PuzzleStepData> allData = new List<PuzzleStepData>();
        for (int i = 0; i < gameState.AllPuzzles.Count; i++)
        {
            for (int j = 0; j < gameState.AllPuzzles[i].puzzleSteps.Length; j++)
            {
                allData.Add(new PuzzleStepData(gameState.AllPuzzles[i].puzzleSteps[i]));
            }
        }
        return allData;
    }

    public List<PuzzleAttemptData> GetAllPuzzleData()
    {
        List<PuzzleAttemptData> allData = new List<PuzzleAttemptData>();
        for (int i = 0; i < gameState.AllPuzzles.Count; i++)
        {
            allData.Add(new PuzzleAttemptData(gameState.AllPuzzles[i]));
        }
        return allData;
    }

    public PuzzleAttemptData GetCurrentPuzzleAttemptData()
    {
        return new PuzzleAttemptData(gameState.ActivePuzzle);
    }

    public PuzzleStepData GetCurrentPuzzleStepData()
    {
        //Debug.Log();
        return new PuzzleStepData(gameState.ActivePuzzle.currentPuzzleStep);
    }

    public PuzzleAnswer GetPuzzleAnswerData(string answer)
    {
        StudentInfo student = GameObject.Find("PlayerData").GetComponent<StudentInfo>();
        Debug.Log("wrong answer: " + answer);

        return new PuzzleAnswer(answer, student.PUZid, student.SID);
    }

    public FormattedData GetDataForExport()
    {
        return null;
    }

    // Update is called once per frame
    void Update()
    {

    }
}