using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidTile : CollidableBase
{
    public SolidTile()
    {
        collisionType = CollisionType.COLLISION_TYPE_SOLID_TILE;
    }

    public override void CollisionHandle(Actor actor, ref float moveX, ref float moveY, RaycastHit2D hit)
    {
        base.CollisionHandle(actor, ref moveX, ref moveY, hit);
    }
}
