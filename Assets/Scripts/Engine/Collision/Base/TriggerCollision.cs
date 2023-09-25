using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCollision : CollidableBase
{
    public TriggerCollision()
    {
        collisionType = CollisionType.COLLISION_TYPE_TRIGGER;
    }

    public override void CollisionHandle(Actor actor, ref float moveX, ref float moveY, RaycastHit2D hit)
    {
        print("TODO: TRIGGER COLLISION");
    }
}
