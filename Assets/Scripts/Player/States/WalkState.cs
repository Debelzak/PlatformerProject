using UnityEngine;

namespace Deb.PlayerState
{
    public class WalkState : State 
    {
        private float horizontalInput;
        private float verticalInput;

        public override void OnEnterState(Player player)
        {
            player.animator.Play("Walk");
        }

        public override void Update(Player player)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");

            //Animation
            if(!player.footDustEffect.isEmitting)
            {
                player.footDustEffect.Play();
            }

            //Handler
            if(horizontalInput == 0)
            {
                player.rigidBody.velocity = new Vector2(0, player.rigidBody.velocity.y);
                player.SetState(player.idleState);
            }

            if(Input.GetButtonDown("Jump") && Input.GetAxisRaw("Vertical") != -1)
            {
                player.SetState(player.jumpState);
            }

            if(player.velocity.y < -2.0f && !player.isGrounded)
            {
                player.SetState(player.fallState);
            }
        }

        public override void FixedUpdate(Player player)
        {
            //Apply horizontal movement
            player.rigidBody.velocity = new Vector2(player.moveSpeed * horizontalInput, player.rigidBody.velocity.y);
        }
    }
}