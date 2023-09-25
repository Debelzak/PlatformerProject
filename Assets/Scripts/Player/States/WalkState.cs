using UnityEngine;

public class WalkState
{
    public static void OnEnterState(Player player)
    {
        player.animator.Play("Walk");
    }

    public static void OnUpdate(Player player)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        //Animation
        if(!player.footDustEffect.isEmitting)
        {
            player.footDustEffect.Play();
        }

        //Handler
        if(horizontalInput == 0)
        {
            player.physics.velocity = new Vector2(0, player.physics.velocity.y);
            player.State.Set(player, (int)PlayerState.Type.STATE_IDLE);
        }

        if(Input.GetButtonDown("Jump") && Input.GetAxisRaw("Vertical") != -1)
        {
            player.State.Set(player, (int)PlayerState.Type.STATE_JUMP);
        }

        if(player.physics.velocity.y < 0f && !player.isGrounded)
        {
            player.State.Set(player, (int)PlayerState.Type.STATE_FALL);
        }
    }

    public static void OnFixedUpdate(Player player)
    {
        
    }
}
