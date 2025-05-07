using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class BossHudControl : MonoBehaviour
{
    [SerializeField] TMP_Text BossName;
    [SerializeField] TMP_Text BossHP;
    [SerializeField] Slider BossHealthBar;

    GeneralCombatStats BossStats;
    bool HasSetBoss = false;

    public void SetBoss(string bossName, GeneralCombatStats BossStats)
    {
        this.BossName.text = bossName;
        this.BossStats = BossStats;
        HasSetBoss = true;
        BossHealthBar.value = 1;
    }

    private void Update()
    {
        if (!HasSetBoss || !gameObject.activeSelf) return;
        float maxHP = BossStats.MaxHP;
        float HP = BossStats.HP;
        if (HP <= 0)
        { // stop showing ui if boss die
            HasSetBoss = false;
            gameObject.SetActive(false);
            return;
        }
        BossHealthBar.value = HP / maxHP;
        HP.RoundToDecimals(2);
        BossHP.text = HP + " / " + Mathf.Floor(maxHP);
    }
}