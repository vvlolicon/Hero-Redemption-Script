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

    private void Awake()
    {
        InitializePatrolPoints();
    }
    void InitializePatrolPoints()
    {
        // 获取巡逻点
        if (_patrolPoints == null)
        {
            // 如果巡逻点为空，创建一个虚拟巡逻点，初始位置为AI自身位置
            CreateNewPatrolPoint();
            return;
        }
        if (_patrolPoints.Count == 0)
        {
            CreateNewPatrolPoint();
            return;
        }
        if (_patrolPoints.Count == 1)
        {
            // 如果只有一个巡逻点，直接使用该点
            Debug.Log($"Only one patrol point detected for {gameObject.name}. Using it as the initial point.");
        }

        Debug.Log($"Initial patrol points count: {_patrolPoints.Count}");
        void CreateNewPatrolPoint()
        {
            Debug.Log("Creating new patrol point");
            GameObject virtualPoint = new GameObject($"SpawnPoint of {gameObject.name}");
            virtualPoint.transform.position = gameObject.transform.position;
            _patrolPoints = new List<Transform> { virtualPoint.transform };
        }
    }

}
