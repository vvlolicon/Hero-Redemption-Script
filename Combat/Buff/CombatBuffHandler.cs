using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IBuffReceiver
{
    public void AddBuff(CombatBuff buff);
}

public class CombatBuffHandler : MonoBehaviour, IBuffReceiver
{
    float timer = 0;
    [SerializeField]List<BuffStats> _buffs = new List<BuffStats>();
    List<BuffStats> _identitalBuffs = new List<BuffStats>();
    List<BuffStats> _stackableBuffs => _buffs.Where(b => b.stackable).ToList();

    EnemyStateExecutor _enemyExecutor;
    PlayerStateExecutor _playerExecutor;

    GeneralCombatStats _originStats { get {
        if (_isPlayer)
            return _playerExecutor.PlayerOriginStats.GetCombatStats();
        return _enemyExecutor.OriginStats.GetCombatStats();
    }}
    GeneralCombatStats _curCombatStats { get {
        return _isPlayer ? _playerExecutor.PlayerCombatStats : _enemyExecutor.CombatStats;
    }}
    GeneralCombatStats _lastChangedStats = new();
    GeneralCombatStats _statsAfterChanges = new();

    Dictionary<CombatStatsType, float> _buffAffectValues = new ();
    Dictionary<CombatStatsType, float> _buffAffectValuesPerc = new();

    public delegate void OnStatsChangedDelegate();
    public event OnStatsChangedDelegate OnStatsChanged;

    PlayerBackpack PlayerBackpack { get { return GameObjectManager.TryGetPlayerComp<PlayerBackpack>(); } }

    bool _isPlayer;

    private void Start()
    {
        _isPlayer = TryGetComponent(out _playerExecutor);
        if (!_isPlayer)
        {
            _enemyExecutor = GetComponent<EnemyStateExecutor>();
        }
        if (_isPlayer)
        {
            _playerExecutor.OnStatsChanged += HandleExternalStatsChange;
            PlayerBackpack.OnStatsChanged += HandleEquipmentChange;
            _statsAfterChanges.SetStats(_playerExecutor.PlayerCombatStats);
        }
        else 
        {
            _enemyExecutor.OnStatsChanged += HandleExternalStatsChange;
            _statsAfterChanges.SetStats(_enemyExecutor.CombatStats);
        }
        _lastChangedStats.SetStats(_statsAfterChanges);
        ResetAffectValues();
    }
    private void OnDestroy()
    {
        if (_isPlayer)
        {
            _playerExecutor.OnStatsChanged -= HandleExternalStatsChange;
            PlayerBackpack.OnStatsChanged -= HandleEquipmentChange;
        }
        else
        {
            _enemyExecutor.OnStatsChanged -= HandleExternalStatsChange;
        }
    }

    void ResetAffectValues()
    {
        for(int i = 0; i< typeof(CombatStatsType).GetEnumCount(); i++)
        {
            CombatStatsType type = (CombatStatsType)i;
            _buffAffectValues[type] = 0;
            _buffAffectValuesPerc[type] = 0;
        }
    }

    void FixedUpdate()
    {
        HandleStats();
        if (_isPlayer)
        {
            if (!_playerExecutor.HasInitialized)
            {
                _playerExecutor.InitializeCombatStats();
            }
        }
    }

    void HandleStats()
    {
        ResetStats();
        float[] statsChanges = GetStatsChanges();
        if (_buffs.Count > 0)
        {
            for(int i = _buffs.Count -1; i>= 0; i--)
            { // buff can be removed, therefore loop reversely
                HandleBuff(_buffs[i]);
            }
        }
        CalHPandMPChange(statsChanges);
        SetStatsForObject();
    }

    void HandleExternalStatsChange()
    {
        _statsAfterChanges.SetStats(_curCombatStats);
        _lastChangedStats.SetStats(_curCombatStats);
        Debug.Log($"external stats change {_statsAfterChanges.HP}, {_statsAfterChanges.MP}");
    }

    void HandleEquipmentChange()
    {
        Debug.Log($"_curCombatStats.HP = {_curCombatStats.HP}, _curCombatStats.MaxHP = {_curCombatStats.MaxHP}");
        Debug.Log($"_lastChangedStats.HP = {_lastChangedStats.HP}, _lastChangedStats.MaxHP = {_lastChangedStats.MaxHP}");
        ResetStats();
        SetStatsForObject();
    }

