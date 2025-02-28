using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStates { IDLE, GROUNDED, WALK, JUMP, FALL, ATTACK, HIT }

public class PlayerStatsManager
{
    Dictionary<PlayerStates, PlayerBaseState> _states = new Dictionary<PlayerStates, PlayerBaseState>();
    PlayerStateExecutor _curExecutor;
    public PlayerStatsManager(PlayerStateExecutor executor)
    {
        _curExecutor = executor;
        _states[PlayerStates.IDLE] = new PlayerIdleState(this, _curExecutor);
        _states[PlayerStates.GROUNDED] = new PlayerGroundedState(this, _curExecutor);
        _states[PlayerStates.WALK] = new PlayerWalkState(this, _curExecutor);
        _states[PlayerStates.JUMP] = new PlayerJumpState(this, _curExecutor);
        _states[PlayerStates.FALL] = new PlayerFallingState(this, _curExecutor);
        _states[PlayerStates.ATTACK] = new PlayerAttackState(this, _curExecutor);
        _states[PlayerStates.HIT] = new PlayerHitState(this, _curExecutor);
    }

    public PlayerBaseState Idle()
    {
        return _states[PlayerStates.IDLE];
    }
    public PlayerBaseState Grounded()
    {
        return _states[PlayerStates.GROUNDED];
    }
    public PlayerBaseState Walk()
    {
        return _states[PlayerStates.WALK];
    }
    public PlayerBaseState Run()
    {
        return _states[PlayerStates.WALK];
    }
    public PlayerBaseState Jump()
    {
        return _states[PlayerStates.JUMP];
    }
    public PlayerBaseState Fall()
    {
        return _states[PlayerStates.FALL];
    }
    public PlayerBaseState Attack()
    {
        return _states[PlayerStates.ATTACK];
    }
    public PlayerBaseState Hit()
    {
        return _states[PlayerStates.HIT];
    }

}
