using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float arrowSpeed = 0.5f;
    public float fastArrowSpeed = 1f;
    public float arrowDamage = 5.0f;
    public float volleyRotateSpeed = 1.0f; 
    public bool facingRight;

    private bool isVolley = false;
    private bool fastShot = false;
    private Rigidbody2D rb;
    
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Get the left/ right orientation and turn it into a vector
        Vector3 facing = facingRight ? Vector3.right : Vector3.left;
        //Get the vector of where the arrow is pointing
        Vector2 direction = gameObject.transform.rotation * facing;
        //Get the speed that will be applied to the arrow
        float speed = fastShot ? fastArrowSpeed : arrowSpeed;

        if (isVolley)
        {
            float rotation = (volleyRotateSpeed * Time.deltaTime);
            if (facingRight)
            {
                rotation *= -1;
            }
            this.gameObject.transform.Rotate(new Vector3(0, 0, rotation));
        }

        rb.AddForce(direction * speed * Time.deltaTime);
    }

    public void setRotation(bool direction)
    {
        facingRight = direction;
        Vector3 currentScale = gameObject.transform.localScale;
        if (!facingRight)
        {
            currentScale.x = -1;
        } else
        {
            currentScale.x = 1;
        }
        gameObject.transform.localScale = currentScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            if(collision.gameObject.name == "dummy") {
                DummyController dummy = collision.gameObject.GetComponent<DummyController>();
                if(dummy != null)
                {
                    dummy.takeDamage(arrowDamage);
                }
            }
            Destroy(this.gameObject);
        } 
        else
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            ClientSend.SendDamage(player.id, arrowDamage);
            Physics2D.IgnoreCollision(this.gameObject.GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>());
        }   
    }

    public void setArrowSpeed(bool isFast)
    {
        this.fastShot = isFast;
    }

    public void setIsVolley(bool isVolley)
    {
        this.isVolley = isVolley;
    }
    
}