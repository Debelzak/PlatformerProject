using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SolidObject : CollidableBase
{
    // Public
    public Vector2 velocity;
    public List<Actor> Pushing {get; private set;}

    // Components
    protected Rigidbody2D rb;

    public SolidObject()
    {
        collisionType = CollisionType.COLLISION_TYPE_SOLID_OBJECT;
    }

    // Unity functions
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;

        Pushing = new List<Actor>();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Move(velocity.x * Time.fixedDeltaTime, velocity.y * Time.fixedDeltaTime);
    }

    // Custom functions
    public void Move(float x, float y)
    {
        MovePassengers(x, y);

        rb.position = new Vector3(rb.position.x + x, rb.position.y + y, transform.position.z);
    }
    public void AddPassenger(Actor actor)
    {
        if(!Pushing.Contains(actor))
        {
            Pushing.Add(actor);
        }
    }
    private void RemovePassenger(Actor actor)
    {
        if(Pushing.Contains(actor))
        {
            Pushing.Remove(actor);
        }
    }
    private void RemoveInvalidPassengers()
    {
        List<Actor> removeBuffer = new List<Actor>();

        if(Pushing.Count > 0)
        {
            foreach(Actor actor in Pushing)
            {
                if(!IsOnboard(actor))
                {
                    removeBuffer.Add(actor);
                }
            }

            foreach(Actor actor in removeBuffer)
            {
                actor.BeignPushed = null;
                RemovePassenger(actor);
            }
        }
    }
    private void MovePassengers(float x, float y)
    {
        RemoveInvalidPassengers();
        foreach(Actor actor in Pushing)
        {
            actor.StepHorizontal(velocity.x * Time.fixedDeltaTime);
            actor.StepVertical(velocity.y * Time.fixedDeltaTime);
        }
    }
    private bool IsOnboard(Actor actor)
    {
        Bounds actorBounds = actor.col.bounds;
        if(actorBounds.min.y - 0.001f <= col.bounds.max.y &&
           actorBounds.max.x >= col.bounds.min.x &&
           actorBounds.min.x <= col.bounds.max.x &&
           actorBounds.min.y >= col.bounds.max.y - 0.001f)
        {
            return true;
        }

        return false;
    }
    public override void CollisionHandle(Actor actor, ref float moveX, ref float moveY, RaycastHit2D hit) 
    {
        actor.BeignPushed = this;
        AddPassenger(actor);
        base.CollisionHandle(actor, ref moveX, ref moveY, hit);
    }
}
