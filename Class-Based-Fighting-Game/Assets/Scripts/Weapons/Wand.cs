using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand : MonoBehaviour, Weapon
{
    private Animator animator;
    private CircleCollider2D wandCollider;
    private AttackMode attackMode;
    PlayerController player;

    public float slashDamage;
    public float fireworkDamage;
    public float laserDamage;
    public float teleportDistance;
    public Transform fireballSpawn;
    public Transform laserSpawn;
    public Transform confusionSpawn;
    public GameObject laserPrefab;
    public GameObject fireballPrefab;
    public GameObject confusionPrefab;
    private Vector2 facing2;

    public float rayDistance;
    public float teleportBuffer;

    private void Start()
    {
        player = this.gameObject.GetComponentInParent<PlayerController>();
        animator = this.gameObject.GetComponent<Animator>();
        wandCollider = this.gameObject.GetComponent<CircleCollider2D>();
        wandCollider.enabled = false;

        this.attackMode = AttackMode.None;
    }

    private void Update()
    {
        Vector3 facing = player.IsFacingRight() ? Vector3.right : Vector3.left;
        facing2.x = facing.x;
        facing2.y = facing.y;
    }

    void Weapon.lightDirectional(bool facingRight)
    {
        slash();
    }

    void Weapon.lightNonDirectional(bool facingRight)
    {
        fireball(facingRight);
    }

    void Weapon.lightDown()
    {
        Debug.Log("N/A");
    }

    void Weapon.lightUp()
    {
        Debug.Log("N/A");
    }

    void Weapon.heavyDirectional(bool facingRight)
    {
        teleport(facingRight);
    }

    void Weapon.heavyNonDirectional(bool facingRight)
    {
        confusion(facingRight);
    }

    void Weapon.heavyUp()
    {
        laser();
    }

    void Weapon.heavyDown()
    {
        Debug.Log("N/A");
    }

    public void disableCollider()
    {
        this.wandCollider.enabled = false;
        attackMode = AttackMode.None;
    }
    private enum AttackMode
    {
        None,
        Slash,
        Firework,
        Laser
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
                        case AttackMode.Slash:
                            dummy.takeDamage(slashDamage);
                            break;
                        case AttackMode.Firework:
                            dummy.takeDamage(fireworkDamage);
                            break;
                        case AttackMode.Laser:
                            dummy.takeDamage(laserDamage);
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
                case AttackMode.Slash:
                    ClientSend.SendDamage(player.id, slashDamage);
                    break;
                case AttackMode.Firework:
                    ClientSend.SendDamage(player.id, fireworkDamage);
                    break;
                case AttackMode.Laser:
                    ClientSend.SendDamage(player.id, laserDamage);
                    break;
                default:
                    break;
            }
        }
    }

    public enum Skills {
        Fireball,
        Slash,
        Firework,
        Laser,
        Teleport,
        Confusion
    }

    private void fireball(bool facingRight) 
    {
        animator.SetTrigger("shoot");
        var fireball = Instantiate(fireballPrefab, fireballSpawn.position, fireballSpawn.rotation);
        fireball.gameObject.GetComponent<Fireball>().setRotation(facingRight);
    }
    private void slash()
    {
        attackMode = AttackMode.Slash;
        wandCollider.enabled = true;
        animator.SetTrigger("slash");
    }
    private void firework() { }
    private void laser()
    {
        animator.SetTrigger("laser");
        var laser = Instantiate(laserPrefab, laserSpawn);
    }
    private void teleport(bool facingRight) 
    {
        animator.SetTrigger("teleport");
        CheckWallCollision(facing2);
    }
    private void confusion(bool facingRight)
    {
        animator.SetTrigger("shoot");
        var confusion = Instantiate(confusionPrefab, confusionSpawn.position, confusionSpawn.rotation);
        confusion.gameObject.GetComponent<Confusion>().setRotation(facingRight);
    }

    private void CheckWallCollision(Vector2 direction)
    {
        Vector3 pos = player.gameObject.transform.position;
        float TPDist = rayDistance;

        if (!player.IsFacingRight())
        {
            TPDist *= -1;
        }
            
        pos.x += TPDist;

        player.gameObject.transform.position = pos;
    }
}