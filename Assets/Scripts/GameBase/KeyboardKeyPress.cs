using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class KeyboardKeyPress : MonoBehaviour
{
    public BoxCollider _collider;
    public Keyboard parentKeyboardScript;
    string letter;
    public TextMeshProUGUI keycap;
    float baseYHeight;
    GameObject keycapGameobject;
    bool alreadyMoving;

    bool depressing;

    bool pressed;

    bool special;

    public Vector3 finalKeyPos;

    // Start is called before the first frame update
    void Start()
    {
        _collider = this.gameObject.GetComponent<BoxCollider>();
        letter = transform.parent.gameObject.name;
        letter = ValidateLetter(letter);
        keycap = transform.parent.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        if (keycap != null)
        {
            keycap.text = letter;
            keycapGameobject = keycap.gameObject.transform.parent.gameObject;
        }
        else
        {
            keycap = transform.parent.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        }

        alreadyMoving = false;
        pressed = false;
        depressing = false;
        baseYHeight = keycapGameobject.transform.position.y;
        finalKeyPos = keycapGameobject.transform.position;
        finalKeyPos.y -= 0.012f;
    }

    string ValidateLetter(string letter)
    {
        if (letter.Length != 1)
        {
            special = true;
            return letter;
        }
        else
        {
            special = false;
            return letter.ToLower();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public const float distance = .01f;

    void keycapMover()
    {
        _collider.enabled = false;
        StartCoroutine(KeyMove(distance)); // * multiplier))
    }

    void MoveKeycap(bool downwards)
    {
        /*int multiplier = -1;
            if(!downwards)
            {
                multiplier = 1;
            }
            if(!alreadyMoving && !pressed)
            {*/
        //StartCoroutine(KeyMove(distance));// * multiplier));
        //}

        //    if(GameState.Instance.OfflineMode)
        // {
        keycapMover();
        /*}
            else
            {
            photonView.RPC("keycapMover", RpcTarget.All);
            }*/

    }

    public void PlayClickSound()
    {
        // get audiosources from child of parent
        AudioSource[] sounds = parentKeyboardScript.gameObject.GetComponents<AudioSource>();
        // select number between 1 and 3, play this sound on press boom done
        sounds[Random.Range(0, 4)].Play();
    }

    float cooldown = 0;

    void FixedUpdate()
    {

        this.gameObject.transform.position = keycapGameobject.transform.position;
        if (!alreadyMoving && !pressed && !depressing)
        {
            if (keycapGameobject.transform.position.y < baseYHeight)
            {
                //StartCoroutine(KeyUnmove());
                Debug.Log("calling this rn");
            }
        }

    }

    public void keyHelper()
    {
        if (special)
        {
            switch (letter)
            {
                case "Back":
                    parentKeyboardScript.Backspace();
                    break;
                case "Enter":
                    Debug.Log("Enter pressed");
                    parentKeyboardScript.Submit();
                    break;
                case "Space":
                    parentKeyboardScript.AddToInput(" ");
                    break;
                default:
                    Debug.Log("Invalid special key.");
                    break;
            }
        }
        else
        {
            parentKeyboardScript.AddToInput(letter);
        }
    }

    void ActivateKey()
    {
        keyHelper();
    }

    public const float buttonSpeed = .23f;

    bool moveDownAgain;
    IEnumerator KeyMove(float distance)
    {

        float finalYpos = finalKeyPos.y - keycapGameobject.transform.position.y;
        alreadyMoving = true;

        cooldown = 0.5f;

        if (distance > 0)
        {
            //finalYpos = baseYHeight;
        }

        float trueDistance = finalKeyPos.y - keycapGameobject.transform.position.y;
        Debug.Log("true distance to move is " + trueDistance + " FinalYos is " + finalYpos + " because the OG and End are : " + keycapGameobject.transform.position.y + " and " + finalKeyPos.y);
        /// <summary>
        ///  speed = distance / time
        /// </summary>
        float newDuration = Mathf.Abs(trueDistance / buttonSpeed);
        float time = 0;
        Vector3 newPos = keycapGameobject.transform.position;
        while (time < newDuration)
        {
            newPos = keycapGameobject.transform.position;
            newPos.y += -1 * Mathf.Abs(buttonSpeed * Time.fixedDeltaTime);
            time += Time.fixedDeltaTime;
            keycapGameobject.transform.position = newPos;
            yield
            return new WaitForFixedUpdate();
        }

        time = 0;
        while (time < .06f)
        {
            time += Time.fixedDeltaTime;
            yield
            return new WaitForFixedUpdate();
        }

        alreadyMoving = false;

        depressing = true;
        newPos = keycapGameobject.transform.position;
        trueDistance = baseYHeight - newPos.y;

        if (trueDistance < 0)
        {
            newPos.y = baseYHeight;
            keycapGameobject.transform.position = newPos;
            //pressed = false;
            _collider.enabled = true;
            yield
            break;
        }
        else if ((newPos.y + trueDistance) > baseYHeight)
        {
            newPos.y = baseYHeight;
            keycapGameobject.transform.position = newPos;
            //pressed = false;
            _collider.enabled = true;
            yield
            break;
        }
        Debug.Log("true distance to UNMOVE is " + trueDistance + " because the OG and End are : " + keycapGameobject.transform.position.y + " and " + baseYHeight);
        float duration = Mathf.Abs(trueDistance / buttonSpeed);
        time = 0;
        while (time < duration)
        {
            newPos = keycapGameobject.transform.position;
            newPos.y += (buttonSpeed * Time.fixedDeltaTime);
            if (newPos.y > baseYHeight)
            {
                newPos.y = baseYHeight;
                keycapGameobject.transform.position = newPos;
                //pressed = false;
                _collider.enabled = true;
                yield
                break;

            }
            time += Time.fixedDeltaTime;
            keycapGameobject.transform.position = newPos;
            yield
            return new WaitForFixedUpdate();
        }

        depressing = false;
        //pressed = false;
        _collider.enabled = true;
        yield
        break;

    }

    IEnumerator KeyUnmove()
    {
        //while(alreadyMoving)
        //  {
        //    yield return new WaitForFixedUpdate();
        // }
        depressing = true;
        Vector3 newPos = keycapGameobject.transform.position;
        float trueDistance = baseYHeight - newPos.y;

        if (trueDistance < 0)
        {
            yield
            break;
        }
        else if ((newPos.y + trueDistance) > baseYHeight)
        {
            yield
            break;
        }
        Debug.Log("true distance to UNMOVE is " + trueDistance + " because the OG and End are : " + keycapGameobject.transform.position.y + " and " + baseYHeight);
        float duration = Mathf.Abs(trueDistance / buttonSpeed);
        float currentTime = 0;
        while (currentTime < duration)
        {
            newPos = keycapGameobject.transform.position;
            newPos.y += (buttonSpeed * Time.fixedDeltaTime);
            if (newPos.y > baseYHeight)
            {
                yield
                break;
            }
            currentTime += Time.fixedDeltaTime;
            keycapGameobject.transform.position = newPos;
            yield
            return new WaitForFixedUpdate();
        }

        depressing = false;
        pressed = false;
        yield
        break;
    }

    //void OnCollisionEnter(Collision other)
    //{

    //    if(other.gameObject.tag == "Player")// && !depressing)
    //    {
    //        pressed = false;
    //    }
    //    //MoveKeycap(false);
    //  //  if(!depressing)
    //   // {
    //   //     depressing = true;
    //     //   StartCoroutine(KeyUnmove());
    //  //  }
    //   // }
    //}

    //void OnCollisionEnter(Collision other)
    //{
    //    if(pressed)
    //    {
    //        return;
    //    }
    //    if(other.gameObject.tag == "Player" && !alreadyMoving)// && cooldown == 0)
    //    {
    //        if(depressing)
    //        {
    //            //StopCoroutine(KeyUnmove());
    //            depressing = false;
    //        }
    //    MoveKeycap(true);
    //    pressed = true;
    //    ActivateKey();
    //    }

    //}

    void OnTriggerEnter(Collider other)
    {
        if (pressed)
        {
            return;
        }
        if (other.gameObject.CompareTag("Player")) // && cooldown == 0)
        {
            if (depressing)
            {
                //StopCoroutine(KeyUnmove());
                depressing = false;
            }
            Debug.Log("Clicked");
            PlayClickSound();
            MoveKeycap(true);
            pressed = true;
            ActivateKey();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            pressed = false;
        }
    }
}