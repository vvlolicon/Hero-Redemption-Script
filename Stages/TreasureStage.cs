using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TreasureStage : StageSettings
{
    [SerializeField] List<LootBox> lootBoxes = new List<LootBox>();
    private void Start()
    {
        
    }

    public override void OnEnterStage()
    {
        base.OnEnterStage();
        ClearStage();
    }
}
