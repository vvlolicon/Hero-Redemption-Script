using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Functions/Game Stats/General Stats")]
public class GeneralStatsObj : ScriptableObject
{
    public float maxHP;
    public float HP;
    public float maxMP;
    public float MP;
    public float ATK;
    public float SPEED;
    public float DEF;
    public float AttackTime;
    public float CritChance = 0;
    public float CritMult;
    public float CritResis;
    public float DmgReduction;

    private void OnEnable()
    {
        HP = maxHP;
        MP = maxMP;
    }

    public void setStats(GeneralStatsObj stats)
    {
        maxHP = stats.maxHP;
        HP = maxHP;
        maxMP = stats.maxMP;
        MP = maxMP;
        ATK = stats.ATK;
        DEF = stats.DEF;
        SPEED = stats.SPEED;
        AttackTime = stats.AttackTime;
        CritChance = stats.CritChance;
        CritMult = stats.CritMult;
        CritResis = stats.CritResis;
        DmgReduction = stats.DmgReduction;
    }
}
