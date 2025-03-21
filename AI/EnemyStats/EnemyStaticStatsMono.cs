using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStaticStatsMono : MonoBehaviour
{
    public EnemyStaticData _stats;
    public List<Transform> _patrolPoints = new List<Transform>();
    public Transform _damageTextPos;   
    public List<Transform> PatrolPoints { get { return _patrolPoints; } }
    public Transform DmgPos { get { return _damageTextPos; } }
}
