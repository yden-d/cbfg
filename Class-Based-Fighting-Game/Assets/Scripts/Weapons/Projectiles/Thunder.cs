using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder : MonoBehaviour
{
    public float cloudSpeed = 0.5f;
    public bool facingRight;
    public float cloudDuration;
    private float tickTimer;
    private Rigidbody2D rb;
    public GameObject lightningPrefab;
    public float lightningDamage;
    private bool isStormy;
    public float lightningTimer;
    public float lightningDuration;

    void Start()
    {
        tickTimer = 0f;
        lightningTimer = 0f;
        lightningDuration = 1f;
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        isStormy = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 facing = facingRight ? Vector3.right : Vector3.left;
        Vector2 direction = gameObject.transform.rotation * facing;
        rb.AddForce(direction * cloudSpeed);
        tickTimer += Time.deltaTime;

        if(tickTimer >= cloudDuration) 
        {
            Destroy(this.gameObject);
        }
        if(isStormy) 
        {
            lightningTimer += Time.deltaTime;
            if (lightningTimer >= lightningDuration) 
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void setRotation(bool direction)
    {
        facingRight = direction;
        Vector3 currentScale = gameObject.transform.localScale;
        if (!facingRight)
        {
            currentScale.x = -1;
        }
        else
        {
            currentScale.x = 1;
        }
        gameObject.transform.localScale = currentScale;
    }

    public void stopCloud() 
    {
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        createLightning();
        isStormy = true;
    }

    private void createLightning() 
    {
        Instantiate(lightningPrefab, this.transform);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (!collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.name == "dummy")
            {
                DummyController dummy = collision.gameObject.GetComponent<DummyController>();
                if (dummy != null)
                {
                    dummy.takeDamage(lightningDamage);
                    Debug.Log(lightningDamage);
                }
            }
        }
        else
        {
            Physics2D.IgnoreCollision(this.gameObject.GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>());
        }
    }
}
