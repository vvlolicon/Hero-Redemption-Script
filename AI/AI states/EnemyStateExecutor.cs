using Assets.AI.BehaviourTree;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyStateExecutor : MonoBehaviour, IDamageable
{
    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponentInChildren<Animator>();
        _animEv = GetComponentInChildren<AnimatorEventsEn>();
        _soundMan = GetComponent<SoundManager>();
        _enemyStaticStatScript = GetComponent<EnemyStaticStatsMono>();
        _healthManager = GetComponent<HealthManager>();
        _defaultStrategy = GetComponent<AI_DefaultStrategy>();
        _healthManager._enemyExecutor = this;
        for (int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);
            if (child.CompareTag("MonsterCanvas"))
            {
                _billboard = transform.GetChild(i).gameObject;
                break;
            }
        }
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.activeSelf)
            {
                _player = player.transform;
                break;
            }
        }

        WaitTimer = 0;
        
    }

    void OnEnable()
    {
        // reset stats everytime it enable
        initailizeStats();
    }

    void initailizeStats()
    {
        IsInvincible = false;
        IsDying = false;
        chasePlayerForever = false;
        IsHit = false;

        ATK = _enemyStats.ATK;
        DEF = _enemyStats.DEF;
        Speed = _enemyStats.SPEED / 10f;
        MaxHP = _enemyStats.MaxHP;
        HP = MaxHP;
        Agent.speed = Speed;
        //if(CurState!= null) {
        //    CurState.EnterState();
        //    if (CurState.SubState.CurStateType() == AIStates.PATROL)
        //    {
        //        Agent.SetDestination(PatrolPoints[0].position);
        //    }
        //}
    }

    void Start()
    {
        _stateMan = new EnemyStateManager(this);
        //CurState = _stateMan.Ground();
        initailizeStats();
        _methods = new AIMethods(this);
        _defaultStrategy.BuildBehaviorTree(this, AIMethods);
        //_healthManager.Initialize();
    }

    void Update()
    {
        //if (test_showState != null && test_showData != null) { 
        //    test_showState.text = "Current enemy State:" + CurState.CurStateType();
        //    test_showData.text = "";
        //    if (CurState.SubState != null)
        //    {
        //        test_showState.text += "\n" + "current enemy Sub State: " + CurState.SubState.CurStateType();
        //        test_showData.text += "\n" + "enemy speed: " + Speed;
        //    }
        //}
        //if (CurState != null)
        //{
        //    CurState.UpdateStates();
        //}

    }

    public void ApplyDamage(DmgInfo data)
    {
        if (!IsInvincible && data is EnemyDmgInfo)
        {
            EnemyDmgInfo info = (EnemyDmgInfo)data;
            if (!IsInvincible && info.Target == this.gameObject)
            {

                _defaultStrategy.OnHit();
                StartCoroutine(ExtendMethods.DelayAction(0.2f, () =>
                {
                    IsInvincible = false;
                }));
                if (!_billboard.activeSelf)
                    _billboard.SetActive(true);
                
                //CurState.SwitchState(_stateMan.Hit());
                DmgResult dmgResult = HealthManager.calculateDamage(info.ATK, _enemyStats.DEF, info.CritChance, _enemyStats.CritChanRdc, DmgReduc, info.CritMult, _enemyStats.CritDmgResis);
                int dmgShow = (int)Mathf.Floor(dmgResult.Dmg);
                _healthManager.createHealthMeg(new EnemyDmgInfo(dmgShow, dmgResult.IsCritHit, info.TextColor, DmgTextPos, gameObject));
                _healthManager.Damage(dmgResult.Dmg);
            }
        }
    }

    public void DamagePlayer(Vector3 impact)
    {
        PlayerDmgInfo dmgInfo = new PlayerDmgInfo(ATK, impact, 250f);
        dmgInfo.CallDamageable(Player.gameObject);
    }

    public void DamagePlayer()
    {
        PlayerDmgInfo dmgInfo = new PlayerDmgInfo(ATK, Player.position - transform.position, 250f);
        dmgInfo.CallDamageable(Player.gameObject);
    }

    public void OnDying()
    {
        _methods.ResetAllAnimationTriggers();
        Agent.isStopped = true;
        IsDying = true;
        StartCoroutine(ExtendMethods.DelayAction(
            _enemyStaticStatScript._stats._dieTime, () => { AfterDying(); }));
       
    }

    void AfterDying()
    {
        Agent.isStopped = false;
        IsDying = false;
        transform.position = _enemyStaticStatScript.PatrolPoints[0].position;
        //CurState.SwitchState(_stateMan.Ground());
        gameObject.SetActive(false);
        _billboard.SetActive(false);
        // TODO: give exp and money to player and shows message in game
        // TODO: drop item in drop table

        gameObject.SetActive(true);
    }

    NavMeshAgent _agent;
    Animator _anim;
    SoundManager _soundMan;
    AnimatorEventsEn _animEv;
    Transform _player;
    EnemyStateManager _stateMan;
    EnemyStaticStatsMono _enemyStaticStatScript;
    GameObject _billboard;
    AIMethods _methods;
    [HideInInspector]
    public TMP_Text test_showState;
    [HideInInspector]
    public TMP_Text test_showData;
    public GeneralStatsObj _enemyStats;

    HealthManager _healthManager;
    AI_DefaultStrategy _defaultStrategy;

    // getters and setters
    public AIMethods AIMethods { get { return _methods; } }
    public EnemyBaseStates CurState { get; set; }
    public NavMeshAgent Agent { get { return _agent; } }
    public Animator Animator { get { return _anim; } }
    public SoundManager SoundManager { get { return _soundMan; } }
    public AnimatorEventsEn AnimatorEvents { get { return _animEv; } }
    public Transform Player { get { return _player; } }

    [HideInInspector] public bool chasePlayerForever = false;

    // the stats below will change for different enemy individual,
    // so it cannot get the stats from static object directly, it needs to store locally
    public float HP { get; set; }
    public float MaxHP { get; set; }
    public float Speed { get; set; }
    public float ATK { get; set; }
    public float DEF { get; set; }
    public bool IsInvincible { get; set; }
    public bool IsHit { get; set; }
    public bool IsDying { get; private set; }
    // these stats below will not change for same enemy type, so it can get directly from Script Object
    public float DmgReduc { get { return _enemyStats.DmgReduce; } }
    public float CritChance { get { return _enemyStats.CritChance; } }
    public float CritResis { get { return _enemyStats.CritDmgResis; } }
    public float CritMult { get { return _enemyStats.CritDmgMult; } }
    public float AttackTime { get { return _enemyStats.AttackTime; } }

    public float ChaseTime { get { return _enemyStaticStatScript._stats._chaseTime; } }
    public float VisDist { get { return _enemyStaticStatScript._stats._visDist; } }
    public float VisAngle { get { return _enemyStaticStatScript._stats._visAngle; } }
    public float AttackDist { get { return _enemyStaticStatScript._stats._attackDist; } }
    public float PreAtkTime { get { return _enemyStaticStatScript._stats._preAtkTime; } }
    public float AtkAnimTime { get { return _enemyStaticStatScript._stats._atkAnimTime; } }
    public float HitAnimTime { get { return _enemyStaticStatScript._stats._hitAnimTime; } }
    public bool Interruptable { get { return _enemyStaticStatScript._stats.Interruptable; } }
    public float WaitTimer { get; set; }
    public List<Transform> PatrolPoints { get { return _enemyStaticStatScript.PatrolPoints; } }
    public Transform DmgTextPos { get { return _enemyStaticStatScript.DmgPos; } }

}
