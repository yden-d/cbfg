using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : MonoBehaviour
{
    // -------------- private --------------

    // Rigidbody for the player
    private Rigidbody2D rb;

    // Flag to determine facing direction
    private bool facingRight;

    // Animator for player
    private Animator animator;

    // Frames that wielded weapon's input button is held down for
    private float wieldedWeaponFramesDown;

    // Player's max health
    private float maxHealth;


    // -------------- public --------------

        // Currently equipped primary weapon
    public GameObject primaryWeapon {get; set;}

    // Currently equipped secondary weapon
    public GameObject secondaryWeapon {get; set;}

    // Currently wielded weapon
    public GameObject wieldedWeapon {get; set;}

    // Determines if the wielded weapon is the primary or secondary weapon
    private bool wieldingPrimary {get; set;}

    // LayerMask for scene objects
    public LayerMask scene;
    
    // Movement speed scalar
    public float movement_scalar;

    // Jump force scalar
    public float jump_scalar;

    // Player max movement speed
    public float max_speed;

    // Ray length to determine how far off the ground the player can jump
    public float jump_ray;

    // Ray length to determine how far away objects collide
    public float collision_ray;

    // The delay time from when the jump button has been pressed and when the player leaves the ground
    public float jump_delay;

    // Attack threashold
    public float attackThreshold;

    // Player can move
    public bool canMove;

    // References to equippable weapons
    public List<GameObject> weapons;

    //Client id for networking
    public int id;

    // Player's current health
    public float health { get; private set; }

    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        animator = this.gameObject.GetComponent<Animator>();
        facingRight = true;
        wieldedWeaponFramesDown = 0;
        canMove = true;
        SetAllWeaponsInactive();
        maxHealth = 150;
        health = maxHealth;
        EquipWeapons();
    }

    // Player takes damage
    public void TakeDamage(float dmg)
    {
        health -= dmg;
        if(health <= 0) Respawn();
        Debug.Log($"Player {id} took {dmg} damage... now has {health} health");
    }

    // Player respawns
    private void Respawn()
    {
        this.gameObject.SetActive(false);
        health = maxHealth;
        this.gameObject.transform.position = new Vector3(0,0,0);
        this.gameObject.SetActive(true);
    }


    public void EquipWeapons() {
        switch(PlayerPrefs.GetString("secondary")) {
            case "sword":
                EquipSecondary(Weapon.Weapon.Sword);
                break;
            case "shield":
                EquipSecondary(Weapon.Weapon.Shield);
                break;
            case "wand":
                EquipSecondary(Weapon.Weapon.Wand);
                break;
            case "staff":
                EquipSecondary(Weapon.Weapon.Staff);
                break;
            case "bow":
                EquipSecondary(Weapon.Weapon.Bow);
                break;
            default:
                break;
        }
        switch (PlayerPrefs.GetString("primary"))
        {
            case "sword":
                EquipPrimary(Weapon.Weapon.Sword);
                break;
            case "shield":
                EquipPrimary(Weapon.Weapon.Shield);
                break;
            case "wand":
                EquipPrimary(Weapon.Weapon.Wand);
                break;
            case "staff":
                EquipPrimary(Weapon.Weapon.Staff);
                break;
            case "bow":
                EquipPrimary(Weapon.Weapon.Bow);
                break;
            default:
                break;
        }
    }

    private void FixedUpdate()
    {
        if(id != Client.instance.id) return;
        
        // // Handle movement in fixed update for accurate physics
        HandleMovementInput();
        ClientSend.SendPos(rb.transform.position);
        Vector3 currScale = gameObject.transform.localScale;
        ClientSend.SendRot(currScale.x);
    }

    private void Update()
    {
        if(id != Client.instance.id) return;
        HandleCombatInput();
    } 

    private void HandleCombatInput()
    {
        if(Input.GetKeyDown(KeyCode.S)){
            SwapWielding();
            ClientSend.Swap();
        }

        // Get if the down input is held
        bool downDirectional = Input.GetButton("Down Directional");
        // Get if the Left or right input is held
        bool directional = Input.GetButton("Directional");
        // Get if the up input is held
        bool upDirectional = Input.GetButton("Up Directional");
        // Get if the wielded weapon's input is held
        bool wieldedWeaponInput = Input.GetMouseButton(0);

        // If the wielded weapon's button is pushed down, increment its timer count
        if (wieldedWeaponInput)
        {
            wieldedWeaponFramesDown += Time.deltaTime;
        }
        // If the player has held/ clicked the wielded weapon's button for at most the attack threshold, then released it, it performs a light attack
        else if (!wieldedWeaponInput && wieldedWeaponFramesDown < attackThreshold && wieldedWeaponFramesDown > 0)
        {
            if (wieldedWeapon != null)
            {
                // Get the weapon's interface script from the wielded weapon slot
                Weapon wieldedWeaponScript = wieldedWeapon.GetComponent<Weapon>();
                if (wieldedWeaponScript != null)
                {
                    // Player has tapped the wielded weapon's input while holding the down input
                    if (downDirectional)
                    {
                        wieldedWeaponScript.lightDown();
                        ClientSend.Attack(1, facingRight);
                    }
                    // Player has tapped the wielded weapon's input while holding the left or right input
                    else if (directional)
                    {
                        wieldedWeaponScript.lightDirectional(facingRight);
                        ClientSend.Attack(2, facingRight);
                    }
                    // Player has tapped the wielded weapon's input while holding the up input
                    else if (upDirectional)
                    {
                        wieldedWeaponScript.lightUp();
                        ClientSend.Attack(3, facingRight);
                    }
                    // Player has tapped the wielded weapon's input while not holding a directional input down
                    else
                    {
                        wieldedWeaponScript.lightNonDirectional(facingRight);
                        ClientSend.Attack(4, facingRight);
                    }
                }
            }
            //Reset the wielded weapon's held timer
            wieldedWeaponFramesDown = 0;
        }
        // If the player has held the wielded weapon's button for at least the attack threshold, then released the button, it performs a heavy attack
        else if (!wieldedWeaponInput && wieldedWeaponFramesDown >= attackThreshold)
        {
            if (wieldedWeapon != null)
            {
                // Get the weapon's interface script from the wielded weapon's slot
                Weapon wieldedWeaponScript = wieldedWeapon.GetComponent<Weapon>();
                if (wieldedWeaponScript != null)
                {
                    // Player has held the wielded weapon's input while holding the down input
                    if (downDirectional)
                    {
                        wieldedWeaponScript.heavyDown();
                        ClientSend.Attack(5, facingRight);
                    }
                    // Player has held the wielded weapon's input while holding the left or right input
                    else if (directional)
                    {
                        wieldedWeaponScript.heavyDirectional(facingRight);
                        ClientSend.Attack(6, facingRight);
                    }
                    // Player has held the wielded weapon's input while holding the up input
                    else if (upDirectional)
                    {
                        wieldedWeaponScript.heavyUp();
                        ClientSend.Attack(7, facingRight);
                    }
                    // Player has held the wielded weapon's input while not holding a directional input down
                    else
                    {
                        wieldedWeaponScript.heavyNonDirectional(facingRight);
                        ClientSend.Attack(8, facingRight);
                    }
                }
            }
            //Reset the primary held timer
            wieldedWeaponFramesDown = 0;
        }
    }

    // Determines if the player can move and moves them
    private void HandleMovementInput()
    {
        float x_movement = Input.GetAxis("Horizontal");

        if (canMove)
        {
            rb.velocity = new Vector2(x_movement * movement_scalar, rb.velocity.y);
            //Moving right
            if (x_movement > 0)
            {
                if (!facingRight)
                {
                    FlipCharacter();
                }
                CheckWallCollision(Vector2.right);
            }

            //Moving left
            if (x_movement < 0)
            {
                if (facingRight)
                {
                    FlipCharacter();
                }
                CheckWallCollision(Vector2.left);
            }
        }
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        
        //Handle jumping
        if (Input.GetButton("Jump") && IsGrounded())
        {
            animator.SetBool("isJumping", true);
            StartCoroutine(JumpDelay());
        }
        else
        {
            animator.SetBool("isJumping", false);
        }
    }

    // Adds a delay to the jump for a smoother animation
    IEnumerator JumpDelay() {
        yield return new WaitForSeconds(jump_delay);
        rb.velocity = new Vector2(rb.velocity.x, jump_scalar);
    }

    // Checks to see if the player on the ground
    private bool IsGrounded() {
        RaycastHit2D groundCheck = Physics2D.Raycast(transform.position, Vector2.down, jump_ray, scene);
        return groundCheck.collider != null;
    }

    // Flips the player
    private void FlipCharacter()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
        facingRight = !facingRight;
    }

    // Checks to see if the player is colliding with a wall
    private void CheckWallCollision(Vector2 direction)
    {
        Vector3 headPos = transform.position;
        Vector3 footPos = transform.position;
        headPos.y += 0.525f;
        footPos.y -= 0.525f;
        RaycastHit2D wallCheckBody = Physics2D.Raycast(headPos, direction, collision_ray, scene);
        RaycastHit2D wallCheckHead = Physics2D.Raycast(transform.position, direction, collision_ray, scene);
        RaycastHit2D wallCheckFoot = Physics2D.Raycast(footPos, direction, collision_ray, scene);
        if (wallCheckHead.collider != null || wallCheckBody.collider != null || wallCheckFoot.collider != null)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    // Adds a force to the player from an outside source
    public void AddForce(float change) 
    {
        canMove = false;

        var velocity = rb.velocity;
        velocity.x += change;
        rb.velocity = velocity;

        if (change < 0)
            CheckWallCollision(Vector2.left);
        else
            CheckWallCollision(Vector2.right);

        StartCoroutine(ChargeDelay());
    }

    // Returns if the player is facing right or not
    public bool IsFacingRight() 
    {
        return facingRight;
    }
    
    // Adds a delay after the player charges 
    IEnumerator ChargeDelay()
    {
        yield return new WaitForSeconds(1f);
        canMove = true;
    }

    // Public interface to equip a primary weapon
    public bool EquipPrimary(Weapon.Weapon weapon){
        switch(weapon) {
            case Weapon.Weapon.Bow:
                return _EquipPrimary("bow");
            case Weapon.Weapon.Sword:
                return _EquipPrimary("sword");
            case Weapon.Weapon.Shield:
                return _EquipPrimary("shield");
            case Weapon.Weapon.Wand:
                return _EquipPrimary("wand");
            case Weapon.Weapon.Staff:
                return _EquipPrimary("staff");
            default:
                return false;
        }
    }

    // Public interface to equip a secondary weapon
    public bool EquipSecondary(Weapon.Weapon weapon){
        switch(weapon) {
            case Weapon.Weapon.Bow:
                return _EquipSecondary("bow");
            case Weapon.Weapon.Sword:
                return _EquipSecondary("sword");
            case Weapon.Weapon.Shield:
                return _EquipSecondary("shield");
            case Weapon.Weapon.Wand:
                return _EquipSecondary("wand");
            case Weapon.Weapon.Staff:
                return _EquipSecondary("staff");
            default:
                return false;
        }
    }

    // Returns the currently wielde weapon
    public GameObject GetWielded(){
        return wieldedWeapon;
    }

    // Returns if the wielded weapon is the primary weapon or secondary weapon
    public bool IsPrimaryWielded(){
        return wieldingPrimary;
    }

    // Private logic to handle equipping a primary weapon
    private bool _EquipPrimary(string weapon){
        primaryWeapon = weapons.Find(w => w.name == weapon);
        if(primaryWeapon == null) {
            return false;
        }
        if(wieldedWeapon != null) {
            wieldedWeapon.SetActive(false);
        }
        wieldedWeapon = primaryWeapon;
        wieldingPrimary = true;
        wieldedWeapon.SetActive(true);
        return true;
    }

    // Private logic to handle equipping a secondary weapon
    private bool _EquipSecondary(string weapon){
        secondaryWeapon = weapons.Find(w => w.name == weapon);
        if(secondaryWeapon == null) {
            return false;
        }
        if(wieldedWeapon != null) {
            wieldedWeapon.SetActive(false);
        }
        wieldedWeapon = secondaryWeapon;
        wieldingPrimary = false;
        wieldedWeapon.SetActive(true);
        return true;
    }

    // Swaps the currently wielded weapon to either the primary or secondary depending on what is currently equipped
    public bool SwapWielding(){
        if(primaryWeapon == null || secondaryWeapon == null) {
            return false;
        }

        wieldedWeapon.SetActive(false);
        if(wieldingPrimary) {
            wieldedWeapon = secondaryWeapon;
            wieldingPrimary = false;
        } else {
            wieldedWeapon = primaryWeapon;
            wieldingPrimary = true;
        }
        //ResetWieldedWeaponTransform();
        wieldedWeapon.SetActive(true);
        return true;
    }
    
    // Zero out the position and rotation of the weapon and set the scale to (1,1,1)
    private void ResetWieldedWeaponTransform(){
        wieldedWeapon.transform.position = Vector3.zero;
        wieldedWeapon.transform.localEulerAngles = Vector3.zero;
        wieldedWeapon.transform.localScale = new Vector3(1,1,1);
    }

    // Set all weapons inactive and clear wielded weapon slot
    public void SetAllWeaponsInactive(){
        foreach(GameObject weapon in weapons) {
            weapon.SetActive(false);
        }
        wieldedWeapon = null;
    }

    // Returns the player's animator
    public Animator GetAnimator() 
    {
        return this.animator;
    }

    // Returns the players rigidbody
    public Rigidbody2D getRB() 
    {
        return rb;
    }
}
