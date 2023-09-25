using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Actor : CollidableBase
{
    private class CollisionHit
    {
        public int rayId;
        public CollisionType collisionType;
        public RaycastHit2D hit;

        public CollisionHit(int rayId, RaycastHit2D hit, CollisionType collisionType)
        {
            this.rayId = rayId;
            this.hit = hit;
            this.collisionType = collisionType;
        }

        public static RaycastHit2D GetClosestHit(List<CollisionHit> hits)
        {
            RaycastHit2D closestHit = new();
            closestHit.distance = Mathf.Infinity;

            foreach (CollisionHit thisHit in hits)
            {
                if (thisHit.hit.distance < closestHit.distance) closestHit = thisHit.hit;
            }

            return closestHit;
        }
    }

    [Serializable]
    public struct CollisionStatus
    {
        // vertical related
        public bool CollisionAbove {get; set;}
        public bool CollisionBelow {get; set;}
        public bool OnSlope {get; set;}
        public bool OnOneWayPlatform {get; set;}

        // horizontal related
        public bool CollisionLeft {get; set;}
        public bool CollisionRight {get; set;}
    }

    // Configuration
    [Header("Gravity")]
    public bool gravityEnabled;
    public Vector2 gravityForce = new(0, -9.81f);
    public float gravityScale = 1f;
    public float terminalVelocity = -10f;
    public float maxClimbAngle = 75;

    [Header("Collision Masks")]
    public LayerMask collisionMask;

    // Public
    public Vector2 velocity;
    public Bounds rayCastBounds;
    public CollisionStatus collisionStatus;
    public CollisionStatus lastFrameCollisionStatus;
    public SolidObject BeignPushed {get; set;}

    // Components
    public Rigidbody2D rb {get; private set;}

    // Unity methods
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if(velocity.x != 0) StepHorizontal(velocity.x * Time.fixedDeltaTime);
        if(gravityEnabled) ApplyGravity();
        if(velocity.y != 0) StepVertical(velocity.y * Time.fixedDeltaTime);

        lastFrameCollisionStatus = collisionStatus;
    }

    // Custom methods
    private List<CollisionHit> RaycastHorizontal(float moveX)
    {
        float skinWidth = 0.001f; // Adiciona pequeno offset para que detecte colisões mesmo quando não estiver em movimento.
        int rayCount = 6; // Número de raycasts que serão disparados
        rayCastBounds = col.bounds;
        rayCastBounds.Expand(-skinWidth); // Adiciona micro margem dentro para que, por exemplo, o colisor horizontal não detecte o chão
        float raySpacing = rayCastBounds.size.y / (rayCount - 1); // Espaçamento entre os raycasts
        
        List<CollisionHit> hits = new();

        for(int i = 0; i < rayCount; i++)
        {
            Vector2 rayOrigin = new(rayCastBounds.center.x, rayCastBounds.min.y + raySpacing * i);
            Vector2 rayDirection = (moveX > 0) ? Vector2.right : Vector2.left; // Direção que o raycast será lançado
            float rayLength = col.bounds.size.x/2 + skinWidth + Mathf.Abs(moveX); // Tamanho do raycast

            Debug.DrawLine(rayOrigin, rayOrigin + rayLength * rayDirection, (rayDirection.x > 0) ? Color.yellow : Color.blue);

            RaycastHit2D[] thisRayHits = Physics2D.RaycastAll(rayOrigin, rayDirection, rayLength, collisionMask);

            foreach(RaycastHit2D hit in thisRayHits)
            {
                //Ignore self collision
                if(hit.transform.gameObject == this.gameObject) continue;

                float wallAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(Mathf.Round(wallAngle) < maxClimbAngle) continue;

                CollidableBase collidable = hit.transform.gameObject.GetComponent<CollidableBase>();
                if(collidable)
                {
                    if(collidable.isOneWayCollidable) continue;
                }

                CollisionHit thisHit = new(i, hit, collidable.collisionType);

                hits.Add(thisHit);
                Debug.DrawLine(rayOrigin, hit.point, Color.red);
            }

        }

        return hits;
    }

    public void StepHorizontal(float moveX)
    {
        // Reset horizontal collision status variables
        collisionStatus.CollisionLeft = collisionStatus.CollisionRight = false;

        float velocityYDummy = 0f;
        List<CollisionHit> hits;
        hits = RaycastHorizontal(moveX);

        if(BeignPushed && BeignPushed.velocity.x != 0)
        {
            List<CollisionHit> extraHits;
            extraHits = RaycastHorizontal(BeignPushed.velocity.x * Time.fixedDeltaTime);
            hits.AddRange(extraHits);
        }

        // Resolve
        if(hits.Count > 0)
        {
            RaycastHit2D hit = CollisionHit.GetClosestHit(hits);
            CollidableBase obj = hit.transform.gameObject.GetComponent<CollidableBase>();
            obj?.CollisionHandle(this, ref moveX, ref velocityYDummy, hit);
        }
        
        rb.position = new(rb.position.x + moveX, rb.position.y);
    }

    private List<CollisionHit> RaycastVertical(float moveY)
    {
        float skinWidth = 0.001f; // Adiciona pequeno offset para que detecte colisões mesmo quando não estiver em movimento.
        int rayCount = 3; // Número de raycasts que serão disparados
        rayCastBounds = col.bounds;
        rayCastBounds.Expand(-skinWidth); // Adiciona micro margem dentro para que, por exemplo, o colisor horizontal não detecte o chão
        float raySpacing = rayCastBounds.size.x / (rayCount - 1); // Espaçamento entre os raycasts
        
        List<CollisionHit> hits = new();

        for(int i = 0; i < rayCount; i++)
        {
            Vector2 rayOrigin = new(rayCastBounds.min.x + raySpacing * i, rayCastBounds.center.y);
            Vector2 rayDirection = (moveY > 0) ? Vector2.up : Vector2.down; // Direção que o raycast será lançado
            float rayLength = col.bounds.size.y/2 + skinWidth + Mathf.Abs(moveY); // Tamanho do raycast

            Debug.DrawLine(rayOrigin, rayOrigin + rayLength * rayDirection, (rayDirection.y > 0) ? Color.green : Color.magenta);

            if(lastFrameCollisionStatus.OnSlope && velocity.y <= 0)
            {
                rayLength += Mathf.Abs(velocity.x * Time.fixedDeltaTime);
            }

            RaycastHit2D[] thisRayHits = Physics2D.RaycastAll(rayOrigin, rayDirection, rayLength, collisionMask);

            foreach(RaycastHit2D hit in thisRayHits)
            {
                //Ignore self collision
                if(hit.transform.gameObject == this.gameObject) continue;

                CollidableBase collidable = hit.transform.gameObject.GetComponent<CollidableBase>();
                if(collidable)
                {
                    if(collidable.isOneWayCollidable)
                    {
                        if(rayCastBounds.min.y < hit.point.y) continue;
                    }
                }
                else
                {
                    continue;
                }

                CollisionHit thisHit = new(i, hit, collidable.collisionType);

                hits.Add(thisHit);
                Debug.DrawLine(rayOrigin, hit.point, Color.red);
            }
        }

        return hits;
    }

    public void StepVertical(float moveY)
    {
        // Reset vertical collision status variables
        collisionStatus.CollisionAbove = collisionStatus.CollisionBelow = collisionStatus.OnSlope = collisionStatus.OnOneWayPlatform = false;

        float velocityXDummy = 0f;
        List<CollisionHit> hits = new();
        hits = RaycastVertical(moveY);

        if(BeignPushed && BeignPushed.velocity.y != 0)
        {
            List<CollisionHit> extraHits = new();
            extraHits = RaycastVertical(BeignPushed.velocity.y * Time.fixedDeltaTime);
            hits.AddRange(extraHits);
        }

        // Resolve collisions
        if(hits.Count > 0)
        {
            hits = hits.OrderBy(item => item.hit.distance).ToList();    // Re-organiza baseado na distância
            //hits.Reverse(); // Reverte para que fique da maior para a menor ao invés da maior para a menor
            foreach(var thisHit in hits)
            {
                CollidableBase collidable = thisHit.hit.transform.gameObject.GetComponent<CollidableBase>();
                if(!collidable) continue;

                collidable.CollisionHandle(this, ref velocityXDummy, ref moveY, thisHit.hit);
            }
        }

        // Slope descend smoothly, acontece se jogador estava no chão no frame anterior e existe uma rampa logo abaixo
        // Isso vai ajustar a posição do jogador antes do frame ser processado
        if(!collisionStatus.CollisionBelow && lastFrameCollisionStatus.CollisionBelow && velocity.y <= 0)
        {
            HandleDownhillSlope(ref moveY);
        }

        rb.position = new(rb.position.x, rb.position.y + moveY);
    }

    private void HandleDownhillSlope(ref float moveY)
    {
        float skinWidth = 0.001f;
        float deltaMovementX = Mathf.Abs(velocity.x) * Time.fixedDeltaTime;
        float rayOriginX = (Mathf.Sign(velocity.x) > 0) ? col.bounds.min.x : col.bounds.max.x;
        float rayOriginY = col.bounds.min.y - moveY - skinWidth;
        if(velocity.x == 0 && collisionStatus.CollisionLeft) rayOriginX = col.bounds.max.x;
        if(velocity.x == 0 && collisionStatus.CollisionRight) rayOriginX = col.bounds.min.x; 
        Vector2 rayOrigin = new Vector2(rayOriginX, rayOriginY);
        Vector2 rayDirection = Vector2.down;
        float rayLength = (velocity.x == 0 && (collisionStatus.CollisionRight || collisionStatus.CollisionLeft)) ? float.PositiveInfinity : deltaMovementX;  // Se houver colisões na horizontal, o tamanho do raycast será infinito
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, collisionMask);

        Debug.DrawRay(rayOrigin, rayDirection);
        if(hit)
        {
            //Ignore self collision
            if(hit.transform.gameObject == this.gameObject) return;

            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if(slopeAngle == 0 || (deltaMovementX != 0 && hit.distance > deltaMovementX))
                return;
            
            float yCompensation = -hit.distance;
            collisionStatus.CollisionBelow = true;
            collisionStatus.OnSlope = true;
            moveY = yCompensation - skinWidth;
        }
    }

    private void ApplyGravity()
    {
        velocity += gravityForce * gravityScale * Time.fixedDeltaTime;

        if(velocity.y < terminalVelocity)
        {
            velocity.y = terminalVelocity;
        }
    }

    public void DropFromPlatform()
    {
        if(collisionStatus.OnOneWayPlatform)
        {
            rb.position -= new Vector2(0f, 0.1f);
            if(BeignPushed)
            {
                velocity.y += -Mathf.Abs(BeignPushed.velocity.y);
            }
        }
    }

}