using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatDisplay  : MonoBehaviour
{
    public GeneralStatsObj playerStats;
    public GeneralStatsObj initialPlayerStat;
    public TMP_Text HUD_HP_text;
    public TMP_Text HUD_MP_text;
    public GameObject StatsPanel;

    TMP_Text Stats_MaxHP_text;
    TMP_Text Stats_MaxMP_text;
    TMP_Text Stats_ATK_text;
    TMP_Text Stats_AtkTime_text;
    TMP_Text Stats_DEF_text;
    TMP_Text Stats_SPEED_text;
    TMP_Text Stats_CritChance_text;
    TMP_Text Stats_CritMult_text;
    TMP_Text Stats_CritResis_text;
    TMP_Text Stats_DmgReduce_text;

    Slider HPBar;
    Slider MPBar;

    //public ThirdPersonController playerController;
    private void Start()
    {
        //playerController = GetComponent<ThirdPersonController>();
        playerStats.setStats(initialPlayerStat);
        Stats_MaxHP_text = StatsPanel.transform.GetChild(0).GetComponent<TMP_Text>();
        Stats_MaxMP_text = StatsPanel.transform.GetChild(1).GetComponent<TMP_Text>();
        Stats_ATK_text = StatsPanel.transform.GetChild(2).GetComponent<TMP_Text>();
        Stats_AtkTime_text = StatsPanel.transform.GetChild(3).GetComponent<TMP_Text>();
        Stats_DEF_text = StatsPanel.transform.GetChild(4).GetComponent<TMP_Text>();
        Stats_SPEED_text = StatsPanel.transform.GetChild(5).GetComponent<TMP_Text>();
        Stats_CritChance_text = StatsPanel.transform.GetChild(6).GetComponent<TMP_Text>();
        Stats_CritMult_text = StatsPanel.transform.GetChild(7).GetComponent<TMP_Text>();
        Stats_CritResis_text = StatsPanel.transform.GetChild(8).GetComponent<TMP_Text>();
        Stats_DmgReduce_text = StatsPanel.transform.GetChild(9).GetComponent<TMP_Text>();
        HPBar = transform.GetChild(0).GetComponent<Slider>();
        MPBar = transform.GetChild(1).GetComponent<Slider>();

    }

    // Update is called once per frame
    void Update()
    {
        float MaxHP = playerStats.MaxHP;
        float HP = playerStats.HP;
        float MaxMP = playerStats.MaxMP;
        float MP = playerStats.MP;
        float HPperc = HP / MaxHP;
        float MPperc = MP / MaxMP;

        HPBar.value = HPperc;
        MPBar.value = MPperc;
        HUD_HP_text.text = Mathf.Floor(HP) + " / " + Mathf.Floor(MaxHP);
        HUD_MP_text.text = Mathf.Floor(MP) + " / " + Mathf.Floor(MaxMP);

        if (StatsPanel.activeInHierarchy)
        {
            // if the stats contains digits, show only 2 digits
            if (MaxHP % 1 != 0)
            {
                MaxHP = (float)Math.Round(MaxHP, 2);
            }
            if (MaxMP % 1 != 0)
            {
                MaxMP = (float)Math.Round(MaxMP, 2);
            }
            // restrict the maximum value to 100%
            float critResisPerc = Mathf.Min(playerStats.CritDmgResis, 100);
            float dmgReducePerc = Mathf.Min(playerStats.DmgReduction, 100);

            Stats_MaxHP_text.text = "MaxHP: " +  MaxHP;
            Stats_MaxMP_text.text = "MaxMP: " + MaxMP;
            Stats_ATK_text.text = "Attack: " + playerStats.ATK;
            Stats_AtkTime_text.text = "Attack Time: " + playerStats.AttackTime + "s";
            Stats_DEF_text.text = "Defence: " + playerStats.DEF;
            Stats_SPEED_text.text = "Speed: " + playerStats.SPEED;
            Stats_CritChance_text.text = "Crit. %: " + playerStats.CritChance + "%";
            Stats_CritMult_text.text = "Crit. Mult: " + playerStats.CritDmgMult;
            Stats_CritResis_text.text = "Crit. Resis. : " + critResisPerc + "%";
            Stats_DmgReduce_text.text = "Dmg. Reduce: " + dmgReducePerc + "%";
        }
        //playerController.MoveSpeed = playerStats.SPEED/10;
    }
}
