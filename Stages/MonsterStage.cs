using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterStage : StageSettings
{
    [SerializeField] List<GameObject> PlacedMonsters;

    List<EnemyStateExecutor> MonsterExecutors = new List<EnemyStateExecutor>();

    void Start()
    {
        _isRootStage = false;
        StartCoroutine(ExtendIEnumerator.DelayAction(0.1f, () =>
        {
            foreach (var monster in PlacedMonsters)
            {
                var monsterExecutor = monster.GetComponent<EnemyStateExecutor>();
                MonsterExecutors.Add(monsterExecutor);
                monsterExecutor.ResetMonster();
                monsterExecutor.SetupMonster(CheckClearStage, true);
                monster.SetActive(false);
            }
        }));
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
            foreach (var monster in MonsterExecutors)
            {
                monster.gameObject.SetActive(false);
            }
            return;
        }
        foreach(var monster in MonsterExecutors)
        {
            monster.gameObject.SetActive(true);
            monster.ResetMonster();
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
    }

    void CheckClearStage()
    {
        bool hasMonsterAlive = false;
        foreach (var monster in MonsterExecutors)
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

    public override void SetStage(bool b1, bool b2, bool isCleared, List<bool> lb)
    {
        if (isCleared)
        {
            foreach (var monster in PlacedMonsters)
            {
                monster.SetActive(false);
            }
        }
        base.SetStage(b1, b2, isCleared, lb);
    }
}
