using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class PuzzleObject : MonoBehaviour
{

    int stepNumber;
    public PuzzleStep step;

    public bool isTrigger = false;
    public bool isMoveable = false;
    public bool freezeOnInactive = false;

    public bool disappearOnNextStep = false;
    public int persistUntilStep = -1;
    private Vector3 lastPos;

    public bool activated;
    string objectName;

    public GameObject puzzleGameObject;
    public GameObject targetGameObject;

    UnityEvent objectSolved = new UnityEvent();

    public Collider objectCollider;
    public Rigidbody objectRigidBody;

    public Animator anim;
    public string parameterName;

    // notes: each object that progresses the scene is a puzzleobject
    public PuzzleObject(int stepNo, string objName)
    {
        this.stepNumber = stepNo;
        this.objectName = objName;
        this.activated = false;
    }

    public void SetGameObject(GameObject gObject)
    {
        this.puzzleGameObject = gObject;
    }

    public void SetTargetGameObject(GameObject gObject)
    {
        this.targetGameObject = gObject;
    }

    private void OnTriggerEnter(Collider collision)
    {
        // if chest is intended to be opened right now
        if (targetGameObject != null)
        {
            if (collision.gameObject == targetGameObject && this.activated && this.isTrigger)
            {
                if (anim != null)
                {
                    if (GameState.Instance.OfflineMode)
                    {
                        this.step.TriggerAnimation(parameterName, anim);
                    }
                    else
                    {
                        Debug.Log("called");
                        this.step.GetComponent<PhotonView>().RPC("AltTriggerAnimation", RpcTarget.All, parameterName, this.anim.gameObject.name);
                    }

                }
                Debug.Log("Puzzle Object triggered");
                this.step.objectiveComplete();
            }
        }
        // if chest is unlocked and the player is touching it 
        else if (this.activated && this.isTrigger)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("PuzzleObject [<color>Trigger " + puzzleGameObject.name + " </color>] interacted with by the player- objective complete");
                if (this.step != null)
                {
                    this.step.objectiveComplete();
                }
            }
        }
    }

    private bool Moved()
    {
        Vector3 displacement = transform.position - lastPos;

        if (displacement.magnitude > 0.001) return true;
        return false;
    }

    public void ActivateObject()
    {
        Debug.Log("activating puzzle object " + gameObject.name);
        if (this.gameObject.activeSelf == false)
        {
            this.gameObject.SetActive(true);
        }

        if (puzzleGameObject == null)
        {
            SetGameObject(this.gameObject);
        }

        if (puzzleGameObject.activeSelf != true) puzzleGameObject.SetActive(true);

        if (anim != null && targetGameObject == null)
        {
            if (GameState.Instance.OfflineMode)
            {
                this.step.TriggerAnimation(parameterName, anim);
            }
            else
            {
                this.step.GetComponent<PhotonView>().RPC("AltTriggerAnimation", RpcTarget.All, parameterName, this.anim.gameObject.name);
            }

            //anim.enabled = true;
            //anim.speed = 1;
            //anim.StartPlayback();
        }
        if (isTrigger && objectCollider == null || isMoveable && objectRigidBody == null)
        {
            StartCoroutine(DelayedActivation());
        }
        else
        {
            if (isTrigger && objectCollider != null)
            {
                objectCollider.enabled = true;
            }
            if (isMoveable && objectRigidBody != null)
            {
                objectRigidBody.isKinematic = false;
                objectRigidBody.constraints = RigidbodyConstraints.None;
            }
            this.activated = true;

        }

        //if (!isTrigger && !isMoveable)
        //    this.step.objectiveComplete();
    }

    public IEnumerator DelayedActivation()
    {
        while (puzzleGameObject == null)
        {
            SetGameObject(this.gameObject);
        }
        if (isTrigger)
        {
            while (objectCollider == null)
            {
                objectCollider = puzzleGameObject.GetComponent<Collider>();
                yield
                return null;
            }
        }

        if (isMoveable)
        {
            while (objectRigidBody == null)
            {
                objectRigidBody = puzzleGameObject.GetComponent<Rigidbody>();
                yield
                return null;
            }
        }

        ActivateObject();
    }

    public void DeactivateObject()
    {

        if (isMoveable && objectRigidBody != null)
        {
            objectRigidBody.isKinematic = true;
            if (freezeOnInactive)
            {
                if (!this.step.stepCompleted)
                {
                    objectRigidBody.constraints = RigidbodyConstraints.FreezeAll;
                }
            }
        }
        if (isTrigger && objectCollider != null)
        {
            if (freezeOnInactive) objectCollider.enabled = false;
        }
        if (disappearOnNextStep)
        {
            if (puzzleGameObject != null && puzzleGameObject != this.gameObject)
            {
                puzzleGameObject.SetActive(false);
            }
            this.gameObject.SetActive(false);
        }
        this.activated = false;

    }

    //Used for non-collision puzzle objects. Usually triggered manually by a UI button or similar in-scene gameobject calls.
    public void TriggerObject()
    {
        Debug.Log("Object triggered!");
        this.step.objectiveComplete();
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (puzzleGameObject == null) SetGameObject(this.gameObject);

        if (isTrigger)
        {
            objectCollider = puzzleGameObject.GetComponent<Collider>();
        }

        if (isMoveable)
        {
            lastPos = transform.position;
            if (objectRigidBody == null) objectRigidBody = puzzleGameObject.GetComponent<Rigidbody>();

            if (objectRigidBody == null)
            {
                objectRigidBody = puzzleGameObject.GetComponentInChildren<Rigidbody>();
                Debug.Log("Finding the rigid body in children.");
            }
            else
            {
                Debug.Log("Rigidbody found in " + objectRigidBody.gameObject.name);
            }
        }

        if (anim != null)
        {
            //anim.speed = 0;
        }

        DeactivateObject();

    }

    public void Start()
    {

    }

    public void PuzzleLoaded(PuzzleBaseObject puzBaseObj)
    {
        puzBaseObj.AddPuzzleObject(this);
        if (step == null)
        {

        }
        else
        {
            for (int i = 0; i < step.targetObjects.Length; i++)
            {
                if (step.targetObjects[i] == this)
                {
                    return;
                }
            }
            step.AddPuzzleObject(this);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (activated)
        {
            // Any future puzzle object logic should happen here.
        }

        if (persistUntilStep > 0)
        {
            if (step != null && persistUntilStep == GameState.Instance.ActivePuzzle.currentPuzzleStep.stepNo)
            {
                if (puzzleGameObject != null && puzzleGameObject != this.gameObject)
                {
                    puzzleGameObject.SetActive(false);
                }
                this.gameObject.SetActive(false);
            }
        }

    }
}