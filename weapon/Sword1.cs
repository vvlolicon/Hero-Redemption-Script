using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sword1 : MonoBehaviour
{

    public Color _dmgColor = Color.cyan; //Color of the text with the damage value

    private BoxCollider _coll; //Collider of the weapon
    //private HealthManager healthManager;
    private GameObject _player;
    private PlayerStateExecutor _executor;
    private GeneralStatsObj _playerStats;
    List<Collider> attackedEnemy = new List<Collider>();

    void Awake()
    {
        _coll = GetComponent<BoxCollider>();
    }

    private void OnEnable()
    {
        if (_player == null)
            _player = GameObject.FindWithTag("Player");
        _executor = _player.GetComponent<PlayerStateExecutor>();
        _playerStats = _executor.PlayerStats;
    }

	private void OnTriggerEnter(Collider other)
	{
        if (other.tag == "Enemy" && _executor.CurState.CurStateType() == PlayerStates.ATTACK)
        {
            if (!attackedEnemy.Contains(other)) { 
                attackedEnemy.Add(other);
                EnemyDmgInfo dmgInfo = new EnemyDmgInfo(_playerStats.ATK, _playerStats.CritChance, _playerStats.CritDmgMult, _dmgColor, transform, other.gameObject);
                dmgInfo.CallDamageable(other.gameObject);
            }
        }
	}

    public void EnableColliders() //Called from the AnimatorEvent script
    {
        _coll.enabled = true;
    }

    public void FinishAttack() //Called from the AnimatorEvent script
    {
        _coll.enabled = false;
        attackedEnemy.Clear();
    }
}