    #region Stats Calculation
    void ResetStats()
    {
        _statsAfterChanges.SetStats(_originStats);
        if (_isPlayer)
        {
            // atk and def times to 1.3 to the power of level
            _statsAfterChanges.ATK = _originStats.ATK * Mathf.Pow(1.3f, PlayerBackpack.PlayerLevel - 1);
            _statsAfterChanges.DEF = _originStats.DEF * Mathf.Pow(1.3f, PlayerBackpack.PlayerLevel - 1);
            _statsAfterChanges.AddStatsRange(_playerExecutor.ExtraStats);

            bool HPfull = _curCombatStats.HP >= _curCombatStats.MaxHP;
            bool MPfull = _curCombatStats.MP >= _curCombatStats.MaxMP;
            _statsAfterChanges.AddStatsRange(PlayerBackpack.GetEquippedItemStats());
            
            if(_statsAfterChanges.MaxHP < _lastChangedStats.MaxHP)
            {
                if(_lastChangedStats.HP > _statsAfterChanges.MaxHP)
                    _statsAfterChanges.HP = _statsAfterChanges.MaxHP - (_lastChangedStats.MaxHP - _lastChangedStats.HP);
                else
                    _statsAfterChanges.HP = _lastChangedStats.HP;
            }
            else if(_statsAfterChanges.MaxHP > _lastChangedStats.MaxHP)
            {
                _statsAfterChanges.HP = (HPfull) ? _statsAfterChanges.MaxHP : _lastChangedStats.HP;
            }
            else
            {
                _statsAfterChanges.HP = _lastChangedStats.HP;
            }

            if (_statsAfterChanges.MaxMP < _lastChangedStats.MaxMP)
            {
                if (_lastChangedStats.MP > _statsAfterChanges.MaxMP)
                    _statsAfterChanges.MP = _statsAfterChanges.MaxMP - (_lastChangedStats.MaxMP - _lastChangedStats.MP);
                else
                    _statsAfterChanges.MP = _lastChangedStats.MP;
            }
            else if (_statsAfterChanges.MaxMP > _lastChangedStats.MaxMP)
            {
                _statsAfterChanges.MP = (MPfull) ?_statsAfterChanges.MaxMP: _lastChangedStats.MP;
            }
            else
            {
                _statsAfterChanges.MP = _lastChangedStats.MP;
            }
        }
    }
    float[] GetStatsChanges()
    {
        float HPDiff = _statsAfterChanges.HP - _lastChangedStats.HP; 
        float MPDiff = _statsAfterChanges.MP - _lastChangedStats.MP; 
        return new float[] { HPDiff, MPDiff };
    }
    void CalHPandMPChange(float[] statsChanges)
    {
        float finalHP = _statsAfterChanges.HP - statsChanges[0];
        float finalMP = _statsAfterChanges.MP - statsChanges[1];
        _statsAfterChanges.HP = Mathf.Clamp(finalHP, 0, _statsAfterChanges.MaxHP);
        _statsAfterChanges.MP = Mathf.Clamp(finalMP, 0, _statsAfterChanges.MaxMP);
    }
    void SetStatsForObject()
    {
        if (_isPlayer)
            _playerExecutor.PlayerCombatStats.SetStats(_statsAfterChanges);
        else
            _enemyExecutor.CombatStats.SetStats(_statsAfterChanges);
        _lastChangedStats.SetStats(_statsAfterChanges);
    }
    #endregion

    void HandleBuff(BuffStats buff)
    {
        if (buff.CountDown(Time.fixedDeltaTime))
        {
            HandleHPorMPChangeOnBuff(buff);
        }
        CheckBuffExpired(buff);
        UpdateStatsForBuff(buff);
    }

    void HandleHPorMPChangeOnBuff(BuffStats buff)
    {
        foreach (var attr in buff.affectStats)
        {
            CombatStatsType statType = attr.AtbrType;
            if (statType != CombatStatsType.HP && statType != CombatStatsType.MP) continue;
            if (!buff.usePercentage)
                _statsAfterChanges.ChangeStats(statType, attr.Value);
            else
            {
                var maxValue = (statType == CombatStatsType.HP) ? _originStats.MaxHP : _originStats.MaxMP;
                var newValue = Mathf.Clamp(
                    _originStats.GetStats(statType) * (1 + _buffAffectValuesPerc[statType] / 100),
                    0, maxValue);
                _statsAfterChanges.SetStats(statType, newValue);
            }
        }
    }

