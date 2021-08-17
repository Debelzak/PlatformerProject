using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Animation and Particles")]
    public ParticleSystem footDustEffect;
    public ParticleSystem footImpactEffect;
    Animator animator;

    [Header("Physics")]
    public LayerMask groundMask;
    public PhysicsMaterial2D slliperyMaterial;
    public PhysicsMaterial2D frictionMaterial;
    BoxCollider2D coll;
    Rigidbody2D rb;

    float gravityScale = 2;
    float moveSpeed = 5f;
    float minJumpForce = 1f;
    float maxJumpForce = 12f;
    float facingDirection = 1;

    Vector2 velocity, lastFramePosition;
    bool isGrounded, wasGroundedLastFrame;
    bool standingOnSlope, movingOnSlope;

    Vector2 moveInput;
    private float jumpBuffer = 0.05f;
    private float jumpBufferTimer;
    private float jumpExtraMomentum = 0.10f;
    private float jumpMomentumTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>(); 
        animator = GetComponent<Animator>();
        rb.gravityScale = gravityScale;
    }

    //Update Methods
    void Update()
    {
        velocity = new Vector2(Mathf.Round(rb.velocity.x * 100f) / 100f, Mathf.Round(rb.velocity.y * 100f) / 100f);

        HandleInput();
        HandleAnimation();
    }

    void FixedUpdate() {
        HandleTerrain();
    }

    void LateUpdate() {
        wasGroundedLastFrame = isGrounded;
        lastFramePosition = transform.position;
    }


    void HandleTerrain() {
        int rayCount = 3;
        float rayLength = 0.25f;

        isGrounded = false;
        standingOnSlope = false;
        movingOnSlope = false;

        Vector2 movingDirection;
        movingDirection.x = (velocity.x != 0) ? Mathf.Sign(velocity.x) : 0;
        movingDirection.y = (velocity.y != 0) ? Mathf.Sign(velocity.y) : 0;

        for(int i = 0; i < rayCount; i++) {
            float raySpacing = (i==0) ? 0 : coll.size.x / i;
            Vector2 rayOrigin = new Vector2(coll.bounds.min.x + raySpacing, coll.bounds.min.y);
            Vector2 rayDirection = Vector2.down;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, groundMask);

            if(hit) isGrounded = true;

            if(isGrounded) {
                float floorAngle = Vector2.Angle(hit.normal, Vector2.up);
                Vector2 floorAnglePerpendicular = Vector2.Perpendicular(hit.normal).normalized;
                standingOnSlope = (floorAngle > 0 && floorAngle < 90 && moveInput.x == 0);
                movingOnSlope = (floorAngle > 0 && floorAngle < 90 && moveInput.x != 0);
 
                if(movingOnSlope && rb.velocity.y < maxJumpForce - minJumpForce) {
                    rb.velocity = Vector2.zero;
                    Vector2 moveAmount = new Vector2();
                    Vector2 moveDirection = floorAnglePerpendicular;

                    Vector2 rayCastHorizontalOrigin = (moveInput.x == -1) ? new Vector2(coll.bounds.min.x, coll.bounds.min.y + coll.size.y / 2) : new Vector2(coll.bounds.max.x, coll.bounds.min.y + coll.size.y / 2);
                    Vector2 rayCastHorizontalDirection = (moveInput.x == -1) ? Vector2.left : Vector2.right;
                    RaycastHit2D hitHorizontal = Physics2D.Raycast(rayCastHorizontalOrigin, rayCastHorizontalDirection, 0.05f, groundMask);
                    
                    moveAmount.x = moveSpeed * moveDirection.x * -moveInput.x;
                    moveAmount.y = moveSpeed * moveDirection.y * -moveInput.x;

                    if(hitHorizontal) {
                        Debug.DrawLine(rayCastHorizontalOrigin, hit.point, Color.red);
                        moveAmount = Physics2D.gravity;
                    }
                    moveAmount.x = (moveAmount.x * 32) / 32;
                    moveAmount.y = (moveAmount.y * 32) / 32;
                    rb.MovePosition(rb.position + moveAmount * Time.deltaTime);
                    
                }

                Debug.DrawLine(rayOrigin, hit.point, Color.red);
                Debug.DrawRay(hit.point, floorAnglePerpendicular, Color.green);
            }

            if(hit) break;
        }
        
        coll.sharedMaterial = (standingOnSlope && isGrounded) ? frictionMaterial : slliperyMaterial;
    }

    void HandleInput() {
        moveInput.x = (Input.GetAxisRaw("Horizontal") >= 0.5) ? 1 :
                      (Input.GetAxisRaw("Horizontal") <= -0.5) ? -1 :
                      0;
        moveInput.y = Input.GetAxisRaw("Vertical");

        //Jump handle
        if(Input.GetButtonDown("Jump") && Input.GetAxisRaw("Vertical") != -1) { //Add timer to when is grounded jump right away.
            jumpBufferTimer = jumpBuffer;
        }

        //Jump extra momentum.
        if(wasGroundedLastFrame && !isGrounded && rb.velocity.y < 0) {
            jumpMomentumTimer = jumpExtraMomentum;
        }

        if(jumpMomentumTimer > 0 && jumpBufferTimer > 0) {
            Jump();
        } else {
            jumpMomentumTimer -= Time.deltaTime;
        }

        //Jump buffer, initiate timer when request jump, to jump ASAP when grounded.
        if(jumpBufferTimer > 0) {
            if(isGrounded) {
               Jump();
               jumpBufferTimer = 0;
            } else {
               jumpBufferTimer -= Time.deltaTime;
            }
        }

        //Jump cancel
        if(rb.velocity.y > minJumpForce && Input.GetButtonUp("Jump")) {
            rb.velocity = new Vector2(rb.velocity.x, minJumpForce);
        }

        //Normal movement <- ->
        rb.velocity = new Vector2(moveSpeed * moveInput.x, rb.velocity.y);

        //Functions
        void Jump() {
            rb.velocity = new Vector2(rb.velocity.x, maxJumpForce);
        }
    }

    void HandleAnimation() {
        if(!wasGroundedLastFrame && isGrounded && velocity.y <= 0) footImpactEffect.Play();

        if(isGrounded && Mathf.Abs(moveSpeed * moveInput.x) != 0 && velocity.y == 0) {
            if(!footDustEffect.isEmitting) footDustEffect.Play();
        }

        if( (facingDirection == 1 && moveInput.x < 0) || (facingDirection == -1 && moveInput.x > 0) ) {
            facingDirection = -facingDirection;
        }
        transform.localScale = new Vector3(facingDirection, transform.localScale.y, transform.localScale.z);

        animator.SetFloat("velocity.x", Mathf.Abs(moveSpeed * moveInput.x));
        animator.SetFloat("velocity.y", velocity.y);
        animator.SetBool("isGrounded", isGrounded);
    }
}
