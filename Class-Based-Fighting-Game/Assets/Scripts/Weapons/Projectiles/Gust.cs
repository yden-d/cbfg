using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gust : MonoBehaviour
{
    public float gustDuration;
    private float tickTimer;
    // Start is called before the first frame update
    void Start()
    {
        tickTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= gustDuration) 
        {
            endGust();
        }
    }

    private void endGust() 
    {
        Destroy(this.gameObject);
    }
}
