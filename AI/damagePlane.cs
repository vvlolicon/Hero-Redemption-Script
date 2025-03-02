using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damagePlane : MonoBehaviour
{
    public float DmgFreq = 3f;
    public GeneralStatsObj _statsObj;
    float _dmgTimer = 0f;
    bool _canHurtPlayer = true;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && _canHurtPlayer)
        {
            _canHurtPlayer = false;
            _dmgTimer = DmgFreq;
            PlayerDmgInfo dmgInfo = new PlayerDmgInfo(_statsObj.ATK, _statsObj.CritChance, _statsObj.CritDmgMult, other.transform.position - transform.position, 0f);
            dmgInfo.CallDamageable(other.gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (_dmgTimer > 0 && !_canHurtPlayer)
        {
            _dmgTimer -= Time.deltaTime;
        }
        else
        {
            _canHurtPlayer = true;
        }
    }
}
