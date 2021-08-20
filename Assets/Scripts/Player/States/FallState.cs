using UnityEngine;

namespace Deb.PlayerState
{
    public class FallState : State
    {
        private float horizontalInput;

        public override void OnEnterState(Player player)
        {
            player.animator.Play("Fall");
        }

        public override void Update(Player player)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");

            if(player.isGrounded && player.velocity.y == 0)
            {
                player.SetState(player.idleState);
            }

            //Animation
            if(!player.wasGroundedLastFrame && player.isGrounded)
            {
                player.footImpactEffect.Play();
            }
        }

        public override void FixedUpdate(Player player)
        {
            //Apply horizontal movement
            player.rigidBody.velocity = new Vector2(player.moveSpeed * horizontalInput, player.rigidBody.velocity.y);
        }
    }
}