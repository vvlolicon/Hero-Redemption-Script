using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour
{
    public bool infiniteHP = false;
    //public UnityEvent<DmgInfo> healthChangeEvent;
    public GameObject damageTextPrefab;
    public float textPopupOffset = 50f;

    Camera _camera;
    public PlayerStateExecutor _playerExecutor { get; set; }
    public EnemyStateExecutor _enemyExecutor { get; set; }
    //PlayerStatDisplay _statDisplay;

    float health 
    { 
        get {
            if (_playerExecutor != null)
                return _playerExecutor.PlayerStats.HP;
            else if (_enemyExecutor != null)
                return _enemyExecutor.HP;
            else
                return Mathf.Infinity;
        }
        set
        {
            if (_playerExecutor != null)
            {
                _playerExecutor.PlayerStats.HP = value;
            }
            else if (_enemyExecutor != null)
            {
                _enemyExecutor.HP = value;
                Debug.Log(gameObject.name + " HP left: " + maxHealth + " / " + health);
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
                return _playerExecutor.PlayerStats.MaxHP;
            else if (_enemyExecutor != null)
                return _enemyExecutor.MaxHP;
            else
                return Mathf.Infinity;
        }
        set
        {
            if (_playerExecutor != null)
                _playerExecutor.PlayerStats.MaxHP = value;
            else if (_enemyExecutor != null)
                _enemyExecutor.MaxHP = value;
            else
                Debug.Log("cannnot find object to set HP");
        }
    }

    private void Start()
    {
       
    }

    public void Damage(float dmg)
    {
        if (!infiniteHP)
        {
            onHealthChange(-1 * dmg);
        }
            //health -= 
    }

    void ownerDies()
    {
        if (_playerExecutor != null)
            _playerExecutor.OnDying();
        else if (_enemyExecutor != null)
            _enemyExecutor.OnDying();
    }

    public void createHealthMeg(DmgInfo TDmgInfo)
    {
        //throws a damage text
        if (TDmgInfo is EnemyDmgInfo)
        {
            EnemyDmgInfo dmgInfo = (EnemyDmgInfo)TDmgInfo;
            _camera = Camera.main;
            Vector2 randomOffset = new Vector2(Random.Range(0.5f, 1f) * RandomMethods.RandomPosNegNumber(), Random.Range(0, 1f));
            // multiply the offset to offset distance to enlarge the randomlise offset position
            Vector3 textPos = RandomMethods.ScreenPointOffset(_camera, dmgInfo.DmgTextPos.position, randomOffset * textPopupOffset);
            GameObject dmgText = Instantiate(damageTextPrefab, textPos, Quaternion.identity);
            string damageShow = "" + dmgInfo.damageShow;
            Color dmgColor = dmgInfo.TextColor;
            if (dmgInfo.IsCrit)
            {
                // change the damage text color to red and enlarge 1.5 times size to show critical attack
                dmgColor = Color.red;
                dmgText.transform.localScale *= 1.5f;
                damageShow += "!";
            }
            dmgText.GetComponent<DamagePopup>().SetUp(damageShow, dmgColor, randomOffset);
        }
    }
    public void onHealthChange(float value)
    {
        health += value;
        if (health <= 0)
        {
            Debug.Log( gameObject.name + " dies");
            ownerDies();
        }
    }

    public static DmgResult calculateDamage(float atk, float def, float critChance, float critChanRdc, float dmgReduc, float critMult, float critResis)
    {
        //atk *(100 / (100 + def)) *[1.3 x(crit dmg multiplier * crit dmg reduction) + 0.3] * dmg reduction x(0.95~1.05)
        bool isCritical = IsCriticalHit(critChance, critChanRdc);
        float dmg = atk * (100f / (100f + def));
        if (isCritical)
        {
            dmg *= (1.3f * (1f + critMult) * (1f - critResis));
        }
        dmg *= (1 - dmgReduc) * Random.Range(0.95f, 1.05f);
        return new DmgResult(dmg, isCritical);
    }

    public static bool IsCriticalHit(float critChance, float critChanRdc)
    {
        //Crit Chance = Base Crit Chance(20%) ¡Á ( 1 + critChance - critChanRdc)
        float chance = 20f * (1 + (Mathf.Max(critChance - critChanRdc, 0) / 100));
        return chance >= Random.Range(0, 100);
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
