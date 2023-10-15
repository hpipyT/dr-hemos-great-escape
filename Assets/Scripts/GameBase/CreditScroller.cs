using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditScroller : MonoBehaviour
{
    public GameObject scroller;

    Vector3 startPos;
    Vector3 currentPos;

    public float height;

    bool scrolling;

    public float speed;
    public float initY;
    // Start is called before the first frame update
    void Start() { }

    void Awake()
    {
        scrolling = false;
        startPos = scroller.transform.localPosition;
        initY = scroller.transform.localPosition.y;
        currentPos = startPos;
        height = scroller.GetComponent<RectTransform>().rect.height;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartScrolling()
    {
        if (!scrolling)
        {
            scrolling = true;
            StartCoroutine(Scroll());
        }
    }

    IEnumerator Scroll()
    {
        while (scrolling)
        {
            currentPos.y += (speed * Time.fixedDeltaTime);
            float delta = Mathf.Abs(currentPos.y - initY);
            if (delta >= (1.5 * height))
            {
                currentPos.y = initY;
            }

            scroller.transform.localPosition = currentPos;
            yield
            return new WaitForFixedUpdate();
        }
    }
}