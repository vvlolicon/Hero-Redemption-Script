using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Functions/Combat Buff")]
public class CombatBuff : ScriptableObject
{
    public BuffContext buffContext;

    public BuffStats CreateBuff() => new BuffStats(this);
}

[System.Serializable]
public class BuffContext
{
    public string buffName;
    public string buffDescription;
    public Sprite buffIcon;
    public List<CombatStatAttributes> affectStats;
    public float interval;
    public float duration;
    public int priority;
    public bool usePercentage = false;
    public bool stackable;
    public bool isNerf = false;

    public static BuffContext Clone(BuffContext other)
    {
        BuffContext newContext = new BuffContext();
        newContext.buffName = other.buffName;
        newContext.buffDescription = other.buffDescription;
        newContext.buffIcon = other.buffIcon;
        newContext.affectStats = new List<CombatStatAttributes>(other.affectStats);
        newContext.interval = other.interval;
        newContext.duration = other.duration;
        newContext.priority = other.priority;
        newContext.usePercentage = other.usePercentage;
        newContext.stackable = other.stackable;
        newContext.isNerf = other.isNerf;
        return newContext;
    }
}
public class BuffStats
{
    CombatBuff buffData;
    BuffContext buffContext;

    float _intervalTimer = 0;
    float _passedTime = 0;

    public BuffStats(CombatBuff buff)
    {
        buffContext = BuffContext.Clone(buff.buffContext);
        buffData = buff;
        //AffectOnTotalValue = buff.AffectOnTotalValue;
    }

    public bool CountDown(float deltaTime)
    {
        _intervalTimer += deltaTime;
        _passedTime += deltaTime;
        if (buffContext.interval == 0) return false;
        if (_intervalTimer >= buffContext.interval)
        {
            _intervalTimer = 0;
            return true;
        }
        return false;
    }

    public bool IsExpired() => _passedTime >= duration;
    public float GetPassedTime() => _passedTime;
    public void ResetPassedTime() => _passedTime = 0;

    public bool IsSameBuff(BuffStats otherBuff) => buffData == otherBuff.buffData;
    public bool IsSameBuff(CombatBuff otherBuffData) => buffData == otherBuffData;
    public bool IsSameTypeOfBuff(BuffStats otherBuff)
    {
        if (IsSameBuff(otherBuff)) return true;
        if (affectStats.Count != otherBuff.affectStats.Count) return false;
        for (int i = 0; i < affectStats.Count; i++)
        {
            CombatStatAttributes attribute = affectStats[i];
            CombatStatAttributes otherBuffAttr = otherBuff.affectStats[i];

            if (attribute.AtbrType != otherBuffAttr.AtbrType)
                return false;
        }
        return 
            stackable == otherBuff.stackable &&
            isNerf == otherBuff.isNerf;
    }

    public bool IsStrongerBuff(BuffStats otherBuff)
    {
        if (!IsSameTypeOfBuff(otherBuff)) return false;
        return priority > otherBuff.priority;
    }

    public string buffName => buffContext.buffName;
    public string buffDescription => buffContext.buffDescription;
    public Sprite buffIcon => buffContext.buffIcon;
    public List<CombatStatAttributes> affectStats => buffContext.affectStats;
    public float interval => buffContext.interval;
    public float duration => buffContext.duration;
    public int priority => buffContext.priority;
    public bool usePercentage => buffContext.usePercentage;
    public bool stackable => buffContext.stackable;
    public bool isNerf => buffContext.isNerf;
}
