using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEnemy : MonoBehaviour
{
    public Color dmgColor = Color.cyan; //Color of the text with the damage value

    private BoxCollider coll; //Collider of the weapon
    public GameObject owner;
    private EnemyStateExecutor _executor;

    //[Header("Events")] public GameEventOnAction OnHealthChanged;

    void Awake()
    {
        coll = GetComponent<BoxCollider>();
        _executor = owner.GetComponent<EnemyStateExecutor>();
    }

	private void OnTriggerEnter(Collider other)
	{
        if (other.tag == "Player")
        {
            PlayerDmgInfo dmgInfo = new PlayerDmgInfo(_executor.CombatStats.ATK, other.transform.position - transform.position, 250f);
            dmgInfo.CallDamageable(other.gameObject);
            // ensure only cause damage once;
            _executor.AnimatorEvents.EndAttack();
        }
	}

    public void EnableColliders() //Called from the AnimatorEvent script
    {
        coll.enabled = true;
    }

    public void DisableColliders() //Called from the AnimatorEvent script
    {
        coll.enabled = false;
    }
}
