using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour, Weapon
{
    private Animator animator;
    private EdgeCollider2D swordCollider;
    private AttackMode attackMode;
    PlayerController player;

    public float stabDamage = 0f;
    public float slashDamage = 0f;
    public float lungeDamage = 0f;
    public float spinDamage = 0f;
    public float boomerangDamage = 0f;
    public float force = 0f;
    public bool isActive;
    public Transform swordSpawn;
    public GameObject swordPrefab;

    private void Start()
    {
        player = this.gameObject.GetComponentInParent<PlayerController>();
        animator = this.gameObject.GetComponent<Animator>();
        swordCollider = this.gameObject.GetComponent<EdgeCollider2D>();
        swordCollider.enabled = false;
        isActive = true;
        attackMode = AttackMode.None;
    }

    void Weapon.lightDirectional(bool facingRight)
    {
        if (isActive) 
        {
            stab();
        }
        
    }

    void Weapon.lightNonDirectional(bool facingRight)
    {
        if (isActive) 
        {
            slash();
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
            swordThrow();
        }
        
    }

    public void heavyDirectional(bool facingRight)
    {
        if (isActive)
        {
            lunge(facingRight);
        }

    }

    public void heavyNonDirectional(bool facingRight)
    {
        if (isActive)
        {
            spin();
        }

    }

    public void heavyUp()
    {
        if (isActive)
        {
            upSwipe();
        }
    }

    public void heavyDown()
    {
        Debug.Log("N/A");
    }

    public void disableSwordCollider()
    {
        swordCollider.enabled = false;
        attackMode = AttackMode.None;
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
                    switch (attackMode)
                    {
                        case AttackMode.Stab:
                            dummy.takeDamage(stabDamage);
                            disableSwordCollider();
                            break;
                        case AttackMode.Slash:
                            dummy.takeDamage(slashDamage);
                            disableSwordCollider();
                            break;
                        case AttackMode.Spin:
                            dummy.takeDamage(spinDamage);
                            disableSwordCollider();
                            break;
                        case AttackMode.Lunge:
                            dummy.takeDamage(lungeDamage);
                            disableSwordCollider();
                            break;
                        case AttackMode.UpSwipe:
                            dummy.takeDamage(slashDamage);
                            disableSwordCollider();
                            break;
                        case AttackMode.None:
                        default:
                            break;
                    }
                }
            }
        }
        else
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            switch (attackMode)
            {
                case AttackMode.Stab:
                    ClientSend.SendDamage(player.id, stabDamage);
                    disableSwordCollider();
                    break;
                case AttackMode.Slash:
                    ClientSend.SendDamage(player.id, slashDamage);
                    disableSwordCollider();
                    break;
                case AttackMode.Spin:
                    ClientSend.SendDamage(player.id, spinDamage);
                    disableSwordCollider();
                    break;
                case AttackMode.Lunge:
                    ClientSend.SendDamage(player.id, lungeDamage);
                    disableSwordCollider();
                    break;
                case AttackMode.UpSwipe:
                    ClientSend.SendDamage(player.id, slashDamage);
                    disableSwordCollider();
                    break;
                default:
                    break;
            }
            Physics2D.IgnoreCollision(this.gameObject.GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>());
        }
    }

    private enum AttackMode
    {
        Stab,
        Slash,
        Lunge,
        UpSwipe,
        Spin,
        Boomerang,
        None
    }

    public enum Skills { 
        Stab,
        Slash,
        Lunge,
        UpSwipe,
        Spin,
        Boomerang
    }

    private void stab() 
    {
        swordCollider.enabled = true;
        attackMode = AttackMode.Stab;
        animator.SetTrigger("stab");
    }

    private void lunge(bool facingRight) 
    {
        if (!facingRight) force *= -1;

        attackMode = AttackMode.Lunge;
        swordCollider.isTrigger = true;
        swordCollider.enabled = true;
        animator.SetTrigger("lunge");
        player.AddForce(force);
    }

    private void slash() 
    {
        swordCollider.enabled = true;
        attackMode = AttackMode.Slash;
        animator.SetTrigger("slash");
    }

    private void spin() 
    {
        swordCollider.enabled = true;
        attackMode = AttackMode.Spin;
        animator.SetTrigger("spin");
        setPlayerAnimation("benSpin");
    }

    private void upSwipe() 
    {
        swordCollider.enabled = true;
        attackMode = AttackMode.UpSwipe;
        animator.SetTrigger("up_swipe");
    }

    private void swordThrow() 
    {
        Instantiate(swordPrefab, swordSpawn.position, swordSpawn.rotation);
    }

    public void setPlayerAnimation(string anim) 
    {
        Animator playerAnim = player.GetAnimator();

        playerAnim.SetTrigger(anim);
    }
}