    void CheckBuffExpired(BuffStats buff)
    {
        if (buff.IsExpired())
        {
            _buffs.Remove(buff);
            if (!buff.stackable)
            {
                BuffStats newBuff = null;
                foreach (BuffStats existingBuff in _buffs)
                {
                    if (!buff.IsSameTypeOfBuff(existingBuff)) continue;
                    if (newBuff == null || existingBuff.IsStrongerBuff(newBuff))
                    {
                        newBuff = existingBuff;
                    }
                }
                //var passedTime = buff.GetPassedTime();
                RemoveBuffStats(buff);
                _identitalBuffs.Remove(buff);
                if (newBuff != null)
                {
                    _identitalBuffs.Add(newBuff);
                    AddBuffStats(newBuff);
                }
            }
            else
            {
                RemoveBuffStats(buff);
            }
        }
    }
    void UpdateStatsForBuff(BuffStats buff)
    {
        Dictionary<CombatStatsType, float> tmpStats = _statsAfterChanges.GetAllStats();
        Dictionary<CombatStatsType, float> statsChangeForPerc = new();
        foreach(var statsChange in _buffAffectValuesPerc)
        {
            float perc = statsChange.Value;
            if (perc == 0) continue;
            CombatStatsType statType = statsChange.Key;
            if (statType == CombatStatsType.HP || statType == CombatStatsType.MP) continue;
            statsChangeForPerc[statType] = tmpStats[statType] * perc / 100;
        }
        for(int i = 0; i< typeof(CombatStatsType).GetEnumCount(); i++)
        {
            CombatStatsType statType = (CombatStatsType)i;
            if (statType == CombatStatsType.HP || statType == CombatStatsType.MP) continue;
            if (statsChangeForPerc.ContainsKey(statType))
            {
                _statsAfterChanges.ChangeStats(statType, statsChangeForPerc[statType]);
            }
            if (_buffAffectValues.ContainsKey(statType))
            {
                _statsAfterChanges.ChangeStats(statType, _buffAffectValues[statType]);
            }
        }
    }

    public void AddBuff(CombatBuff buff)
    {
        foreach (var existBuff in _buffs)
        { // check if any buff exists from incoming buff
            if (existBuff.IsSameBuff(buff))
            { // reset time if same buff incoming
                existBuff.ResetPassedTime();
                return;
            }
        }
        BuffStats newBuff = buff.CreateBuff();
        //Debug.Log($"adding buff {newBuff.buffName} for {gameObject.name}");
        _buffs.Add(newBuff);
        if (!newBuff.stackable)
        {
            if (_identitalBuffs.Count == 0)
            {
                _identitalBuffs.Add(newBuff);
                AddBuffStats(newBuff);
                return;
            }
            bool existsIdentitalBuff = false;
            foreach (var identitalBuff in _identitalBuffs)
            { // check if any buff in list has greater priority than new buff
                if (newBuff.IsSameTypeOfBuff(identitalBuff))
                {
                    existsIdentitalBuff = true;
                    if (newBuff.priority > identitalBuff.priority)
                    {
                        _identitalBuffs.Remove(identitalBuff);
                        _identitalBuffs.Add(newBuff);
                        RemoveBuffStats(identitalBuff);
                        AddBuffStats(newBuff);
                        return;
                    }
                }
            }
            if (!existsIdentitalBuff)
            {
                // Cannot find any buff in list with is same type of new buff, just add it
                _identitalBuffs.Add(newBuff);
                AddBuffStats(newBuff);
            }
        }
        else
        {
            AddBuffStats(newBuff);
        }
    }

    void AddBuffStats(BuffStats buff)
    {
        Debug.Log($"adding stats of {buff.buffName} for {gameObject.name}");
        foreach (var attr in buff.affectStats)
        {
            CombatStatsType statType = attr.AtbrType;
            float value = attr.Value;
            if (statType == CombatStatsType.HP && statType == CombatStatsType.MP) continue;
            if (!buff.usePercentage)
            {
                _buffAffectValues[statType] += value;
                //Debug.Log($"_buffAffectValues {statType} = {_buffAffectValues[statType]}");
            }
            else
            {
                _buffAffectValuesPerc[statType] += value;
                //Debug.Log($"_buffAffectValuesPerc {statType} = {_buffAffectValuesPerc[statType]}");
            }
        }
    }

    void RemoveBuffStats(BuffStats buff)
    {
        Debug.Log($"removing buff {buff.buffName} for {gameObject.name}");
        foreach (var attr in buff.affectStats)
        {
            CombatStatsType statType = attr.AtbrType;
            float value = attr.Value;
            //Debug.Log($"removing {statType} : {value}");
            if (statType == CombatStatsType.HP && statType == CombatStatsType.MP) continue;
            if (!buff.usePercentage)
            {
                _buffAffectValues[statType] -= value;
                //Debug.Log($"_buffAffectValues {statType} = {_buffAffectValues[statType]}");
            }
            else
            {
                _buffAffectValuesPerc[statType] -= value;
                //Debug.Log($"_buffAffectValuesPerc {statType} = {_buffAffectValuesPerc[statType]}");
            }
        }
    }
}
