using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEnemy : MonoBehaviour
{
    //public int attack = 132; //Damage of the weapon
    //public int random_range = 10;
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
            PlayerDmgInfo dmgInfo = new PlayerDmgInfo(_executor.ATK, other.transform.position - transform.position, 250f);
            dmgInfo.CallDamageable(other.gameObject);
            //float dmgValue = attack + Random.Range(-1 * random_range, random_range);
            //PlayerDmgInfo dmgInfo = new PlayerDmgInfo(dmgValue, other.transform.position - transform.position, 250f);
            //OnHealthChanged.Raise(dmgInfo);
            //dmgInfo.CallDamageable(other.gameObject);
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
