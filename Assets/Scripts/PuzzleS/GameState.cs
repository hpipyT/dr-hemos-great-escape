using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.Events;

public class GameState : MonoBehaviourPun
{

    public static GameState Instance
    {
        get;
        private set;
    }

    public UnityEvent roomComplete;
    public const int MaxRuntime = 3600;
    // This is one hour, in seconds. Change to change the max time.

    [SerializeField]
    public float totalRunTime = -1;
    [SerializeField]
    public Puzzle ActivePuzzle;

    public bool isRunning;
    [SerializeField]
    public static int puzzleCount = 2;

    public List<Puzzle> AllPuzzles = new List<Puzzle> {
    new Puzzle("Cafeteria"),
    new Puzzle("Library")
  };

    String teamName;
    bool[] finishedPuzzles;

    public GameObject EscapeObject;
    public GameObject LettersParent;

    List<TextMeshProUGUI> allLetters;
    List<string> activeLetters;

    public GameObject HintPrefab;
    public GameObject HintParent;

    List<GameObject> AllHintIcons;

    public TextMeshProUGUI TimeDisplay;
    public TextMeshProUGUI RoomsDisplay;
    public TextMeshProUGUI HintsTakenText;

    public bool dbActive = true;

    public void SetRunning(bool state)
    {
        isRunning = state;
    }

    public Puzzle GetPuzzle(string room)
    {
        for (int i = 0; i < AllPuzzles.Count; i++)
        {
            if (AllPuzzles[i] == null) continue;
            if (room == AllPuzzles[i].room)
            {
                AllPuzzles[i].SetPuzzleID(i);
                return AllPuzzles[i];
            }

        }
        Debug.LogWarning("No Puzzle with name " + room + " found.");
        return null;

    }

    [PunRPC]
    public void AwakenLetters(List<string> letters)
    {
        for (int i = 0; i < letters.Count; i++)
        {
            if (allLetters == null || allLetters.Count == 0)
            {
                allLetters = LettersParent.GetComponentsInChildren<TextMeshProUGUI>().ToList();
            }

            Debug.Log("attempting activation of letter " + letters[i] + ", from a list of size" + allLetters.Count);

            for (int s = 0; s < allLetters.Count; s++)
            {
                if (allLetters[s].gameObject.name.ToLower().Contains(letters[i].ToLower()))
                {
                    allLetters[s].enabled = true;
                }
            }
        }

    }

    [PunRPC]
    public void RPCCompletePuzzle()
    {
        Debug.Log("Completed puzzle: " + ActivePuzzle.room + " with a time of " + ActivePuzzle.completionTime);
        this.ActivePuzzle.completed = true;

        roomComplete.Invoke();
        finishedPuzzles[ActivePuzzle.GetPuzzleID()] = true;
        for (int i = 0; i < ActivePuzzle.AnswerLetter.Length; i++)
        {
            activeLetters.Add(ActivePuzzle.AnswerLetter[i].ToString());
        }
        //activeLetters.Add(ActivePuzzle.AnswerLetter);
        AwakenLetters(activeLetters);

        ActivePuzzle = null;
        bool Incomplete = false;
        for (int i = 0; i < finishedPuzzles.Length; i++)
        {
            if (finishedPuzzles[i] == false) Incomplete = true;
        }

        if (!Incomplete)
        {
            isRunning = false;
            Debug.Log("TIME STOP! ESCAPE ROOM COMPLETED AT " + totalRunTime + " FRAMES");

            EscapeGameManager.Instance.gameCompleted = true;

            // send one submission as host
            if (PhotonNetwork.IsMasterClient && dbActive)
            {
                try
                {
                    GameObject db = GameObject.Find("DBAccess");
                    StudentInfo student = GameObject.Find("PlayerData").GetComponent<StudentInfo>();

                    db.GetComponent<SendAttempt>().OnSubmit(student.GAMEid, totalRunTime.ToString());
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    Debug.Log("Err or submitting attempt");
                }
            }

        }

        UpdateRoomVisuals();

        //The door becomes active again - and the players can now escape.
        EscapeObject.SetActive(true);
        //TODO: Implement post-puzzle step implementation.
    }

