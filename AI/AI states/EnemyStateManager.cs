using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIStates { GROUND, IDLE, PATROL, CHASE, ATTACK, HIT }
public class EnemyStateManager
{
    Dictionary<AIStates, EnemyBaseStates> _states = new Dictionary<AIStates, EnemyBaseStates>();
    EnemyStateExecutor _curExecutor;
    
    public EnemyStateManager(EnemyStateExecutor executor)
    {
        _curExecutor = executor;
        _states[AIStates.GROUND] = new EnemyStateGrounded(this, executor);
        _states[AIStates.IDLE] = new EnemyStateIdle(this, executor);
        _states[AIStates.PATROL] = new EnemyStatePatrol(this, executor);
        _states[AIStates.CHASE] = new EnemyStateChase(this, executor);
        _states[AIStates.ATTACK] = new EnemyStateAttack(this, executor);
        _states[AIStates.HIT] = new EnemyStateHit(this, executor);
    }

    public EnemyBaseStates Ground() { return _states[AIStates.GROUND]; }
    public EnemyBaseStates Idle() {  return _states[AIStates.IDLE]; }
    public EnemyBaseStates Patrol() { return _states[AIStates.PATROL]; }
    public EnemyBaseStates Chase() { return _states[AIStates.CHASE]; }
    public EnemyBaseStates Attack() { return _states[AIStates.ATTACK]; }
    public EnemyBaseStates Hit() { return _states[AIStates.HIT]; }
}
