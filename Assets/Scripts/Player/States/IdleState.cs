using UnityEngine;

namespace Deb.PlayerState
{
    public class IdleState : State
    {
        public override void OnEnterState(Player player)
        {
            player.animator.Play("Idle");
        }

        public override void Update(Player player)
        {
            if(Input.GetAxisRaw("Horizontal") != 0)
            {
                player.SetState(player.walkState);
            }

            if(Input.GetButtonDown("Jump") && Input.GetAxisRaw("Vertical") != -1)   //Add timer to when is grounded jump right away.
            {
                player.SetState(player.jumpState);
            }

            if(player.velocity.y < -2.0f) {
                player.SetState(player.fallState);
            }
        }

        public override void FixedUpdate(Player player)
        {
            
        }
    }
}