    public void CompletePuzzle()
    {
        if (OfflineMode)
        {
            RPCCompletePuzzle();
        }
        else
        {
            photonView.RPC("RPCCompletePuzzle", RpcTarget.All);
        }
    }

    public void SetPuzzle(Puzzle roomPuzzle)
    {
        ActivePuzzle = roomPuzzle;
        if (ActivePuzzle.completed)
        {
            ActivePuzzle = null;
            return;
        }
        for (int i = 0; i < AllPuzzles.Count; i++)
        {
            if (AllPuzzles[i] == null)
            {
                Debug.Log("Null puzzle chilling at " + i);
                continue;
            }

            if (AllPuzzles[i].room == ActivePuzzle.room) return;
        }

        AllPuzzles.Add(ActivePuzzle);
    }

    IEnumerator LoadPuzzle()
    {
        if (ActivePuzzle == null || ActivePuzzle.loaded == false)
        {

            yield
            return new WaitForFixedUpdate();
        }

        StartPuzzle();
    }

    //The door object will manually call this when it is finished loading.
    // TODO: THIS IS CURRENTLY DONE IN PUZZLE BASE OBJECT.
    public void StartPuzzle()
    {

        Debug.Log("sssstarting puzzzle?");
        if (ActivePuzzle == null || ActivePuzzle.loaded == false)
        {
            Debug.Log("puzzle not real");

            StartCoroutine(LoadPuzzle());
            return;
        }
        Debug.Log("wheres it stuck. here...");
        ActivePuzzle.nextStep();
        Debug.Log("or here cause " + ActivePuzzle.room);
        //BeginEscape();
        UpdateHintVisuals();
        Debug.Log("Puzzle is " + ActivePuzzle.room + ", and the step is now: " + ActivePuzzle.currentPuzzleStep.stepNo);
    }

    //TODO: May need slight refactor based on monitor board usage.
    public List<bool> AllHintsTaken()
    {
        List<bool> boolHints = new List<bool>();
        for (int i = 0; i < AllPuzzles.Count; i++)
        {
            for (int j = 0; j < AllPuzzles[i].puzzleSteps.Length; j++)
            {
                for (int k = 0; k < AllPuzzles[i].puzzleSteps[j].hints.Length; k++)
                {
                    if (AllPuzzles[i].puzzleSteps[j].hints[k].hintTaken)
                    {
                        boolHints.Add(true);
                    }
                }
            }
        }
        return boolHints;
    }

    public bool OfflineMode = true;
    public bool SkipCurrentStep = false;

    public void SetGameStateData(GameState references)
    {
        Debug.Log("Giving the gamestate instance the relevant stuff.");
        this.TimeDisplay = references.TimeDisplay;
        this.RoomsDisplay = references.RoomsDisplay;
        this.HintPrefab = references.HintPrefab;
        this.HintParent = references.HintParent;
        if (AllHintIcons != null && AllHintIcons.Count > 0)
        {
            AllHintIcons.Clear();
        }
        this.LettersParent = references.LettersParent;
        allLetters = LettersParent.GetComponentsInChildren<TextMeshProUGUI>().ToList();
        Debug.Log("There are " + allLetters.Count + " letters on the scoreboard");
        this.FloatyHintPrefab = references.FloatyHintPrefab;
        UpdateHintVisuals();
        UpdateRoomVisuals();
        UpdateTimeVisuals();
        AwakenLetters(activeLetters);
        Debug.Log("Total running time is: " + totalRunTime);
        Destroy(references.gameObject);
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            if (this.HintParent != null)
            {
                GameObject[] kids = this.HintParent.GetComponentsInChildren<GameObject>();
                Debug.Log(kids.Length + " hints on the gamestate screen to transfer");
                for (int i = 0; i < kids.Length; i++)
                {
                    kids[i].transform.SetParent(GameState.Instance.HintParent.transform);
                    kids[i].transform.localPosition = new Vector3(180, -25.8f, 980.5717f);
                }
                //this.HintParent.transform.SetParent(GameState.Instance.TimeDisplay.transform.parent);
            }
            Destroy(this);
            //GameState.Instance.SetGameStateData(this);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            GameObject.Find("EscapeGameManager").GetComponent<EscapeGameManager>().destroyOnLeave.Add(gameObject);
            DontDestroyOnLoad(this);
            Instance = this;
            if (totalRunTime < 0)
            {
                totalRunTime = 0;
            }
            activeLetters = new List<string>();
        }
        //this.ActivePuzzle = null;
        SceneManager.sceneLoaded += SceneLoaded;
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ActivePuzzle = null;
        if (AllHintIcons != null && AllHintIcons.Count > 0)
        {
            for (int i = 0; i < AllHintIcons.Count; i++)
            {
                Destroy(AllHintIcons[i]);
            }
            AllHintIcons.Clear();
        }
        if (SceneManager.GetActiveScene().name == "CafeteriaPuzzles" || SceneManager.GetActiveScene().name == "LibraryPuzzles")
        {
            HintsTakenText.text = "Hints Taken:";
            BeginEscape();
        }
        else
        {
            isRunning = false;
            HintsTakenText.text = "";
        }

