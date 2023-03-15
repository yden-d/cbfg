using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vortex : MonoBehaviour
{
    public float vortexDamage = 0f;
    public float tickInterval;

    private float tickTimer;

    void Start()
    {
        tickTimer = 0f;
    }

    public void vortex() 
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) 
        {
            if (collision.gameObject.name == "dummy") 
            {
                DummyController dummy = collision.gameObject.GetComponent<DummyController>();
                if (dummy != null) 
                {

                    tickTimer += Time.deltaTime;
                    
                    if (tickTimer >= tickInterval)
                    {
                        dummy.takeDamage(vortexDamage);
                        tickTimer = 0f;
                    }

                }
            }
        }
        else
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            ClientSend.SendDamage(player.id, vortexDamage);
            Physics2D.IgnoreCollision(this.gameObject.GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (!collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.name == "dummy")
            {
                DummyController dummy = collision.gameObject.GetComponent<DummyController>();
                if (dummy != null)
                {
                    tickTimer = 0f;
                }
            }
        }
    }
}
