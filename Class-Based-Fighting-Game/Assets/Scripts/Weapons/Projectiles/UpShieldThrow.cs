using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpShieldThrow : MonoBehaviour
{
    public float throwSpeed;
    private Rigidbody2D rb;
    public float shieldThrowDamage;
    Animator anim;
    public bool throwDirection;
    private GameObject shield;
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        anim = this.gameObject.GetComponent<Animator>();
        shield = GameObject.Find("shield");
        shield.gameObject.GetComponent<Shield>().isActive = false;
        shield.SetActive(false);
        throwDirection = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = gameObject.transform.up;

        if (throwDirection)
        {
            throwShieldUp(direction);
        }
        else if (!throwDirection)
        {
            reverseThrow(direction);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (throwDirection && !collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.name == "dummy")
            {
                DummyController dummy = collision.gameObject.GetComponent<DummyController>();
                if (dummy != null)
                {
                    dummy.takeDamage(shieldThrowDamage);
                }
            }
            throwDirection = false;
        }
        else if (!throwDirection && !collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.name == "dummy")
            {
                DummyController dummy = collision.gameObject.GetComponent<DummyController>();
                if (dummy != null)
                {
                    dummy.takeDamage(shieldThrowDamage);
                    retShield();
                    Destroy(this.gameObject);
                }
            }
            StartCoroutine(shieldMissDelay());
        }
        else
        {
            retShield();
            Destroy(this.gameObject);
        }
    }

    private void reverseThrow(Vector2 direction)
    {
        anim.SetTrigger("stopSpin");
        anim.SetTrigger("throw reverse");
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * throwSpeed * 20 * -1);
    }

    private void throwShieldUp(Vector2 direction)
    {
        anim.SetTrigger("throw");
        rb.AddForce(direction * throwSpeed * 10);
    }

    IEnumerator shieldMissDelay()
    {
        yield return new WaitForSeconds(2);
        retShield();
        Destroy(this.gameObject);
    }

    private void retShield()
    {
        shield.gameObject.GetComponent<Shield>().isActive = true;
        shield.SetActive(true);
    }
}
