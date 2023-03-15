using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    public float ffDuration;
    private float tickTimer;
    PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        tickTimer = 0f;
        player = this.gameObject.GetComponentInParent<PlayerController>();
        player.canMove = false;
    }

    // Update is called once per frame
    void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= ffDuration)
        {
            player.canMove = true;
            Destroy(this.gameObject);
        }
    }
}
