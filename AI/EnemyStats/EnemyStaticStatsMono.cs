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
        // ��ȡѲ�ߵ�
        if (_patrolPoints == null)
        {
            // ���Ѳ�ߵ�Ϊ�գ�����һ������Ѳ�ߵ㣬��ʼλ��ΪAI����λ��
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
            // ���ֻ��һ��Ѳ�ߵ㣬ֱ��ʹ�øõ�
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
