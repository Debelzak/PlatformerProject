using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Player : MonoBehaviour
{
    [Header("Components")]
    public Actor physics;
    public BoxCollider2D collider2d;
    public Animator animator;
    public LayerMask groundMask;
    public PhysicsMaterial2D slliperyMaterial;
    public PhysicsMaterial2D frictionMaterial;

    [Header("Animation and Particles")]
    public ParticleSystem footDustEffect;
    public ParticleSystem footImpactEffect;

    [Header("Physics")]
    public float moveSpeed = 5f;
    public float minJumpForce = 1f;
    public float maxJumpForce = 12f;

    //Ground check
    public bool isGrounded {get; private set;}
    public bool wasGroundedLastFrame {get; private set;}

    private float facingDirection = 1;
    private float horizontalInput;

    // Player state machine
    public PlayerState State {get; private set;}

    public Player()
    {
        State = new PlayerState();
    }
    
    private void Awake()
    {
        physics = GetComponent<Actor>();
        collider2d = GetComponent<BoxCollider2D>(); 
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        State.Set(this, (int)PlayerState.Type.STATE_IDLE);
    }
    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        /// TEMPORARY
        physics.velocity.x = horizontalInput * moveSpeed;
        //
        if(Input.GetButtonDown("Jump"))
        {
            physics.DropFromPlatform();
        }
        isGrounded = physics.collisionStatus.CollisionBelow;
        HandleFacingDirection();
        State.OnUpdate(this);

        wasGroundedLastFrame = isGrounded;
    }
    private void FixedUpdate()
    {
        State.OnFixedUpdate(this);
    }

    private void HandleFacingDirection() 
    {
        if( (facingDirection == 1 && horizontalInput < 0) || (facingDirection == -1 && horizontalInput > 0) ) 
        {
            facingDirection = -facingDirection;
        }

        transform.localScale = new Vector3(facingDirection, transform.localScale.y, transform.localScale.z);
    }
}