using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void ApplyDamage(DmgInfo dmgInfo);
}
public abstract class DmgInfo
{
    //int _dmgValue; //Damage value
    public float ATK { get; protected set; }
    public int damageShow { get; protected set; }
    public float CritChance { get; protected set; }
    public float CritMult { get; protected set; }

    //public void RaiseEvent(GameEventOnAction action)
    //{
    //    action.Raise(this);
    //}
    public void CallDamageable(GameObject other)
    {
        Debug.Log("causing damage to " + other.gameObject.name);
        IDamageable[] damageable = other.GetComponents<IDamageable>();
        foreach (var dmg in damageable)
        {
            Debug.Log("triggering: " + dmg.ToString());
            dmg.ApplyDamage(this);
        }
    }
}

public class EnemyDmgInfo : DmgInfo
{
    public Color TextColor { get; private set; } //Color of the damage text
    public Transform DmgTextPos { get; private set; } //Position where the damage text pops out
    public GameObject Target { get; private set; } //damage target
    public bool IsCrit { get; private set; }
    

    // use when showing damage detail on enmey
    public EnemyDmgInfo(int dmg, bool isCrit, Color tcolor, Transform dmgTextPos, GameObject target)
    {
        damageShow = dmg;
        TextColor = tcolor;
        DmgTextPos = dmgTextPos;
        IsCrit = isCrit;
        this.Target = target;
    }

    // use when player cause damage to the enemy
    public EnemyDmgInfo(float atk, float critChance, float critMult, Color tcolor, Transform dmgTextPos, GameObject target)
    {
        ATK = atk;
        CritChance = critChance;
        CritMult = critMult;
        TextColor = tcolor;
        DmgTextPos = dmgTextPos;
        this.Target = target;
    }
}

// use when the enemy deal damage to player
public class PlayerDmgInfo : DmgInfo
{
    //Player attribute
    public Vector3 DmgDir { get; private set; } //Direction of Damage comes from
    public float Force { get; private set; } //Force applied to the player

    public PlayerDmgInfo(float atk, Vector3 dmgD, float force)
    {
        ATK = atk;
        DmgDir = dmgD;
        Force = force;
        // normally enemy should not have critical hit
        CritChance = -100f;
        CritMult = 0;
    }

    // in case if any enemy has critical hit
    public PlayerDmgInfo(float atk, float critChance, float critMult, Vector3 dmgD, float force)
    {
        CritChance = critChance;
        CritMult = critMult;
        ATK = atk;
        DmgDir = dmgD;
        Force = force;
    }
}
