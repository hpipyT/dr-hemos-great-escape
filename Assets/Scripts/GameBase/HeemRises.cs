using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeemRises : MonoBehaviour
{
    Vector3 pos;
    public CanvasGroup textBubble;
    public float speed = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        pos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1.0f, gameObject.transform.position.z);
        StartCoroutine(RiseHim());
        textBubble.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator RiseHim()
    {

        float t = 0.0f;
        float time = 1.0f;

        float step = speed * Time.deltaTime;
        Vector3 newPos = gameObject.transform.position;
        while (newPos.y < pos.y)
        {
            // interpolate where the droid will be at time (t/time), given start, and end (start + step)
            newPos = Vector3.Lerp(newPos, pos, speed * Time.fixedDeltaTime);
            if (Mathf.Abs(pos.y - newPos.y) < 0.08f)
            {
                newPos = pos;
            }
            this.gameObject.transform.position = newPos;

            t += Time.fixedDeltaTime;

            yield
            return new WaitForFixedUpdate();
        }

        // dumb
        while (textBubble.alpha < 1.0f)
        {
            yield
            return new WaitForSecondsRealtime(0.0025f);
            textBubble.alpha += 0.01f;
        }

    }

}