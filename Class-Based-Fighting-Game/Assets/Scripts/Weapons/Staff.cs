using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : MonoBehaviour, Weapon
{
    private Animator animator;
    private CircleCollider2D staffCollider;
    private AttackMode attackMode;

    public float whackDamage = 0f;
    public Transform vortexSpawn;
    public GameObject vortexPrefab;
    public Transform icicleSpawn;
    public GameObject iciclePrefab;
    public Transform gustSpawn;
    public GameObject gustPrefab;
    public Transform forceFieldSpawn;
    public GameObject forceFieldPrefab;
    public Transform thunderSpawn;
    public GameObject thunderPrefab;

    private bool thunderTracker;
    private GameObject stormCloud;

    private void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();
        staffCollider = this.gameObject.GetComponent<CircleCollider2D>();
        staffCollider.enabled = false;
        attackMode = AttackMode.None;
        thunderTracker = false;
    }

    private void Update()
    {
        if (stormCloud == null) 
        {
            thunderTracker = false;
        }
    }

    void Weapon.lightDirectional(bool facingRight)
    {
        whack();
    }

    void Weapon.lightNonDirectional(bool facingRight)
    {
        iceBolt(facingRight);
    }

    void Weapon.lightDown()
    {
       Debug.Log("N/A"); 
    }

    void Weapon.lightUp()
    {
        gust();
    }

    void Weapon.heavyDirectional(bool facingRight)
    {
        storm(facingRight);
    }

    void Weapon.heavyNonDirectional(bool facingRight)
    {
        vortex();
    }

    void Weapon.heavyUp()
    {
        forceField();
    }

    void Weapon.heavyDown()
    {
        Debug.Log("N/A");
    }

    public void disableCollider() 
    {
        staffCollider.enabled = false;
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
                        case AttackMode.Whack:
                            dummy.takeDamage(whackDamage);
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
            if(attackMode == AttackMode.Whack) ClientSend.SendDamage(player.id, whackDamage);
        }    
    }

    private enum AttackMode 
    { 
        Whack,
        Vortex,
        IceBolt,
        Thunder,
        None
    }

    public enum Skills {
        Whack,
        IceBolt,
        Vortex,
        Gust,
        Thunder,
        ForceField
    }

    private void iceBolt(bool facingRight) 
    {
        attackMode = AttackMode.IceBolt;
        animator.SetTrigger("ice bolt");
        var icicle = Instantiate(iciclePrefab, icicleSpawn.position, icicleSpawn.rotation);
        icicle.gameObject.GetComponent<Icicle>().setRotation(facingRight);
    }

    private void whack() 
    {
        staffCollider.enabled = true;
        attackMode = AttackMode.Whack;
        animator.SetTrigger("whack");
    }

    private void vortex() 
    {
        attackMode = AttackMode.Vortex;
        animator.SetTrigger("hold");
        var vortex = Instantiate(vortexPrefab, vortexSpawn);
    }

    private void gust() 
    {
        animator.SetTrigger("thunder");
        var gust = Instantiate(gustPrefab, gustSpawn);
    }

    private void storm(bool facingRight) 
    {
        if (!thunderTracker)
        {
            callThunder(facingRight);
            thunderTracker = true;
        }
        else
        {
            callLightning();
            thunderTracker = false;
        }
    }
    private void callThunder(bool facingRight) 
    {
        animator.SetTrigger("thunder");
        var thunder = Instantiate(thunderPrefab, thunderSpawn.position, thunderSpawn.rotation);
        thunder.gameObject.GetComponent<Thunder>().setRotation(facingRight);
        thunderTracker = true;
        stormCloud = thunder;
    }

    private void callLightning() 
    {
        stormCloud.gameObject.GetComponent<Thunder>().stopCloud();
    }

    private void forceField() 
    {
        animator.SetTrigger("forcefield");
        var ff = Instantiate(forceFieldPrefab, forceFieldSpawn);
    }
}
