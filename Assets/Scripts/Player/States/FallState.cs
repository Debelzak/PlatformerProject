using UnityEngine;

public class FallState
{
    public static void OnEnterState(Player player)
    {
        player.animator.Play("Fall");
    }

    public static void OnUpdate(Player player)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if(player.isGrounded)
        {
            player.State.Set(player, (int)PlayerState.Type.STATE_IDLE);
            if(horizontalInput != 0)
            {
                player.State.Set(player, (int)PlayerState.Type.STATE_WALK);
            }
        }

        //Animation
        if(!player.wasGroundedLastFrame && player.isGrounded)
        {
            player.footImpactEffect.Play();
        }
    }

    public static void OnFixedUpdate(Player player)
    {

    }
}
