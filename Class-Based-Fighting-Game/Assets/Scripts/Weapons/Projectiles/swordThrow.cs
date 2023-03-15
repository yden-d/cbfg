using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swordThrow : MonoBehaviour
{
    public float throwSpeed;
    private Rigidbody2D rb;
    public float swordThrowDamage;
    private GameObject sword;
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        sword = GameObject.Find("sword");
        sword.gameObject.GetComponent<Sword>().isActive = false;
        sword.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = gameObject.transform.up;

        throwSwordUp(direction);
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
                    dummy.takeDamage(swordThrowDamage);
                }
            }
            retSword();
            Destroy(this.gameObject);
        }
        else
        {
            Physics2D.IgnoreCollision(this.gameObject.GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>());
        }
    }

    private void throwSwordUp(Vector2 direction)
    {
        rb.AddForce(direction * throwSpeed * 10 * -1);
    }

    private void retSword() 
    {
        sword.gameObject.GetComponent<Sword>().isActive = true;
        sword.SetActive(true);
    }
}
