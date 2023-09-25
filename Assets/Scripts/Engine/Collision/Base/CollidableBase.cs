using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class CollidableBase : MonoBehaviour
{
    public enum CollisionType
    {
        COLLISION_TYPE_SOLID_TILE,
        COLLISION_TYPE_SOLID_OBJECT,
        COLLISION_TYPE_ACTOR,
        COLLISION_TYPE_TRIGGER,
    }

    // Properties
    public bool isOneWayCollidable;

    public CollisionType collisionType {get; protected set;}

    // Components
    public Collider2D col {get; private set;}

    protected virtual void Start()
    {
        col = GetComponent<Collider2D>();
    }

    protected virtual void FixedUpdate()
    {
        
    }

    // Resolve as colisões por padrão
    public virtual void CollisionHandle(Actor actor, ref float moveX, ref float moveY, RaycastHit2D hit)
    {
        actor.collisionStatus.OnOneWayPlatform = isOneWayCollidable;

        if(moveX != 0)
        {
            // Collisions left
            if(hit.point.x < actor.col.bounds.center.x)
            {
                actor.collisionStatus.CollisionLeft = true;
                actor.velocity.x = 0;
                moveX = 0;
                
                float newPositionX = hit.point.x + actor.col.bounds.size.x/2 - actor.col.offset.x;

                actor.rb.position = new Vector2(newPositionX, actor.rb.position.y);
            }

            // Collisions right
            if(hit.point.x > actor.col.bounds.center.x)
            {
                actor.collisionStatus.CollisionRight = true;
                actor.velocity.x = 0;
                moveX = 0;
                
                float newPositionX = hit.point.x - actor.col.bounds.size.x/2 - actor.col.offset.x;

                actor.rb.position = new Vector2(newPositionX, actor.rb.position.y);
            }
        }

        if(moveY != 0)
        {
            float floorAngle = Vector2.Angle(hit.normal, Vector2.up);
            if(floorAngle > 0 && floorAngle < actor.maxClimbAngle)
            {
                actor.collisionStatus.OnSlope = true;
            }

            // Collisions below
            if(hit.point.y < actor.col.bounds.center.y)
            {
                actor.collisionStatus.CollisionBelow = true;
                actor.velocity.y = 0;
                moveY = 0;
                
                float newPositionY = hit.point.y + actor.col.bounds.size.y/2 - actor.col.offset.y;

                actor.rb.position = new Vector2(actor.rb.position.x, newPositionY);
            }

            // Collisions above
            if(hit.point.y > actor.col.bounds.center.y)
            {
                actor.collisionStatus.CollisionAbove = true;
                actor.velocity.y = 0;
                moveY = 0;

                float newPositionY = hit.point.y - actor.col.bounds.size.y/2 - actor.col.offset.y;
                
                actor.rb.position = new Vector2(actor.rb.position.x, newPositionY);
            }

        }
    }
}
