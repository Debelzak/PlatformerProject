using UnityEngine;

public static class IdleState
{
    public static void OnEnterState(Player player)
    {
        player.animator.Play("Idle");
    }

    public static void OnUpdate(Player player)
    {
        if(Input.GetAxisRaw("Horizontal") != 0)
        {
            player.State.Set(player, (int)PlayerState.Type.STATE_WALK);
        }

        if(Input.GetButtonDown("Jump") && Input.GetAxisRaw("Vertical") != -1)   //Add timer to when is grounded jump right away.
        {
            player.State.Set(player, (int)PlayerState.Type.STATE_JUMP);
        }

        if(!player.isGrounded && player.physics.velocity.y < 0)
        {
            player.State.Set(player, (int)PlayerState.Type.STATE_FALL);
        }
    }

    public static void OnFixedUpdate(Player player)
    {
        
    }
}
