using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatDisplay  : MonoBehaviour
{
    GeneralCombatStats _playerCombatStats { get { return PlayerCompManager.TryGetPlayerComp<PlayerStateExecutor>().PlayerCombatStats; } }
    PlayerBackpack _playerBackpack { get { return PlayerCompManager.TryGetPlayerComp<PlayerBackpack>(); } }
    [SerializeField] Transform StatsPanel;
    [SerializeField] Transform MainStats;

    TMP_Text Stats_MaxHP_text, Stats_MaxMP_text, Stats_ATK_text, Stats_AtkTime_text,Stats_DEF_text
        ,Stats_SPEED_text,Stats_CritChance_text,Stats_CritChanceRdc_text,Stats_CritMult_text,
        Stats_CritResis_text, Stats_DmgReduce_text,
        PlayerMoneyText, PlayerLevelText;

    //public ThirdPersonController playerController;
    private void Start()
    {
        //playerController = GetComponent<ThirdPersonController>();
        Stats_MaxHP_text = StatsPanel.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
        Stats_MaxMP_text = StatsPanel.GetChild(1).GetChild(1).GetComponent<TMP_Text>();
        Stats_ATK_text = StatsPanel.GetChild(2).GetChild(1).GetComponent<TMP_Text>();
        Stats_AtkTime_text = StatsPanel.GetChild(3).GetChild(1).GetComponent<TMP_Text>();
        Stats_DEF_text = StatsPanel.GetChild(4).GetChild(1).GetComponent<TMP_Text>();
        Stats_SPEED_text = StatsPanel.GetChild(5).GetChild(1).GetComponent<TMP_Text>();
        Stats_CritChance_text = StatsPanel.GetChild(6).GetChild(1).GetComponent<TMP_Text>();
        //Stats_CritChanceRdc_text = StatsPanel.GetChild(7).GetChild(1).GetComponent<TMP_Text>();
        //Stats_CritMult_text = StatsPanel.GetChild(8).GetChild(1).GetComponent<TMP_Text>();
        //Stats_CritResis_text = StatsPanel.GetChild(9).GetChild(1).GetComponent<TMP_Text>();
        Stats_DmgReduce_text = StatsPanel.GetChild(7).GetChild(1).GetComponent<TMP_Text>();
        PlayerLevelText = MainStats.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
        PlayerMoneyText = MainStats.GetChild(1).GetChild(1).GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.activeSelf && !gameObject.IsDestroyed() && _playerCombatStats != null)
        {
            UpdateStats();
        }
    }

    void UpdateStats()
    {
        GeneralCombatStats playerCombatStats = this._playerCombatStats;
        float MaxHP = playerCombatStats.MaxHP;
        float HP = playerCombatStats.HP;
        float MaxMP = playerCombatStats.MaxMP;
        float MP = playerCombatStats.MP;
        float HPperc = HP / MaxHP;
        float MPperc = MP / MaxMP;
        HP.RoundToDecimals(2);
        MP.RoundToDecimals(2);

        if (StatsPanel.gameObject.activeInHierarchy)
        {
            float CritChance = playerCombatStats.CritChance;
            float CritChanRdc = playerCombatStats.CritChanRdc;
            float CritDmgMult = playerCombatStats.CritDmgMult;
            // if the stats contains digits, show only 2 digits
            MaxHP.RoundToDecimals(2);
            MaxMP.RoundToDecimals(2);
            CritChance.RoundToDecimals(2);
            CritChanRdc.RoundToDecimals(2);
            CritDmgMult.RoundToDecimals(2);

            // restrict the maximum value to 100%
            float critResisPerc = Mathf.Min(playerCombatStats.CritDmgResis, 100);
            float dmgReducePerc = Mathf.Min(playerCombatStats.DmgReduce, 100);

            Stats_MaxHP_text.text = "" + MaxHP;
            Stats_MaxMP_text.text = "" + MaxMP;
            Stats_ATK_text.text = "" + (int)playerCombatStats.ATK;
            Stats_AtkTime_text.text = "" + playerCombatStats.AttackTime * 100 + "%";
            Stats_DEF_text.text = "" + (int)playerCombatStats.DEF;
            Stats_SPEED_text.text = "" + playerCombatStats.Speed;
            Stats_CritChance_text.text = "" + HealthManager.CalCriticalChance(playerCombatStats.CritChance, 0) + "%";
            //Stats_CritChanceRdc_text.text = "" + playerStats.CritChanRdc + "%";
            //Stats_CritMult_text.text = "" + playerStats.CritDmgMult;
            //Stats_CritResis_text.text = "" + critResisPerc + "%";
            Stats_DmgReduce_text.text = "" + dmgReducePerc + "%";
        }
        if (MainStats.gameObject.activeInHierarchy)
        {
            PlayerLevelText.text = _playerBackpack.PlayerLevel.ToString();
            PlayerMoneyText.text = _playerBackpack.PlayerOwnedMoney + " $";
        }
    }

}
