using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Game Functions/Health Manager")]
public class HealthManager : ScriptableObject
{
    [SerializeField] public float health;
    [SerializeField] public float maxHealth = 100;
    public bool infiniteHP = false;
    //public UnityEvent<DmgInfo> healthChangeEvent;
    public GameObject damageTextPrefab;

    private Camera camera;


    public void Initialize()
    {
        health = maxHealth;
        
    }

    public void Damage(float dmg)
    {
        if (!infiniteHP)
        {
            onHealthChange(-1 * dmg);
        }
            //health -= 
    }

    public void createHealthMeg(DmgInfo TDmgInfo)
    {
        //throws a damage text
        if (TDmgInfo is EnemyDmgInfo)
        {
            EnemyDmgInfo dmgInfo = (EnemyDmgInfo)TDmgInfo;
            camera = Camera.main;
            Vector2 randomOffset = new Vector2(Random.Range(0.5f, 1f) * RandomMethods.RandomPosNegNumber(), Random.Range(0, 1f));
            // multiply the offset to 100f to enlarge the randomlise offset position
            Vector3 textPos = RandomMethods.ScreenPointOffset(camera, dmgInfo.DmgTextPos.position, randomOffset * 100f);
            GameObject dmgText = Instantiate(damageTextPrefab, textPos, Quaternion.identity);
            string damageShow = "" + dmgInfo.damageShow;
            if (dmgInfo.IsCrit)
            {
                dmgText.transform.localScale *= 1.5f;
                damageShow += "!";
            }
            dmgText.GetComponent<DamagePopup>().SetUp(damageShow, dmgInfo.TextColor, randomOffset);
        }
    }
    public void onHealthChange(float value)
    {
        health += value;
        if (health <= 0)
        {
            //Debug.Log("player dies");
        }
    }

    public static DmgResult calculateDamage(float atk, float def, float critChance, float dmgReduc, float critMult, float critResis)
    {
        //atk *(100 / (100 + def)) *[1.3 x(crit dmg multiplier * crit dmg reduction) + 0.3] * dmg reduction x(0.95~1.05)
        bool isCritical = IsCriticalHit(critChance);
        float dmg = atk * (100f / (100f + def));
        if (isCritical)
        {
            dmg *= (1.3f * (1f + critMult) * (1f - critResis));
        }
        dmg *= (1 - dmgReduc) * Random.Range(0.95f, 1.05f);
        return new DmgResult(dmg, isCritical);
    }

    public static bool IsCriticalHit(float critChance)
    {
        //Crit Chance = Base Crit Chance(20%) ¡Á ( 1 + critChance given)
        float chance = 20f * (1 + (critChance / 100));
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
