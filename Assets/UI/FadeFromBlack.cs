using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeFromBlack : MonoBehaviour
{
    public Image image;
    double fade;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        StartCoroutine(FadeIn());
        // if(color.a >= 0)
        // {
        //     fade = color.a - (0.1 * Time.deltaTime);

        //     Debug.Log("opacity:" + color.a);
        //     image = new Color(image.r,image.g,image.b,(float)fade);
        // }
    }
    
    public IEnumerator FadeIn()
    {
        Color color = image.color;
        if(color.a > 0)
        {
            fade = color.a - (0.5 * Time.deltaTime);

            // Debug.Log("opacity:" + color.a);
            image.color = new Color(color.r,color.g,color.b,(float)fade);
            yield return null;
        }
    }
}
