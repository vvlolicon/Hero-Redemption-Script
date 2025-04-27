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
    public float DmgReduce;

    GeneralCombatStats _combatStats;

    public void InitializeStats()
    {
        HP = MaxHP;
        MP = MaxMP;
        if (_combatStats == null)
            _combatStats = new GeneralCombatStats(this);
        else
            _combatStats.SetStats(this);
    }

    public void SetStats(GeneralStatsObj stats)
    {
        MaxHP = stats.MaxHP;
        HP = MaxHP;
        MaxMP = stats.MaxMP;
        MP = MaxMP;
        MP_Regen = stats.MP_Regen;
        ATK = stats.ATK;
        DEF = stats.DEF;
        SPEED = stats.SPEED;
        AttackTime = stats.AttackTime;
        CritChance = stats.CritChance;
        CritChanRdc = stats.CritChanRdc;
        CritDmgMult = stats.CritDmgMult;
        CritDmgResis = stats.CritDmgResis;
        DmgReduce = stats.DmgReduce;
    }

    public GeneralCombatStats GetCombatStats() => _combatStats;

    public float GetStats(CombatStatsType type) => _combatStats.GetStats(type);
    public void SetStats(CombatStatsType type, float value) => _combatStats.SetStats(type, value);
    public void ChangeStats(CombatStatsType type, float value) => _combatStats.ChangeStats(type, value);
}
