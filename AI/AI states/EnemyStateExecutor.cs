using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateExecutor : MonoBehaviour, IDamageable
{
    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponentInChildren<Animator>();
        _animEv = GetComponentInChildren<AnimatorEventsEn>();
        _soundMan = GetComponent<SoundManager>();
        _enemyStaticStats = GetComponent<EnemyStaticStatsMono>();
        GameObject[] players;
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.activeSelf)
            {
                _player = player.transform;
                break;
            }
        }

        WaitTimer = 0;
        setUpStats();
    }

    void setUpStats()
    {
        ATK = _enemyStats.ATK;
        DEF = _enemyStats.DEF;
        Speed = _enemyStats.SPEED / 10f;

        Agent.speed = Speed;
    }

    void Start()
    {
        _stateMan = new EnemyStateManager(this);
        CurState = _stateMan.Ground();
        CurState.EnterState();
        if (CurState.SubState.CurStateType() == AIStates.PATROL)
        {
            Agent.SetDestination(PatrolPoints[0].transform.position);
        }
        healthManager.Initialize();
    }

    void Update()
    {

        //test_showState.text = "Current enemy State:" + CurState.CurStateType();
        //if (CurState.SubState != null)
        //{
        //    test_showState.text += "\n" + "current enemy Sub State: " + CurState.SubState.CurStateType();
        //    if (CurState.SubState.CurStateType() == AIStates.PATROL)
        //    {
        //        EnemyStatePatrol partolState = (EnemyStatePatrol)CurState.SubState;
        //        test_showState.text += "\n" + "curPatrolIndex: " + partolState.curPatrolIndex;
        //    }
        //    test_showState.text += "\n" + "enemy speed: " + Speed;
        //}
        
        CurState.UpdateStates();
    }

    public void ApplyDamage(DmgInfo data)
    {
        if (!IsInvincible && data is EnemyDmgInfo)
        {
            EnemyDmgInfo info = (EnemyDmgInfo)data;
            if (!IsInvincible && info.Target == this.gameObject)
            {
                Debug.Log("triggered ApplyDmg to enemy");
                IsInvincible = true;
                CurState.SwitchState(_stateMan.Hit());
                DmgResult dmgResult = HealthManager.calculateDamage(info.ATK, _enemyStats.DEF, info.CritChance, DmgReduc, info.CritMult, _enemyStats.CritResis);
                int dmgShow = (int)Mathf.Floor(dmgResult.Dmg);
                healthManager.createHealthMeg(new EnemyDmgInfo(dmgShow, dmgResult.IsCritHit, info.TextColor, DmgTextPos, gameObject));
            }
        }
    }

    NavMeshAgent _agent;
    Animator _anim;
    SoundManager _soundMan;
    AnimatorEventsEn _animEv;
    Transform _player;
    EnemyStateManager _stateMan;
    EnemyStaticStatsMono _enemyStaticStats;

    //public TMP_Text test_showState;
    public GeneralStatsObj _enemyStats;

    [Header("Manager")] 
    public HealthManager healthManager;

    // getters and setters
    public EnemyBaseStates CurState { get; set; }
    public NavMeshAgent Agent { get { return _agent; } }
    public Animator Animator { get { return _anim; } }
    public SoundManager SoundManager { get { return _soundMan; } }
    public AnimatorEventsEn AnimatorEvents { get { return _animEv; } }
    public Transform Player { get { return _player; } }
    public bool IsInvincible { get; set; }
    public float Speed { get; set; }

    public float ATK { get; set; }
    public float DEF { get; set; }
    public float DmgReduc { get { return _enemyStats.DmgReduction; } }
    public float CritChance { get { return _enemyStats.CritChance; } }
    public float CritResis { get { return _enemyStats.CritResis; } }
    public float CritMult { get { return _enemyStats.CritMult; } }

    public float ChaseTime { get { return _enemyStaticStats.ChaseTime; } }
    public float VisDist { get { return _enemyStaticStats.VisDist; } }
    public float VisAngle { get { return _enemyStaticStats.VisAngle; } }
    public float AttackDist { get { return _enemyStaticStats.AttackDist; } }
    public float AttackTime { get { return _enemyStats.AttackTime; } }
    public float AtkAnimTime { get { return _enemyStaticStats.AtkAnimTime; } }
    public float WaitTimer { get; set; }
    public List<GameObject> PatrolPoints { get { return _enemyStaticStats.PatrolPoints; } }
    public Transform DmgTextPos { get { return _enemyStaticStats.DmgPos; } }

}
