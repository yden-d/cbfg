using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour, Weapon
{
    private Animator animator;
    public GameObject arrowPrefab;
    public Transform upArrowSpawn;
    public Transform forwardArrowSpawn;
    public Transform downArrowSpawn;
    public float shotDelay = 0f;
    public float whackDamage = 0f;
    private EdgeCollider2D bowCollider;
    private BowMode bowMode;
    private void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();
        bowCollider = this.gameObject.GetComponent<EdgeCollider2D>();
        bowCollider.enabled = false;
    }

    void Weapon.lightDirectional(bool facingRight)
    {
        bowCollider.enabled = true;
        animator.SetTrigger("whack");
    }

    void Weapon.lightNonDirectional(bool facingRight)
    {
        animator.SetTrigger("shoot");
        var newArrow = Instantiate(arrowPrefab, forwardArrowSpawn.position, forwardArrowSpawn.rotation);
        Arrow script = newArrow.GetComponent<Arrow>();
        script.setRotation(facingRight);
        script.setArrowSpeed(false);
        script.setIsVolley(false);
    }

    void Weapon.lightDown()
    {
        Debug.Log("N/A");
    }

    void Weapon.lightUp()
    {
        animator.SetTrigger("singleVolley");
    }

    void Weapon.heavyDirectional(bool facingRight)
    {
        animator.SetTrigger("shoot");
        var newArrow = Instantiate(arrowPrefab, forwardArrowSpawn.position, forwardArrowSpawn.rotation);
        Arrow script = newArrow.GetComponent<Arrow>();
        script.setRotation(facingRight);
        script.setArrowSpeed(true);
        script.setIsVolley(false);
    }

    void Weapon.heavyNonDirectional(bool facingRight)
    {
        animator.SetTrigger("shoot");

        //Spawn the forward arrow
        var arrowForward = Instantiate(arrowPrefab, forwardArrowSpawn.position, forwardArrowSpawn.rotation);
        Arrow script = arrowForward.GetComponent<Arrow>();
        script.setRotation(facingRight);
        script.setArrowSpeed(false);
        script.setIsVolley(false);

        //Spawn the up arrow
        var arrowUp = Instantiate(arrowPrefab, upArrowSpawn.position, upArrowSpawn.rotation);
        script = arrowUp.GetComponent<Arrow>();
        script.setRotation(facingRight);
        script.setArrowSpeed(false);
        script.setIsVolley(false);

        //Spawn the down arrow
        var arrowDown = Instantiate(arrowPrefab, downArrowSpawn.position, downArrowSpawn.rotation);
        script = arrowDown.GetComponent<Arrow>();
        script.setRotation(facingRight);
        script.setArrowSpeed(false);
        script.setIsVolley(false);

    }

    void Weapon.heavyUp()
    {
        animator.SetTrigger("multiVolley");
    }

    void Weapon.heavyDown()
    {
        Debug.Log("N/A");
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
                    dummy.takeDamage(whackDamage);
                }
            }
        }
        else
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            ClientSend.SendDamage(player.id, whackDamage);
        }
    }

    public void WhackEnd() {
        bowCollider.enabled = false;
    }

    public void fireSingleVolley() {
        var newArrow = Instantiate(arrowPrefab, forwardArrowSpawn.position, forwardArrowSpawn.rotation);
        Arrow script = newArrow.GetComponent<Arrow>();
        script.setRotation(this.gameObject.GetComponentInParent<PlayerController>().IsFacingRight());
        script.setArrowSpeed(false);
        script.setIsVolley(true);
    }

    public void fireMultiVolley()
    {
        //Get if we are facing right
        bool facingRight = this.gameObject.GetComponentInParent<PlayerController>().IsFacingRight();

        //Spawn the forward arrow
        var arrowForward = Instantiate(arrowPrefab, forwardArrowSpawn.position, forwardArrowSpawn.rotation);
        Arrow script = arrowForward.GetComponent<Arrow>();
        script.setRotation(facingRight);
        script.setArrowSpeed(false);
        script.setIsVolley(true);

        //Spawn the up arrow
        var arrowUp = Instantiate(arrowPrefab, upArrowSpawn.position, upArrowSpawn.rotation);
        script = arrowUp.GetComponent<Arrow>();
        script.setRotation(facingRight);
        script.setArrowSpeed(false);
        script.setIsVolley(true);

        //Spawn the down arrow
        var arrowDown = Instantiate(arrowPrefab, downArrowSpawn.position, downArrowSpawn.rotation);
        script = arrowDown.GetComponent<Arrow>();
        script.setRotation(facingRight);
        script.setArrowSpeed(false);
        script.setIsVolley(true);
    }

    private enum BowMode
    {
        None,
        SingleVolley,
        MultiVolley,
    }

    public enum Skills { 
        ExplodingShot,
        FasterShot,
        MultiShot,
        SingleVolley,
        MultiVolley,
        Swipe,
        SingleShot
    };
}
