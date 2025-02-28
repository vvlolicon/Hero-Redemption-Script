using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatManager : MonoBehaviour
{
    public GeneralStatsObj playerStats;
    public GeneralStatsObj initialPlayerStat;
    public TMP_Text HUD_HP_text;
    public TMP_Text HUD_MP_text;
    public TMP_Text Stats_MaxHP_text;
    public TMP_Text Stats_MaxMP_text;
    public TMP_Text Stats_ATK_text;
    public TMP_Text Stats_AtkTime_text;
    public TMP_Text Stats_DEF_text;
    public TMP_Text Stats_SPEED_text;
    public TMP_Text Stats_CritChance_text;
    public TMP_Text Stats_CritMult_text;
    public TMP_Text Stats_CritResis_text;
    public TMP_Text Stats_DmgReduce_text;

    //public ThirdPersonController playerController;

    private void Start()
    {
        //playerController = GetComponent<ThirdPersonController>();
        playerStats.setStats(initialPlayerStat);
    }

    // Update is called once per frame
    void Update()
    {
        HUD_HP_text.text = playerStats.maxHP + " / " + playerStats.HP;
        HUD_MP_text.text = playerStats.maxMP + " / " + playerStats.MP;
        Stats_MaxHP_text.text = "MaxHP: " + playerStats.maxHP;
        Stats_MaxMP_text.text = "MaxMP: " + playerStats.maxMP;
        Stats_ATK_text.text = "Attack: " + playerStats.ATK;
        Stats_AtkTime_text.text = "Attack Time: " + playerStats.AttackTime + "s";
        Stats_DEF_text.text = "Defence: " + playerStats.DEF;
        Stats_SPEED_text.text = "Speed: " + playerStats.SPEED;
        Stats_CritChance_text.text = "Crit. %: " + playerStats.CritChance + "%";
        Stats_CritMult_text.text = "Crit. Mult: " + playerStats.CritMult;
        Stats_CritResis_text.text = "Crit. Resis. : " + playerStats.CritResis + "%";
        Stats_DmgReduce_text.text = "Dmg. Reduce: " + playerStats.DmgReduction + "%";
        //playerController.MoveSpeed = playerStats.SPEED/10;
    }
}
