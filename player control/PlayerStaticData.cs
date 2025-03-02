using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Functions/Game Stats/Player Static Data")]
public class PlayerStaticData : ScriptableObject {
    public float _runSpeedMult; //Speed multiplier when player running
    public float _atkSpeedMult; //Speed multiplier when player attacking
    public float _jumpSpeed; //How high does the player jump
    public float _gravity; //Gravity applied to the player
    public float _groundedGravity; //Gravity applied to the player when on the ground

    public float _mass; // Defines the character mass
    public float _maxDashTime; //Time that the player is dashing
    public float _dashSpeed; //Dash speed of the player
    public float _mpRegenFreq = 1f;
    public float _iniAtkSpeed = 2f; // the initial attack speed
}
    