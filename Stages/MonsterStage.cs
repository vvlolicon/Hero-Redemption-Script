using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterStage : StageSettings
{
    [SerializeField] List<GameObject> PlacedMonsters;

    List<EnemyStateExecutor> Monsters = new List<EnemyStateExecutor>();

    
    void OnEnable()
    {
        _isRootStage = false;
    }

    public override void OnStageClear()
    { // player can interact with the door
       base.OnStageClear();
    }

    public override void OnEnterStage()
    {
        base.OnEnterStage();
        if (_stageCleared)
        {
            foreach (var monster in PlacedMonsters)
            {
                monster.SetActive(false);
            }
            return;
        }
        //if (MonsterSpawnPoints.Count > 0 && MonstersPrefab.Count > 0)
        //{
        //    for (int i = 0; i < MonsterSpawnPoints.Count; i++)
        //    {
        //        var monster = Instantiate(MonstersPrefab[i], MonsterSpawnPoints[i].position, Quaternion.identity);
        //        var monsterExecutor = monster.GetComponent<EnemyStateExecutor>();
        //        Monsters.Add(monsterExecutor);
        //        monsterExecutor.SetupMonster(CheckClearStage, false);
        //    }
        //}
        foreach (var monster in PlacedMonsters)
        {
            var monsterExecutor = monster.GetComponent<EnemyStateExecutor>();
            if (Monsters.Contains(monsterExecutor))
            {
                monsterExecutor.ResetMonster();
                continue;
            }
            Monsters.Add(monsterExecutor);
            monsterExecutor.SetupMonster(CheckClearStage, true);
        }
    }

    void CheckClearStage()
    {
        bool hasMonsterAlive = false;
        foreach (var monster in Monsters)
        {
            if (!monster.IsDying)
            {
                hasMonsterAlive = true;
                break;
            }
        }
        if (!hasMonsterAlive)
        {
            ClearStage();
        }
    }

    protected override void ResetStage()
    {
        base.ResetStage();
        //foreach (var monster in Monsters)
        //{
        //    Destroy(monster.gameObject);
        //}
    }
}
