using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFirework : MonoBehaviour
{
    public List<ParticleSystem> fireworks;
    public float time = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FireworkLoop());
    }

    IEnumerator FireworkLoop()
    {
        while (true)
        {
            foreach(ParticleSystem firework in fireworks)
            {
                firework.transform.position = new Vector3(Random.Range(6.58f, 9.8f), Random.Range(1.16f, 2.9f), firework.transform.position.z);
                firework.Play();
                yield return new WaitForSeconds(Random.Range(0.5f, 1f));
            }
        }
    }
}
