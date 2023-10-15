using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudentInfo : MonoBehaviour
{
    // player's identifier in the database -- planned to be assigned by the CoM and loaded into students table by CSV
    public string SID;
    // identifier for the game attempt, one playthrough has one GAMEid -- assigned when joining PhotonRoom
    public string GAMEid;
    // string identifier for the game attempt, associated with each SID (up to 5) participating in a game attempt
    // assigned at PhotonRoom creation
    public string TeamName;
    // each puzzle room has a set of questions to complete, each question is a puzzle step with hints, time spent, and answers
    // assigned when a puzzle room starts the next question
    public string PUZid;
    
    // player who submit an answer calls the logic for every player in the room, used for checking when to add answers to the DB
    // assigned in PuzzleBaseObject
    public bool justSubmitted = false;


    void Awake()
    {
        Object.DontDestroyOnLoad(gameObject);
        GameObject.Find("EscapeGameManager").GetComponent<EscapeGameManager>().destroyOnLeave.Add(gameObject);
        // i dont think i understand instances entirely
        //EscapeGameManager.Instance.destroyOnLeave.Add(gameObject);
    }

    // Start is called before the first frame update

    public string GetSID()
    {
        return SID;
    }    

    public void SetSID(string SID)
    {
        this.SID = SID;
    }

    public string GetGAMEid()
    {
        return GAMEid;
    }

    public void SetGAMEid(string GAMEid)
    {
        this.GAMEid = GAMEid;
    }




}
