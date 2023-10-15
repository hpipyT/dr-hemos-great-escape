using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static System.Net.Mime.MediaTypeNames;
using TMPro;

public class Hint : MonoBehaviour
{
    public enum HintType
    {
        Object,
        String,
        PuzzleStep
    }

    public bool hintTaken = false;

    public int hintNo;

    public int TimePenalty;

    public HintType hintType;

    public List<PuzzleObject> puzObjs = null;

    public List<PuzzleObject> completionObjs = null;
    public string hintString = null;
    public PuzzleStep puzStep = null;

    public GameObject hintLocation;

    public int fontSize = 12;
    public float fontRotation = 0;
    public float textHeight = 0;

    public float canvasWidth = 400;
    public float canvasHeight = 200;

    GameObject textDisplay;
    GameObject myText;
    Canvas myCanvas;
    UnityEngine.UI.Image panel;
    TextMeshPro text;
    RectTransform rectTransform;

    bool hintSpawned = false;

    public bool hintActive = false;

    public bool unityCallHintExperimental = false;

    public void Start()
    {

    }

    public void CompleteHint()
    {
        if (completionObjs != null && completionObjs != Enumerable.Empty<PuzzleObject>())
        {
            for (int i = 0; i < completionObjs.Count; i++)
            {
                completionObjs[i].ActivateObject();
            }
        }
    }

    public void DeactivateHint()
    {
        switch (hintType)
        {
            case Hint.HintType.Object:
                // DeactivateObjects();
                break;
            case Hint.HintType.String:
                // TODO: Basically non-applicable, object is 99% of the time more useful.
                textDisplay.SetActive(false);
                break;
            case Hint.HintType.PuzzleStep:
                puzStep.deactivate();
                break;
        }
        DeactivateCompletionObjs();
        DeactivateObjects();

        hintActive = false;
    }

    public void DeactivateObjects()
    {
        if (puzObjs != null && puzObjs.Count > 0)
        {
            for (int i = 0; i < puzObjs.Count; i++)
            {
                if (puzObjs[i] == null)
                {
                    continue;
                }
                if (puzObjs[i].gameObject.activeSelf == false)
                {
                    continue;
                }
                if (puzObjs[i].activated) puzObjs[i].DeactivateObject();
            }
        }
    }

    public void DeactivateCompletionObjs()
    {
        if (completionObjs != null && completionObjs != Enumerable.Empty<PuzzleObject>())
        {
            for (int i = 0; i < completionObjs.Count; i++)
            {
                if (completionObjs[i].gameObject.activeSelf == false) continue;

                completionObjs[i].DeactivateObject();
            }
        }

    }

    public void Update()
    {
        if (hintType == Hint.HintType.PuzzleStep)
        {
            if (puzStep != null && puzStep.stepCompleted == true)
            {
                CompleteHint();
            }
        }

        if (hintSpawned)
        {
            text.rectTransform.sizeDelta = new Vector2(canvasWidth, canvasHeight);

            textDisplay.transform.rotation = Quaternion.Euler(0, fontRotation, 0);
            Vector3 position = hintLocation.transform.position;
            textDisplay.transform.localPosition = new Vector3(position.x, position.y + textHeight, position.z);
        }
    }

    public void DisplayHint()
    {
        if (puzStep == null) puzStep = transform.parent.GetComponent<PuzzleStep>();

        // Relevant vars: FontSize, FontRotation, TextSize, HintLocation

        SpawnCanvas(hintLocation.transform.position);

    }

    private void SpawnCanvas(Vector3 position)
    {
        // Canvas
        textDisplay = GameObject.Instantiate(GameState.Instance.FloatyHintPrefab);
        textDisplay.name = "FloatyHint - Step " + puzStep.stepNo + "." + hintNo;

        /*        // Back panel
            panel = textDisplay.AddComponent<UnityEngine.UI.Image>();
            panel.color = new Vector4(Color.black.r, Color.black.g, Color.black.b, 0.5f);
            panel.rectTransform.sizeDelta = new Vector2(canvasWidth, canvasHeight);*/

        // Text
        myText = textDisplay.transform.GetComponentInChildren<TextMeshPro>().gameObject;
        myText.name = "Text";

        text = myText.GetComponent<TextMeshPro>();
        text.text = hintString;
        text.rectTransform.sizeDelta = new Vector2(canvasWidth, canvasHeight);
        // Text alignment
        // rectTransform = text.GetComponent<RectTransform>();
        //  rectTransform.localPosition = new Vector3(0, 0, 0);
        //   rectTransform.sizeDelta = new Vector2(400, 200);

        // Canvas location and transform
        //textDisplay.transform.localScale = textDisplay.transform.localScale * 0.01f;
        textDisplay.transform.rotation = Quaternion.Euler(0, fontRotation, 0);
        textDisplay.transform.localPosition = new Vector3(position.x, position.y + textHeight, position.z);

        hintSpawned = true;

    }

    public bool IsHintComplete()
    {
        return hintActive;
    }

    public void ActivateHint()
    {
        hintActive = true;
        GameState.Instance.ActivateHint();

        switch (hintType)
        {
            case HintType.Object:
                if (puzObjs != null)
                {
                    for (int i = 0; i < puzObjs.Count; i++)
                    {
                        if (puzObjs[i] == null)
                        {
                            continue;
                        }
                        if (puzObjs[i].gameObject.activeSelf == false)
                        {
                            puzObjs[i].gameObject.SetActive(true);
                        }
                        puzObjs[i].ActivateObject();
                    }

                    hintTaken = true;
                }
                break;
            case HintType.PuzzleStep:
                if (puzStep != null)
                {
                    Debug.Log("activating hint puzzlestep");
                    puzStep.activate();
                    hintTaken = true;
                }
                break;
            case HintType.String:
                DisplayHint();
                hintTaken = true;
                break;
            default:
                Debug.Log("No hint type set for hint " + gameObject.name + "!");
                break;
        }

        GameState.Instance.HintTimePenalty(TimePenalty);
    }
}