        //StartPuzzle();
    }

    public Dictionary<string,
    string> HintsTakenToString()
    {
        Dictionary<string,
        string> verboseHints = new Dictionary<string,
        string>();
        for (int i = 0; i < AllPuzzles.Count; i++)
        {
            for (int j = 0; j < AllPuzzles[i].puzzleSteps.Length; j++)
            {
                for (int k = 0; k < AllPuzzles[i].puzzleSteps[j].hints.Length; k++)
                {
                    if (AllPuzzles[i].puzzleSteps[j].hints[k].hintTaken)
                    {
                        verboseHints.Add(AllPuzzles[i].puzzleSteps[j].question, AllPuzzles[i].puzzleSteps[j].hints[k].hintString);
                    }
                }
            }
        }

        return verboseHints;
    }

    //TODO: Determine how this is going to be dynamic. Prefab instances? Direct reference?
    void LoadPuzzles()
    {
        if (AllPuzzles == null) AllPuzzles = new List<Puzzle>(puzzleCount);
        for (int i = 0; i < puzzleCount; i++)
        {
            if (AllPuzzles[i] == null) AllPuzzles[i] = new Puzzle("DEFAULT");
            AllPuzzles[i].SetPuzzleID(i);
        }

    }

    bool initLoaded = false;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            GameState.Instance.SetGameStateData(this);
        }
        roomComplete = new UnityEvent();
        if (!initLoaded)
        {
            finishedPuzzles = new bool[puzzleCount];
            LoadPuzzles();
            for (int i = 0; i < puzzleCount; i++)
            {
                finishedPuzzles[i] = false;
            }
            initLoaded = true;
        }

        allLetters = LettersParent.GetComponentsInChildren<TextMeshProUGUI>().ToList();
        Debug.Log("There are " + allLetters.Count + " letters on the scoreboard");
        UpdateRoomVisuals();

    }

    void BeginEscape()
    {
        if (AllHintIcons != null && AllHintIcons.Count > 0)
        {
            for (int i = 0; i < AllHintIcons.Count; i++)
            {
                Destroy(AllHintIcons[i]);
            }
            AllHintIcons.Clear();
        }
        Debug.Log("Beginning escape");
        isRunning = true;
        UpdateRoomVisuals();
        UpdateTimeVisuals();
        AwakenLetters(activeLetters);
    }

    [PunRPC]
    public void UpdateHintVisuals()
    {
        Debug.Log("updating hint visuals");
        int hintcount = 0;
        if (ActivePuzzle != null && ActivePuzzle.loaded == true)
        {
            List<Hint> allHints = new List<Hint>();
            for (int i = 0; i < ActivePuzzle.puzzleSteps.Length; i++)
            {
                hintcount += ActivePuzzle.puzzleSteps[i].hints.Length;
                allHints.AddRange(ActivePuzzle.puzzleSteps[i].hints);
            }

            if (AllHintIcons == null || AllHintIcons.Count == 0)
            {
                AllHintIcons = new List<GameObject>();

                Vector2 basePos = new Vector2(250, -310);

                for (int i = 0; i < hintcount; i++)
                {
                    GameObject newHintIcon = GameObject.Instantiate(HintPrefab, basePos, Quaternion.identity, HintParent.transform);
                    AllHintIcons.Add(newHintIcon);
                    newHintIcon.gameObject.transform.localPosition = basePos;
                    basePos.x += 75;
                    if (basePos.x > 845)
                    {
                        basePos.x = 250;
                        basePos.y = -350;
                    }
                }
            }

            Debug.Log("all hints count: " + allHints.Count);
            HintsTakenText.text = "Hints Taken:";

            for (int i = 0; i < allHints.Count; i++)
            {
                if (allHints[i] == null)
                {
                    continue;
                }
                if (allHints[i].hintTaken == true)
                {
                    AllHintIcons[i].GetComponent<Image>().color = Color.red;
                    continue;
                }

                if (allHints[i].puzStep.stepNo != ActivePuzzle.currentPuzzleStep.stepNo)
                {
                    AllHintIcons[i].GetComponent<Image>().color = Color.gray;
                }
                else
                {
                    AllHintIcons[i].GetComponent<Image>().color = Color.white;
                }
            }
        }
        else
        {
            if (AllHintIcons != null && AllHintIcons.Count > 0)
            {
                for (int i = 0; i < AllHintIcons.Count; i++)
                {
                    Destroy(AllHintIcons[i]);
                }
                AllHintIcons.Clear();
            }
            HintsTakenText.text = "";
        }

    }

    public GameObject FloatyHintPrefab;

    public void ActivateHint()
    {
        UpdateHintVisuals();
    }

    [PunRPC]
    void UpdateRoomVisuals()
    {
        int completedCount = 0;
        for (int i = 0; i < AllPuzzles.Count; i++)
        {
            if (AllPuzzles[i] == null) continue;
            if (finishedPuzzles[i] == true || AllPuzzles[i].completed)
            {
                completedCount++;
            }
        }

        string displayText = completedCount + "/" + puzzleCount;
        RoomsDisplay.SetText(displayText);
    }

    void UpdateTimeVisuals()
    {
        TimeSpan testSpan = TimeSpan.FromSeconds(MaxRuntime - totalRunTime);
        int minutes = testSpan.Minutes;
        int seconds = testSpan.Seconds;

        string displayString = testSpan.ToString("mm':'ss");

        TimeDisplay.SetText(displayString);
    }

    public void HintTimePenalty(int penalty)
    {
        TimePenaltyHelper(penalty);
        /*
            if(OfflineMode)
            {
                TimePenaltyHelper(penalty);
            }
            else
            {
                photonView.RPC("TimePenaltyHelper", RpcTarget.All, penalty);
            }*/
    }

    //[PunRPC]
    public void TimePenaltyHelper(int penalty)
    {
        totalRunTime += penalty;
        Debug.Log("TIME PENALTY OF " + penalty + ", NEW TIME IS " + totalRunTime);
        UpdateTimeVisuals();
        UpdateHintVisuals();
        if (ActivePuzzle != null)
        {
            ActivePuzzle.runningTime += penalty;
            ActivePuzzle.currentPuzzleStep.runningTime += penalty;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SkipCurrentStep)
        {
            SkipCurrentStep = false;
            ActivePuzzle.currentPuzzleStep.unitySkipStepExperimental = true;
        }

        if (isRunning)
        {
            if (ActivePuzzle.completed)
            {
                isRunning = false;
            }
            totalRunTime += Time.deltaTime;
            UpdateTimeVisuals();
            if (ActivePuzzle != null && ActivePuzzle.loaded == true)
            {
                ActivePuzzle.runningTime += Time.deltaTime;
                ActivePuzzle.currentPuzzleStep.runningTime += Time.deltaTime;
            }
        }
    }
}