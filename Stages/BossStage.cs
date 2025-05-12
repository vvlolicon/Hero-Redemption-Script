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
    [SerializeField] Transform returnPoint;

    bool isTriggeredBoss = false;

    UI_Controller UIController { get { return UI_Controller.Instance; } }

    private void Start()
    {
        _isFinalStage = true;
        _enterTriggerCollider.GetComponent<EnterColliderAction>().OnEnterAction += OnEnterBossArea;
        bossExecutor = boss.GetComponent<EnemyStateExecutor>();
        bossExecutor.Agent.enabled = false;
        bossExecutor.SetupMonster(OnBossDying, true);
    }

    //private void OnDestroy()
    //{
    //    _enterTriggerCollider.GetComponent<EnterColliderAction>().OnEnterAction -= OnEnterBossArea;
    //}

    void OnBossDying()
    {
        OnStageClear();
        PlayerCompManager.TryGetPlayerComp<PlayerBackpack>().PlayerLevel++;
        UIController.PopMessage("Congrads! Your level is increased by 1!");
        StartCoroutine(ExtendIEnumerator.DelayAction(5f, () =>
        {
            PlayerInputData input = PlayerInputData.Instance;
            UIController.CloseAllClosableWindows();
            input.EnableAllInput(false);
            input.LockAllInput = true;
            UIController.SetUIActive(UI_Window.WinUI, true);
        }));
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
        //Debug.Log("Setting boss stage clear");
        _airWall.enabled = false;
        _enterTriggerCollider.enabled = false;
        _enterTriggerCollider.GetComponent<EnterColliderAction>().OnEnterAction -= OnEnterBossArea;
        bossExecutor.bossActivated = false;
        _airWall.enabled = false;
        returnPoint.gameObject.SetActive(true);
        if (!isTriggeredBoss)
        {
            boss.SetActive(false);
            UIController.SetUIActive(UI_Window.BossHUD, false);
        }
    }

    void OnEnterBossArea(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isTriggeredBoss = true;
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