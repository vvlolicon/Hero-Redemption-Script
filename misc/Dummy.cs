using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour, IDamageable
{
    public Transform damageTextPos;

    private bool isInvincible = false;
    private Animator anim;
    private AudioSource audioS;
    public GeneralStatsObj testStats;

    [Header("Manager")] public HealthManager healthManager;


    void Awake()
    {
        anim = GetComponent<Animator>();
        audioS = GetComponent<AudioSource>();
    }

    public void ApplyDamage(DmgInfo data)
    {
        if (!isInvincible && data is EnemyDmgInfo)
        {
            EnemyDmgInfo info = (EnemyDmgInfo)data;
            if (info.Target == gameObject)
            {
                Debug.Log("triggered ApplyDmg to dummy");
                anim.Play("Hitted");
                audioS.Play();
                DmgResult result = HealthManager.calculateDamage(
                    info.ATK, testStats.DEF, info.CritChance, testStats.CritChanRdc, 
                    testStats.DmgReduction, info.CritMult, testStats.CritDmgResis);
                int dmgShow = (int)result.Dmg;
                healthManager.createHealthMeg(new EnemyDmgInfo(dmgShow, result.IsCritHit, info.TextColor, damageTextPos, gameObject));
                StartCoroutine("MakeInvincible");
            }
        }
    }

    IEnumerator MakeInvincible()
    {
        isInvincible = true;
        yield return new WaitForSeconds(.5f);
        isInvincible = false;
    }

    //private void OnEnable()
    //{
    //    healthManager.healthChangeEvent.AddListener(ApplyDmg);
    //}
    //private void OnDisable()
    //{
    //    healthManager.healthChangeEvent.RemoveListener(ApplyDmg);
    //}
}
