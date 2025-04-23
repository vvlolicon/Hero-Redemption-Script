using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnStage : StageSettings
{
    void OnEnable()
    {
        ClearStage();
        _isLocked = false;
        _isRootStage = true;
        ExitPointCollider.enabled = true;
    }
}
