using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Functions/Game Stats/Enemy Static Data")]
public class EnemyStaticData: ScriptableObject
{
    public float _visDist = 10.0f; //Distance of vision
    public float _visAngle = 90.0f; //Angle of the cone vision
    public float _attackDist = 1.5f; //Distance from which the enemy will attack the player
    public float _chaseTime = 3.0f;
    public float _atkAnimTime = 1.2f; //time of playing attack animation
}
