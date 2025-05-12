

using Unity.VisualScripting;
using UnityEngine;

public class EventStage : StageSettings
{
    [SerializeField] Collider triggerCollider;

    private void Awake()
    {
        var colliderTrigger = triggerCollider.GetOrAddComponent<EnterColliderAction>();
        colliderTrigger.OnEnterAction += OnEnterTriggerCollider;
    }

    void OnEnterTriggerCollider(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnStageClear();
            UI_Controller.Instance.PopMessage("You have been rewarded 200$ for clearing this stage!");
            PlayerCompManager.TryGetPlayerComp<PlayerBackpack>().PlayerOwnedMoney += 200;
        }
    }

    public override void OnStageClear()
    {
        base.OnStageClear();
        triggerCollider.GetComponent<EnterColliderAction>().OnEnterAction -= OnEnterTriggerCollider;
        triggerCollider.enabled = false;
    }
}