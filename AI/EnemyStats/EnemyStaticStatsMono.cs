using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStaticStatsMono : MonoBehaviour
{
    public EnemyStaticStats _enemyStaticStats;
    public List<GameObject> _patrolPoints = new List<GameObject>();
    public Transform _damageTextPos;   

    public float ChaseTime { get { return _enemyStaticStats._chaseTime; } }
    public float VisDist { get { return _enemyStaticStats._visDist; } }
    public float VisAngle { get { return _enemyStaticStats._visAngle; } }
    public float AttackDist { get { return _enemyStaticStats._attackDist; } }
    public float AtkAnimTime { get { return _enemyStaticStats._atkAnimTime; } }
    public List<GameObject> PatrolPoints { get { return _patrolPoints; } }
    public Transform DmgPos { get { return _damageTextPos; } }
}
