using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;
using Photon.Pun;
using UnityEngine.UI;

public class AnagramKeyboard : MonoBehaviourPun
{

    [SerializeField]
    public string finalResult;

    [SerializeField]
    public string finalResultNoSpace;
    public List<TextMeshProUGUI> letters;
    public List<string> finalResultByRow;
    public PuzzleStep step;
    public TMP_InputField localinput;
    public Image background;
    public PhotonView pv;

    string prevInput = "";
    // Start is called before the first frame update
    void Start()
    {
        letters = gameObject.GetComponentsInChildren<TextMeshProUGUI>().ToList();
        finalResultByRow = finalResult.Split(' ').ToList();
        finalResultNoSpace = (finalResult.Split(' ').Aggregate((i, j) => i + j).ToString());
        pv = gameObject.GetComponent<PhotonView>();
    }

    public void CheckAnswer(TMPro.TMP_InputField input)
    {

        string answer = input.text.ToUpper();
        prevInput = answer;
        if (answer.Length == 0)
        {

            for (int i = 0; i < letters.Count; i++)
            {
                letters[i].text = finalResultNoSpace[i].ToString().ToUpper();
                letters[i].color = Color.white;
            }

            return;

        }
        string tempFinalResult = finalResultNoSpace.ToUpper();
        tempFinalResult = Regex.Replace(tempFinalResult, @"\s+", " ");
        int answerIndex = 0;
        bool keepChecking = true;
        bool rightAnswer = true;
        Debug.Log("checking " + answer + " as the anagram, from input " + input.text);
        for (int i = 0; i < letters.Count; i++)
        {
            if (keepChecking)
            {

                Debug.Log("Comparing " + tempFinalResult[i] + " with " + answer[answerIndex]);
                if (tempFinalResult[i] == answer[answerIndex])
                {
                    letters[i].color = Color.black;
                    letters[i].text = tempFinalResult[i].ToString();
                }
                else
                {
                    letters[i].color = Color.red;
                    rightAnswer = false;
                    letters[i].text = answer[answerIndex].ToString();
                }
                answerIndex++;
                if (answerIndex >= answer.Length)
                {
                    keepChecking = false;
                    if (letters.Count > answer.Length)
                    {
                        rightAnswer = false;
                    }
                }
            }
            else
            {
                letters[i].text = finalResultNoSpace[i].ToString().ToUpper();
                letters[i].color = Color.white;
            }

        }
        input.text = answer;
        if (rightAnswer)
        {
            if (GameState.Instance.OfflineMode)
            {
                AnswerDone();
            }
            else
            {
                pv.RPC("AnswerDone", RpcTarget.All);
            }
        }
    }

    public void AttemptAnswer(TMPro.TMP_InputField input)
    {
        CheckAnswer(input);
    }

    [PunRPC]
    public void AnswerDone()
    {
        localinput.interactable = false;
        localinput.text = finalResultNoSpace;
        for (int i = 0; i < letters.Count; i++)
        {
            letters[i].text = finalResultNoSpace[i].ToString().ToUpper();
            letters[i].color = Color.black;
        }
        background.color = Color.green;
    }

    // Update is called once per frame
    void Update()
    {
        if (localinput == null) return;
        if (prevInput != localinput.text)
        {
            CheckAnswer(localinput);
        }
    }
}