using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyController : MonoBehaviour
{
    public float maxHealth = 20f;
    public float currentHealth;
    private Animator animator;
    void Start()
    {
        currentHealth = maxHealth;
        animator = this.gameObject.GetComponent<Animator>();
    }

    public void takeDamage(float damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            animator.SetTrigger("die");
            currentHealth = maxHealth;
        } else
        {
            animator.SetTrigger("takeDamage");
        }
    }

    public float getHealth() 
    {
        return currentHealth;
    }

}
