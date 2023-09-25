using System;
using UnityEngine;

public class PlayerState : StateMachine<Player>
{
    public enum Type
    {
        STATE_IDLE,
        STATE_WALK,
        STATE_FALL,
        STATE_JUMP,
    }

    private Type currentState;

    public override void OnUpdate(Player player)
    {
        switch (currentState)
        {
            case Type.STATE_IDLE: IdleState.OnUpdate(player); break;
            case Type.STATE_WALK: WalkState.OnUpdate(player); break;
            case Type.STATE_FALL: FallState.OnUpdate(player); break;
            case Type.STATE_JUMP: JumpState.OnUpdate(player); break;
        }
    }
    public override void OnFixedUpdate(Player player)
    {
        switch (currentState)
        {
            case Type.STATE_IDLE: IdleState.OnFixedUpdate(player); break;
            case Type.STATE_WALK: WalkState.OnFixedUpdate(player); break;
            case Type.STATE_FALL: FallState.OnFixedUpdate(player); break;
            case Type.STATE_JUMP: JumpState.OnFixedUpdate(player); break;
        }
    }
    public override void Set(Player player, int newState)
    {
        currentState = (Type)newState;

        switch (currentState)
        {
            case Type.STATE_IDLE: IdleState.OnEnterState(player); break;
            case Type.STATE_WALK: WalkState.OnEnterState(player); break;
            case Type.STATE_FALL: FallState.OnEnterState(player); break;
            case Type.STATE_JUMP: JumpState.OnEnterState(player); break;
        }
    }
}