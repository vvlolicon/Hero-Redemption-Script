using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatManager : MonoBehaviour
{
    public GeneralStatsObj playerStats;
    public PlayerStats initialPlayerStat;
    public TMP_Text HUD_HP_text;
    public TMP_Text HUD_MP_text;
    public TMP_Text Stats_MaxHP_text;
    public TMP_Text Stats_MaxMP_text;
    public TMP_Text Stats_ATK_text;
    public TMP_Text Stats_DEF_text;
    public TMP_Text Stats_SPEED_text;

    //public ThirdPersonController playerController;

    private void Start()
    {
        //playerController = GetComponent<ThirdPersonController>();
        //playerStats.setPlayerStat(initialPlayerStat);
    }

    // Update is called once per frame
    void Update()
    {
        HUD_HP_text.text = playerStats.maxHP + " / " + playerStats.HP;
        HUD_MP_text.text = playerStats.maxMP + " / " + playerStats.MP;
        Stats_MaxHP_text.text = "MaxHP: " + playerStats.maxHP;
        Stats_MaxMP_text.text = "MaxMP: " + playerStats.maxMP;
        Stats_ATK_text.text = "Attack: " + playerStats.ATK;
        Stats_DEF_text.text = "Defence: " + playerStats.DEF;
        Stats_SPEED_text.text = "Speed: " + playerStats.SPEED;
        //playerController.MoveSpeed = playerStats.SPEED/10;
    }
}
