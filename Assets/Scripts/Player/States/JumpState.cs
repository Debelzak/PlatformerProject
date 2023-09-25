using UnityEngine;

public static class JumpState
{
    public static void OnEnterState(Player player)
    {
        player.animator.Play("Jump");
        player.physics.velocity = new Vector2(player.physics.velocity.x, player.maxJumpForce);
    }

    public static void OnUpdate(Player player)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if(player.physics.velocity.y > player.minJumpForce && Input.GetButtonUp("Jump"))
        {
            player.physics.velocity.y = player.minJumpForce;
        }

        if(player.isGrounded && player.physics.velocity.y == 0)
        {
            player.State.Set(player, (int)PlayerState.Type.STATE_IDLE);
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
