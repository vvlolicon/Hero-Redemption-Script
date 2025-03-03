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
    TMP_Text Stats_CritChanceRdc_text;
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
        Stats_MaxHP_text = StatsPanel.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
        Stats_MaxMP_text = StatsPanel.transform.GetChild(1).GetChild(1).GetComponent<TMP_Text>();
        Stats_ATK_text = StatsPanel.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>();
        Stats_AtkTime_text = StatsPanel.transform.GetChild(3).GetChild(1).GetComponent<TMP_Text>();
        Stats_DEF_text = StatsPanel.transform.GetChild(4).GetChild(1).GetComponent<TMP_Text>();
        Stats_SPEED_text = StatsPanel.transform.GetChild(5).GetChild(1).GetComponent<TMP_Text>();
        Stats_CritChance_text = StatsPanel.transform.GetChild(6).GetChild(1).GetComponent<TMP_Text>();
        //Stats_CritChanceRdc_text = StatsPanel.transform.GetChild(7).GetChild(1).GetComponent<TMP_Text>();
        //Stats_CritMult_text = StatsPanel.transform.GetChild(8).GetChild(1).GetComponent<TMP_Text>();
        //Stats_CritResis_text = StatsPanel.transform.GetChild(9).GetChild(1).GetComponent<TMP_Text>();
        Stats_DmgReduce_text = StatsPanel.transform.GetChild(7).GetChild(1).GetComponent<TMP_Text>();
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
            float CritChance = playerStats.CritChance;
            float CritChanRdc = playerStats.CritChanRdc;
            float CritDmgMult = playerStats.CritDmgMult;
            // if the stats contains digits, show only 2 digits
            RoundToXDecimal(MaxHP, 2);
            RoundToXDecimal(MaxMP, 2);
            RoundToXDecimal(CritChance, 2);
            RoundToXDecimal(CritChanRdc, 2);
            RoundToXDecimal(CritDmgMult, 2);

            // restrict the maximum value to 100%
            float critResisPerc = Mathf.Min(playerStats.CritDmgResis, 100);
            float dmgReducePerc = Mathf.Min(playerStats.DmgReduce, 100);

            Stats_MaxHP_text.text = "" + MaxHP;
            Stats_MaxMP_text.text = "" + MaxMP;
            Stats_ATK_text.text = "" + playerStats.ATK;
            Stats_AtkTime_text.text = "" + playerStats.AttackTime * 100 + "%";
            Stats_DEF_text.text = "" + playerStats.DEF;
            Stats_SPEED_text.text = "" + playerStats.SPEED;
            Stats_CritChance_text.text = "" + playerStats.CritChance + "%";
            //Stats_CritChanceRdc_text.text = "" + playerStats.CritChanRdc + "%";
            //Stats_CritMult_text.text = "" + playerStats.CritDmgMult;
            //Stats_CritResis_text.text = "" + critResisPerc + "%";
            Stats_DmgReduce_text.text = "" + dmgReducePerc + "%";
        }
        //playerController.MoveSpeed = playerStats.SPEED/10;
    }

    void RoundToXDecimal(float value, int numDecimal)
    {
        if (value % 1 != 0)
        {
            value = (float)Math.Round(value, numDecimal);
        }
    }
}
