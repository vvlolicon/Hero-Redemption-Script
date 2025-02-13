using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Function/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    public float maxHP;
    public float HP;
    public float maxMP;
    public float MP;
    public float ATK;
    public float SPEED;
    public int DEF;

    private void OnEnable()
    {
        HP = maxHP;
        MP = maxMP;
    }

    public void setPlayerStat(PlayerStats playerStats)
    {
        maxHP = playerStats.maxHP;
        HP = maxHP;
        maxMP = playerStats.maxMP;
        MP = maxMP;
        ATK = playerStats.ATK;
        DEF = playerStats.DEF;
        SPEED = playerStats.SPEED;
    }
}
