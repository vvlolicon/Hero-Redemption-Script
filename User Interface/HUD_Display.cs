using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Display : MonoBehaviour
{
    [SerializeField] TMP_Text HUD_HP_text;
    [SerializeField] TMP_Text HUD_MP_text;

    [SerializeField] Slider HPBar;
    [SerializeField] Image HPBarFill;
    [SerializeField] Slider MPBar;
    [SerializeField] Image MPBarFill;

    GeneralCombatStats _playerCombatStats { get { return PlayerCompManager.TryGetPlayerComp<PlayerStateExecutor>().PlayerCombatStats; } }
    CombatBuffHandler _playerBuffHandler { get { return PlayerCompManager.TryGetPlayerComp<CombatBuffHandler>(); } }

    private void Update()
    {
        if (gameObject.activeSelf && !gameObject.IsDestroyed())
        {
            UpdateHUDSliders();
        }
    }

    void UpdateHUDSliders()
    {
        GeneralCombatStats playerCombatStats = _playerCombatStats;
        float MaxHP = playerCombatStats.MaxHP;
        float HP = playerCombatStats.HP;
        float MaxMP = playerCombatStats.MaxMP;
        float MP = playerCombatStats.MP;
        float HPperc = HP / MaxHP;
        float MPperc = MP / MaxMP;
        HPBar.value = HPperc;
        MPBar.value = MPperc;
        HUD_HP_text.text = Mathf.Floor(HP) + " / " + Mathf.Floor(MaxHP);
        HUD_MP_text.text = Mathf.Floor(MP) + " / " + Mathf.Floor(MaxMP);
        if (_playerBuffHandler.HasPoisonBuff())
        {
            HPBarFill.color = Color.green;
        }
        else
        {
            HPBarFill.color = Color.red;
        }
        if (_playerBuffHandler.HasWeaknessBuff())
        {
            MPBarFill.color = Color.magenta;
        }
        else
        {
            MPBarFill.color = Color.blue;
        }
    }
}