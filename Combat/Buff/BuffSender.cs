using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using static BuffSetting;

public interface IBuffSender
{
    public void AttachBuffToSelf(GameObject victim);
}
public class BuffSender : MonoBehaviour
{
    public List<BuffSetting> buffSettings;
    public AttachBuffWays howToAttachBuff;
    GeneralCombatStats _combatStats
    {
        get
        {
            if (_isPlayer)
                return _playerExecutor.PlayerCombatStats;
            return _enemyExecutor.CombatStats;
        }
    }

    EnemyStateExecutor _enemyExecutor;
    PlayerStateExecutor _playerExecutor;
    CombatBuffHandler playerBuffHandler { get { return PlayerCompManager.TryGetPlayerComp<CombatBuffHandler>(); } }

    bool _isPlayer;

    private void Start()
    {
    }

    public void SendBuffToSelf()
    {
        if (TryGetComponent(out IBuffReceiver buffReceiver))
        {
            foreach (var buffSetting in buffSettings) 
            {
                if (buffSetting.IsConditionMet(_combatStats))
                {
                    buffReceiver.AddBuff(buffSetting.buff);
                }
            }
        }
    }

    public void SendBuff(GameObject victim)
    {
        if (victim.TryGetComponent(out IBuffReceiver buffReceiver))
        {
            foreach (var buffSetting in buffSettings)
                buffReceiver.AddBuff(buffSetting.buff);
        }
    }

    public void SendBuffToPlayer()
    {
        foreach (var buffSetting in buffSettings)
            playerBuffHandler.AddBuff(buffSetting.buff);
    }
}

public enum AttachBuffWays
{
    Self, Damage, UseItem, NONE
}

[System.Serializable]
public class BuffSetting
{
    public CombatBuff buff;
    public AttachBuffCondition condition;

    public bool IsConditionMet(GeneralCombatStats stats)
    {
        if (condition == null) return true;
        return condition.IsConditionMet(stats);
    }
}

[System.Serializable]
public class AttachBuffCondition
{
    public CombatStatsType CompareStatType;
    public Equator CompareEquator;
    public float CompareValue;
    [SerializeField] bool UsePercentage;

    public enum Equator
    {
        LessThan, LessOrEqualThan,
        GreaterThan, GreaterOrEqualThan,
        EqualTo
    }

    public bool IsConditionMet(GeneralCombatStats stats)
    {
        if (UsePercentage && CompareStatType == CombatStatsType.HP)
        {
            float perc = stats.HP / stats.MaxHP;
            return CompareEquator switch
            {
                Equator.LessThan => perc < CompareValue,
                Equator.LessOrEqualThan => perc <= CompareValue,
                Equator.GreaterThan => perc > CompareValue,
                Equator.GreaterOrEqualThan => perc >= CompareValue,
                Equator.EqualTo => perc == CompareValue,
                _ => false,
            };
        }
        return CompareEquator switch
        {
            Equator.LessThan => stats.GetStats(CompareStatType) < CompareValue,
            Equator.LessOrEqualThan => stats.GetStats(CompareStatType) <= CompareValue,
            Equator.GreaterThan => stats.GetStats(CompareStatType) > CompareValue,
            Equator.GreaterOrEqualThan => stats.GetStats(CompareStatType) >= CompareValue,
            Equator.EqualTo => stats.GetStats(CompareStatType) == CompareValue,
            _ => false,
        };
    }
}
