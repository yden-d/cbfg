using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Confusion : MonoBehaviour
{
    public float confusionSpeed = 1f;
    public bool facingRight;
    private Rigidbody2D rb;
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 facing = facingRight ? Vector3.right : Vector3.left;
        Vector2 direction = gameObject.transform.rotation * facing;
        rb.AddForce(direction * confusionSpeed);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.name == "dummy")
            {
                DummyController dummy = collision.gameObject.GetComponent<DummyController>();
                if (dummy != null)
                {
                    //Invert controls
                }
            }
            Destroy(this.gameObject);
        }
        else
        {
            //TODO: send confusion packet
            
            Physics2D.IgnoreCollision(this.gameObject.GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>());
        }
    }
}
