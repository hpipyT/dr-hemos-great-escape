using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalLightHelper : MonoBehaviour
{

    public Material regularLight;
    public Material gatsbyLight;
    public MeshRenderer meshRend;

    public string room;

    bool litUp;

    // Start is called before the first frame update
    void Start()
    {
        if (meshRend == null)
        {
            meshRend = this.gameObject.GetComponent<MeshRenderer>();
        }

        GameState.Instance.roomComplete.AddListener(ChangeLight);
        litUp = CheckForPuzzleCompletion(room);
        ChangeLight();

    }

    bool CheckForPuzzleCompletion(string puzzleName)
    {
        for (int i = 0; i < GameState.Instance.AllPuzzles.Count; i++)
        {
            if (GameState.Instance.AllPuzzles[i] == null)
            {
                continue;
            }

            if (GameState.Instance.AllPuzzles[i].room == puzzleName)
            {
                if (GameState.Instance.AllPuzzles[i].completed)
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ChangeLight()
    {
        if (!litUp)
        {
            litUp = CheckForPuzzleCompletion(room);
        }

        if (litUp)
        {
            meshRend.material = gatsbyLight;
        }
    }
}