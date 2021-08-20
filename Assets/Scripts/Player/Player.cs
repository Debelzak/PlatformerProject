using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Deb.PlayerState;

public class Player : MonoBehaviour
{
    public Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider;
    public Animator animator;

    [Header("Animation and Particles")]
    public ParticleSystem footDustEffect;
    public ParticleSystem footImpactEffect;

    [Header("Physics")]
    public LayerMask groundMask;
    public PhysicsMaterial2D slliperyMaterial;
    public PhysicsMaterial2D frictionMaterial;
    public float gravityScale = 2;
    public float moveSpeed = 5f;
    public float minJumpForce = 1f;
    public float maxJumpForce = 12f;
    
    float facingDirection = 1;

    public Vector2 velocity;
    float horizontalInput;
    public bool isGrounded, wasGroundedLastFrame;

    /////////////////////////
    // States
    /////////////////////////
    private State playerState;
    public readonly IdleState idleState;
    public readonly WalkState walkState;
    public readonly JumpState jumpState;
    public readonly FallState fallState;

    public Player()
    {
        idleState = new IdleState();
        walkState = new WalkState();
        jumpState = new JumpState();
        fallState = new FallState();
    }
    
    /////////////////////////
    // Unity handle
    ////////////////////////
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>(); 
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        rigidBody.gravityScale = gravityScale;
        SetState(idleState);
    }

    private void Update()
    {
        velocity = new Vector2(Mathf.Round(rigidBody.velocity.x * 100f) / 100f, Mathf.Round(rigidBody.velocity.y * 100f) / 100f);

        playerState.Update(this);
        horizontalInput = Input.GetAxisRaw("Horizontal");
        Flip();
    }

    private void FixedUpdate()
    {
        playerState.FixedUpdate(this);
        HandleTerrain();
    }

    private void LateUpdate() {
        wasGroundedLastFrame = isGrounded;
    }

    public void SetState(State state)
    {
        playerState = state;
        playerState.OnEnterState(this);
    }

    private bool standingOnSlope, movingOnSlope;
    private void HandleTerrain() {
        int rayCount = 3;
        float rayLength = 0.25f;

        isGrounded = false;

        Vector2 movingDirection;
        movingDirection.x = (velocity.x != 0) ? Mathf.Sign(velocity.x) : 0;
        movingDirection.y = (velocity.y != 0) ? Mathf.Sign(velocity.y) : 0;

        for(int i = 0; i < rayCount; i++) {
            float raySpacing = (i==0) ? 0 : boxCollider.size.x / i;
            Vector2 rayOrigin = new Vector2(boxCollider.bounds.min.x + raySpacing, boxCollider.bounds.min.y);
            Vector2 rayDirection = Vector2.down;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, groundMask);

            if(hit) isGrounded = true;

            if(isGrounded) {
                float floorAngle = Vector2.Angle(hit.normal, Vector2.up);
                Vector2 floorAnglePerpendicular = Vector2.Perpendicular(hit.normal).normalized;
                standingOnSlope = (floorAngle > 0 && floorAngle < 90 && horizontalInput == 0);
                movingOnSlope = (floorAngle > 0 && floorAngle < 90 && horizontalInput != 0);
 
                if(movingOnSlope && rigidBody.velocity.y < maxJumpForce - minJumpForce) {
                    rigidBody.velocity = Vector2.zero;
                    Vector2 moveAmount = new Vector2();
                    Vector2 moveDirectionAngle = floorAnglePerpendicular;

                    Vector2 rayCastHorizontalOrigin = (horizontalInput == -1) ? new Vector2(boxCollider.bounds.min.x, boxCollider.bounds.min.y + boxCollider.size.y / 2) : new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.min.y + boxCollider.size.y / 2);
                    Vector2 rayCastHorizontalDirection = (horizontalInput == -1) ? Vector2.left : Vector2.right;
                    RaycastHit2D hitHorizontal = Physics2D.Raycast(rayCastHorizontalOrigin, rayCastHorizontalDirection, 0.05f, groundMask);
                    
                    moveAmount = moveDirectionAngle * moveSpeed * -horizontalInput;

                    if(hitHorizontal) {
                        moveAmount = Physics2D.gravity;
                    }
                    
                    rigidBody.MovePosition(rigidBody.position + moveAmount * Time.deltaTime);
                    
                }

                Debug.DrawLine(rayOrigin, hit.point, Color.red);
                Debug.DrawRay(hit.point, floorAnglePerpendicular, Color.green);
            }

            if(hit) break;
        }
        
        boxCollider.sharedMaterial = (standingOnSlope && isGrounded) ? frictionMaterial : slliperyMaterial;
    }

    private void Flip() {
        if( (facingDirection == 1 && horizontalInput < 0) || (facingDirection == -1 && horizontalInput > 0) ) {
            facingDirection = -facingDirection;
        }
        transform.localScale = new Vector3(facingDirection, transform.localScale.y, transform.localScale.z);
    }
}
