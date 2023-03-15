using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    
    public int health { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 150;
    }

    void TakeDamage(int dmg)
    {
        health -= dmg;
    }
}
