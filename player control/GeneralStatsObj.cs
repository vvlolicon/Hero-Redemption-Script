using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Functions/Game Stats/General Stats")]
public class GeneralStatsObj : ScriptableObject
{
    public float MaxHP;
    public float HP;
    public float MaxMP;
    public float MP;
    public float MP_Regen;
    public float ATK;
    public float SPEED;
    public float DEF;
    public float AttackTime;
    public float CritChance = 0;
    public float CritChanRdc = 0;
    public float CritDmgMult;
    public float CritDmgResis;
    public float DmgReduction;

    private void OnEnable()
    {
        HP = MaxHP;
        MP = MaxMP;
    }

    public void setStats(GeneralStatsObj stats)
    {
        MaxHP = stats.MaxHP;
        HP = MaxHP;
        MaxMP = stats.MaxMP;
        MP = MaxMP;
        ATK = stats.ATK;
        DEF = stats.DEF;
        SPEED = stats.SPEED;
        AttackTime = stats.AttackTime;
        CritChance = stats.CritChance;
        CritChanRdc = stats.CritChanRdc;
        CritDmgMult = stats.CritDmgMult;
        CritDmgResis = stats.CritDmgResis;
        DmgReduction = stats.DmgReduction;
    }
}
