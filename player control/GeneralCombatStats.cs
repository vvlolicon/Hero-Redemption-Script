using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GeneralCombatStats
{
    public Dictionary<CombatStatsType, float> CombatStats = new();

    public GeneralCombatStats()
    {
        for (int i = 0; i < typeof(CombatStatsType).GetEnumCount(); i++)
        {
            CombatStatsType type = (CombatStatsType)i;
            CombatStats[type] = 0;
        }
    }

    public GeneralCombatStats(GeneralStatsObj stats)
    {
        SetStats(stats);
    }

    public GeneralCombatStats(GeneralCombatStats stats)
    {
        SetStats(stats);
    }

    public void SetStats(GeneralCombatStats stats)
    {
        if (stats == null || stats == this) {
            Debug.Log("Cannot set stats to null or self");
            return;
        }

        for(int i = 0; i< typeof(CombatStatsType).GetEnumCount(); i++)
        {
            CombatStatsType type = (CombatStatsType)i;
            if(CombatStats.ContainsKey(type) && stats.CombatStats.ContainsKey(type))
            {
                CombatStats[type] = stats.GetStats(type);
            }
        }
        foreach (var kvp in stats.CombatStats)
        {
            CombatStats[kvp.Key] = kvp.Value;
        }
    }

    public void SetStats(Dictionary<CombatStatsType, float> stats)
    {
        if (stats == null)
        {
            Debug.Log("Cannot set stats to null");
            return;
        }

        for (int i = 0; i < typeof(CombatStatsType).GetEnumCount(); i++)
        {
            CombatStatsType type = (CombatStatsType)i;
            if (CombatStats.ContainsKey(type) && stats.ContainsKey(type))
            {
                CombatStats[type] = stats[type];
            }
        }
    }

    public void SetStats(GeneralStatsObj stats)
    {
        CombatStats[CombatStatsType.MaxHP] = stats.MaxHP;
        CombatStats[CombatStatsType.HP] = stats.HP;
        CombatStats[CombatStatsType.MaxMP] = stats.MaxMP;
        CombatStats[CombatStatsType.MP] = stats.MP;
        CombatStats[CombatStatsType.MP_Regen] = stats.MP_Regen;
        CombatStats[CombatStatsType.ATK] = stats.ATK;
        CombatStats[CombatStatsType.SPEED] = stats.SPEED;
        CombatStats[CombatStatsType.DEF] = stats.DEF;
        CombatStats[CombatStatsType.AttackTime] = stats.AttackTime;
        CombatStats[CombatStatsType.CritChance] = stats.CritChance;
        CombatStats[CombatStatsType.CritChanRdc] = stats.CritChanRdc;
        CombatStats[CombatStatsType.CritDmgMult] = stats.CritDmgMult;
        CombatStats[CombatStatsType.CritDmgResis] = stats.CritDmgResis;
        CombatStats[CombatStatsType.DmgReduce] = stats.DmgReduce;
    }

    public Dictionary<CombatStatsType, float> GetAllStats() => CombatStats;
    public float GetStats(CombatStatsType type) => CombatStats[type];
    public void SetStats(CombatStatsType type, float value) => CombatStats[type] = value;
    public void ChangeStats(CombatStatsType type, float value) => CombatStats[type] += value;

    public float Speed
    {
        get { return GetStats(CombatStatsType.SPEED); } // the actual movement
        set { SetStats(CombatStatsType.SPEED, value); }
    }
    public float ATK
    {
        get { return GetStats(CombatStatsType.ATK); }
        set { SetStats(CombatStatsType.ATK, value); }
    }
    public float HP
    {
        get { return GetStats(CombatStatsType.HP); }
        set { SetStats(CombatStatsType.HP, value); }
    }
    public float MaxHP
    {
        get { return GetStats(CombatStatsType.MaxHP); }
        set { SetStats(CombatStatsType.MaxHP, value); }
    }
    public float MP
    {
        get { return GetStats(CombatStatsType.MP); }
        set { SetStats(CombatStatsType.MP, value); }
    }
    public float MaxMP
    {
        get { return GetStats(CombatStatsType.MaxMP); }
        set { SetStats(CombatStatsType.MaxMP, value); }
    }
    public float MP_Regen
    {
        get { return GetStats(CombatStatsType.MP_Regen); }
        set { SetStats(CombatStatsType.MP_Regen, value); }
    }
    public float DEF
    {
        get { return GetStats(CombatStatsType.DEF); }
        set { SetStats(CombatStatsType.DEF, value); }
    }
    public float DmgReduce
    {
        get { return GetStats(CombatStatsType.DmgReduce); }
        set { SetStats(CombatStatsType.DmgReduce, value); }
    }
    public float CritChance
    {
        get { return GetStats(CombatStatsType.CritChance); }
        set { SetStats(CombatStatsType.CritChance, value); }
    }
    public float CritResis
    {
        get { return GetStats(CombatStatsType.CritDmgResis); }
        set { SetStats(CombatStatsType.CritDmgResis, value); }
    }
    public float CritDmgMult
    {
        get { return GetStats(CombatStatsType.CritDmgMult); }
        set { SetStats(CombatStatsType.CritDmgMult, value); }
    }
    public float CritChanRdc
    {
        get { return GetStats(CombatStatsType.CritChanRdc); }
        set { SetStats(CombatStatsType.CritChanRdc, value); }
    }
    public float CritDmgResis
    {
        get { return GetStats(CombatStatsType.CritDmgResis); }
        set { SetStats(CombatStatsType.CritDmgResis, value); }
    }
    public float AttackTime
    {
        get { return GetStats(CombatStatsType.AttackTime); }
        set { SetStats(CombatStatsType.AttackTime, value); }
    }
}

public enum CombatStatsType
{
    MaxHP, HP, MaxMP, MP, MP_Regen, ATK, SPEED, DEF, AttackTime, CritChance,
    CritChanRdc, CritDmgMult, CritDmgResis, DmgReduce
}

[System.Serializable]
public class CombatStatAttributes
{
    public CombatStatsType AtbrType;
    public float Value;
    public CombatStatAttributes(CombatStatsType attributeType, float attributeValue)
    {
        this.AtbrType = attributeType;
        this.Value = attributeValue;
    }
}
