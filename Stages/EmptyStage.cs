using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EmptyStage : StageSettings
{
    private void Start()
    {
    }

    public override void OnEnterStage()
    {
        base.OnEnterStage();
        ExitPointCollider.enabled = true;
        ClearStage();
    }
}
