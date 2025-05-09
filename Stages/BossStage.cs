using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class BossStage : MonsterStage
{
    public GameObject boss;
    EnemyStateExecutor bossExecutor;
    [SerializeField] string bossName;
    [SerializeField] Collider _airWall;
    [SerializeField] Collider _enterTriggerCollider;

    UI_Controller UIController { get { return UI_Controller.Instance; } }

    private void Start()
    {
        _isFinalStage = true;
        _enterTriggerCollider.GetComponent<EnterColliderAction>().OnEnterAction += OnEnterBossArea;
        bossExecutor = boss.GetComponent<EnemyStateExecutor>();
        bossExecutor.Agent.enabled = false;
    }

    private void OnDestroy()
    {
        _enterTriggerCollider.GetComponent<EnterColliderAction>().OnEnterAction -= OnEnterBossArea;
    }

    public override void OnEnterStage()
    {
        base.OnEnterStage();
        if (!_stageCleared)
        {
            bossExecutor.SetupMonster(OnStageClear, true);
            bossExecutor.Agent.enabled = false;
        }
    }

    public override void OnStageClear()
    {
        base.OnStageClear();
        _airWall.enabled = false;
        bossExecutor.bossActivated = false;
        UIController.SetUIActive(UI_Window.BossHUD, false);
    }

    void OnEnterBossArea(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _enterTriggerCollider.GetComponent<EnterColliderAction>().OnEnterAction -= OnEnterBossArea;
            bossExecutor.bossActivated = true;
            bossExecutor.Agent.enabled = true;
            _enterTriggerCollider.enabled = false;
            _airWall.enabled = true;
            UIController.SetUIActive(UI_Window.BossHUD, true);
            UI_Controller.GetUIScript<BossHudControl>().SetBoss(bossName, bossExecutor.CombatStats);
            bossExecutor.SoundManager.PlayExtraSound("Roar", true);
        }
    }
}