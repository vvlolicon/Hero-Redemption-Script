using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour
{
    public bool infiniteHP = false;
    //public UnityEvent<DmgInfo> healthChangeEvent;
    public GameObject damageTextPrefab;
    public float textPopupOffset = 50f;

    Camera _camera;
    [HideInInspector] PlayerStateExecutor _playerExecutor;
    [HideInInspector] EnemyStateExecutor _enemyExecutor;
    bool _isPlayer;
    
    void Start()
    {
        _isPlayer = TryGetComponent(out _playerExecutor);
        if (!_isPlayer)
        {
            _enemyExecutor = GetComponent<EnemyStateExecutor>();
        }
    }

    float health 
    { 
        get {
            if (_isPlayer)
                return _playerExecutor.PlayerCombatStats.HP;
            else if (_enemyExecutor != null)
                return _enemyExecutor.CombatStats.HP;
            else
                return Mathf.Infinity;
        }
        set
        {
            if (_isPlayer)
            {
                _playerExecutor.PlayerCombatStats.HP = value;
            }
            else if (_enemyExecutor != null)
            {
                _enemyExecutor.CombatStats.HP = value;
                //Debug.Log(gameObject.name + " HP left: " + maxHealth + " / " + health);
            }
            else
                Debug.Log("cannnot find object to set HP");
        }
    }
    float maxHealth
    {
        get
        {
            if (_playerExecutor != null)
                return _playerExecutor.PlayerCombatStats.MaxHP;
            else if (_enemyExecutor != null)
                return _enemyExecutor.CombatStats.MaxHP;
            else
                return Mathf.Infinity;
        }
        set
        {
            if (_playerExecutor != null)
                _playerExecutor.PlayerCombatStats.MaxHP = value;
            else if (_enemyExecutor != null)
                _enemyExecutor.CombatStats.MaxHP = value;
            else
                Debug.Log("cannnot find object to set HP");
        }
    }

    public void Damage(float dmg)
    {
        if (!infiniteHP)
        {
            OnHealthChange(-1 * dmg);
        }
            //health -= 
    }

    void ownerDies()
    {
        if (_isPlayer && _playerExecutor != null)
        {
            _playerExecutor.OnDying();
        }
        if (_enemyExecutor != null)
            _enemyExecutor.OnDying();
    }

    public void CreateHealthMeg(DmgInfo TDmgInfo)
    {
        //throws a damage text
        if (TDmgInfo is EnemyDmgInfo)
        {
            EnemyDmgInfo dmgInfo = (EnemyDmgInfo)TDmgInfo;
            _camera = Camera.main;
            Vector2 randomOffset = new Vector2(Random.Range(0.5f, 1f) * RandomMethods.RandomPosNegNumber(), Random.Range(0, 1f));
            // multiply the offset to offset distance to enlarge the randomlise offset position
            Vector3 textPos = RandomMethods.ScreenPointOffset(_camera, dmgInfo.DmgTextPos.position, randomOffset * textPopupOffset);
            
            string damageShow = "" + dmgInfo.damageShow;
            Color dmgColor = dmgInfo.TextColor;
            if (dmgInfo.IsCrit)
            {
                // change the damage text color to red and enlarge 1.5 times size to show critical attack
                dmgColor = Color.red;
                damageShow += "!";
            }
            GameObject dmgText = CreateMsgPopup(textPos, randomOffset, damageShow, dmgColor);
            if (dmgInfo.IsCrit)
                dmgText.transform.localScale *= 1.5f;
        }
    }

    public GameObject CreateMsgPopup(Vector3 textPos, Vector2 offset, string text, Color color)
    {
        GameObject dmgText = Instantiate(damageTextPrefab, textPos, Quaternion.identity);
        dmgText.GetComponent<DamagePopup>().SetUp(text, color, offset);
        return dmgText;
    }
    public void OnHealthChange(float value)
    {
        health += value;
        //Debug.Log(gameObject.name + " health left: " + health);
        if (health <= 0)
        {
            ownerDies();
        }
    }

    public static DmgResult calculateDamage(float atk, float def, float critChance, float critChanRdc, float dmgReduc, float critMult, float critResis)
    {
        //atk x (100/(100+def)) x [1.5 x (crit dmg multiplier / crit dmg reduction)] x dmg reduction(%) x (0.95~1.05) 
        bool isCritical = IsCriticalHit(critChance, 0);//critChanRdc);
        float dmg = atk * (100f / (100f + def));
        if (isCritical)
        {
            // at lease *1.5 critical dmg
            dmg *= 1.5f;
            //dmg *= Mathf.Max((1f + critMult) / (1f + critResis), 1f);
        }
        dmg *= (1 - (Mathf.Min(dmgReduc, 99)/100)) * Random.Range(0.95f, 1.05f);
        return new DmgResult(dmg, isCritical);
    }

    public static bool IsCriticalHit(float critChance, float critChanRdc)
    {
        //Crit Chance = Base Crit Chance(20%) ¡Á ( 1 + critChance)
        float chance = CalCriticalChance(critChance, critChanRdc);
        return chance >= Random.Range(0, 100);
    }

    public static float CalCriticalChance(float critChance, float critChanRdc)
    {
        //Crit Chance = Base Crit Chance(20%) ¡Á ( 1 + critChance)
        return 20 * (1 + (Mathf.Max(critChance - critChanRdc, 0) / 100));

    }


}

public class RandomMethods
{
    public static Vector3 ScreenPointOffset(Camera camera, Vector3 pos, Vector2 offset)
    {

        Vector3 posInCam = camera.WorldToScreenPoint(pos);
        posInCam.x += offset.x;
        posInCam.y += offset.y;
        pos = camera.ScreenToWorldPoint(posInCam);

        return pos;
    }

    public static int RandomPosNegNumber()
    {
        return Random.Range(0, 2) * 2 - 1;
    }
}

public class DmgResult
{
    public float Dmg;
    public bool IsCritHit;
    public DmgResult(float dmg, bool isCrit) 
    {
        Dmg = dmg;
        IsCritHit = isCrit;
    }
}
