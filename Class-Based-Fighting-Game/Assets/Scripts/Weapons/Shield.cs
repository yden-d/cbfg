using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour, Weapon
{
    private Animator animator;
    private EdgeCollider2D shieldCollider;
    private ShieldMode shieldMode;
    PlayerController player;
    public bool isActive;
    public float bashDamage = 0f;
    public float chargeDamage = 0f;
    public float force = 0f;
    public Transform shieldSpawn;
    public GameObject shieldPrefab;
    public Transform upShieldSpawn;
    public GameObject upShieldPrefab;

    private void Start()
    {
        player = this.gameObject.GetComponentInParent<PlayerController>();
        animator = this.gameObject.GetComponent<Animator>();
        shieldCollider = this.gameObject.GetComponent<EdgeCollider2D>();
        shieldCollider.enabled = false;
        shieldMode = ShieldMode.None;
        isActive = true;
    }


    void Weapon.lightDirectional(bool facingRight)
    {
        if (isActive)
        {
            bash();
        }
    }

    void Weapon.lightNonDirectional(bool facingRight)
    {
        if (isActive) 
        {
            shieldThrow(facingRight);
        }
    }

    void Weapon.lightDown()
    {
        Debug.Log("N/A");
    }

    void Weapon.lightUp()
    {
        if (isActive)
        {
            upThrow();
        }
    }

    void Weapon.heavyDirectional(bool facingRight)
    {
        charge(facingRight);
    }

    // Embiggen shield
    void Weapon.heavyNonDirectional(bool facingRight)
    {
        if (isActive) 
        {
            isActive = false;
            block();
        }
    }

    void Weapon.heavyUp()
    {
        while(isActive) 
        {
            isActive = false;
            up_block();
        }
    }

    void Weapon.heavyDown()
    {
        Debug.Log("N/A");
    }

    public void disableCollider()
    {
        shieldCollider.enabled = false;
        shieldMode = ShieldMode.None;
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
                    switch (shieldMode)
                    {
                        case ShieldMode.Bash:
                            dummy.takeDamage(bashDamage);
                            disableCollider();
                            break;
                        case ShieldMode.None:
                        case ShieldMode.Block:
                        case ShieldMode.charge:
                            dummy.takeDamage(chargeDamage);
                            disableCollider();
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        else
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            switch (shieldMode)
            {
                case ShieldMode.Bash:
                    ClientSend.SendDamage(player.id, bashDamage);
                    disableCollider();
                    break;
                case ShieldMode.charge:
                    ClientSend.SendDamage(player.id, chargeDamage);
                    disableCollider();
                    break;
                default:
                    break;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(this.gameObject.GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>());
        }
    }

    private enum ShieldMode 
    {
        None,
        Bash,
        Block,
        charge
    }

    // Upwards blocking ability
    void up_block() 
    {
        shieldMode = ShieldMode.Block;
        shieldCollider.isTrigger = false;
        shieldCollider.enabled = true;
        animator.SetTrigger("up_block");
    }

    // Forward blocking ability
    void block() 
    {
        shieldMode = ShieldMode.Block;
        shieldCollider.isTrigger = false;
        shieldCollider.enabled = true;
        animator.SetTrigger("block");
    }

    // Ability where you whack whatever is in front of you with shield
    void bash() 
    {
        shieldMode = ShieldMode.Bash;
        shieldCollider.isTrigger = true;
        shieldCollider.enabled = true;
        animator.SetTrigger("bash");
    }

    // Charge forward and hit with field
    void charge(bool facingRight) 
    {
        if (!facingRight) force *= -1;

        shieldMode = ShieldMode.Bash;
        shieldCollider.isTrigger = true;
        shieldCollider.enabled = true;
        player.AddForce(force);
    }

    void shieldThrow(bool facingRight) 
    {
        var throwShield = Instantiate(shieldPrefab, shieldSpawn.position, shieldSpawn.rotation);
        throwShield.gameObject.GetComponent<ShieldThrow>().setRotation(facingRight);
    }

    void upThrow() 
    {
        Instantiate(upShieldPrefab, upShieldSpawn.position, upShieldSpawn.rotation);
    }

    public enum Skills {
        Bash,
        Block,
        UpBlock,
        Charge,
        Throw,
        UpThrow
    }

    public GameObject returnObj() 
    {
        return this.gameObject;
    }

    public void setActive() 
    {
        isActive = true;
    }
}
