using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Static Data")]
public class EnemyStaticData: ScriptableObject
{
    [Tooltip("Distance of enemy spot the player and it will chase to attack")]
    public float _visDist = 10.0f;
    [Tooltip("Angle of enemy 'sees' player"), Range(0, 360)]
    public float _visAngle = 90.0f;
    [Tooltip("Distance from which the enemy will attack the player")]
    public float _attackDist = 1.5f;
    [Tooltip("Time of enemy from start playing attack animation to the pose that actually hits player")]
    public float _preAtkTime = 0.6f;
    [Tooltip("Time of enemy attack animation length")]
    public float _atkAnimTime = 1.2f;
    [Tooltip("Maximun time enemy will chase the player(if the monster is attack, it will ignore this)")]
    public float _chaseTime = 3.0f;
    [Tooltip("Time of enemy Hit animation plays( also the time that enemy will stand and do nothing when hit)")]
    public float _hitAnimTime = 2.3f;
    [Tooltip("If the enemy can be interrupted by other player's attack")]
    public bool Interruptable = true;
    public float _dieTime = 2.0f;
    public AttackStrategy _AtkStrategy;
}
public enum AttackStrategy { Melee, Weapon, Range, Detonate }
