using UnityEngine;

namespace Deb.Player.States
{
    public class JumpState : State
    {
        private float horizontalInput;

        public override void OnEnterState(Player player)
        {
            player.animator.Play("Jump");
            player.rigidBody.velocity = new Vector2(player.rigidBody.velocity.x, player.maxJumpForce);
        }

        public override void Update(Player player)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");

            if(player.isGrounded && player.velocity.y == 0)
            {
                player.SetState(player.idleState);
            }

            if(player.rigidBody.velocity.y < 0f && !player.isGrounded)
            {
                player.SetState(player.fallState);
            }

            //Jump cancel
            if(player.rigidBody.velocity.y > player.minJumpForce && Input.GetButtonUp("Jump"))
            {
                player.rigidBody.velocity = new Vector2(player.rigidBody.velocity.x, player.minJumpForce);
            }
        }

        public override void FixedUpdate(Player player)
        {
            //Apply horizontal movement
            player.rigidBody.velocity = new Vector2(player.moveSpeed * horizontalInput, player.rigidBody.velocity.y);
        }
    }
}