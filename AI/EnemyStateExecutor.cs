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
        OriginStats.InitializeStats();
        CombatStats.SetStats(OriginStats.GetCombatStats());
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
        // reset stats everytime it enable(for placed Monster)
        initailizeStats();
    }

    void initailizeStats()
    {
        IsInvincible = false;
        IsDying = false;
        chasePlayerForever = false;
        IsHit = false;
        CombatStats.HP = CombatStats.MaxHP;
        Agent.speed = CombatStats.Speed / 10;
        WaitTimer = 0;
        OriginStats.InitializeStats();
        CombatStats.SetStats(OriginStats.GetCombatStats());
    }

    void Start()
    {
        _methods = new AIMethods(this);
        _defaultStrategy.BuildBehaviorTree(this, AIMethods);
        //_healthManager.Initialize();
    }

    public void SetupMonster(OnMonsterDeathEvent e, bool IsPlacedMonster)
    {
        OnMonsterDeath += e;
        initailizeStats();
        if (IsPlacedMonster)
        {
            InitializeMonster();
        }
    }

    public void ResetMonster()
    {
        initailizeStats();
        InitializeMonster();
    }

    void InitializeMonster()
    {
        Agent.isStopped = false;
        Agent.ResetPath();
        IsDying = false;
        gameObject.SetActive(true);
        transform.position = _enemyStaticStatScript.PatrolPoints[0].position;
        OnRestartPatrolRequested?.Invoke();
    }

    //void Update()
    //{
    //}

    public void ApplyDamage(DmgInfo data)
    {
        if (!IsInvincible && data is EnemyDmgInfo)
        {
            EnemyDmgInfo info = (EnemyDmgInfo)data;
            if (!IsInvincible && info.Target == this.gameObject)
            {
                _defaultStrategy.OnHit();
                StartCoroutine(ExtendIEnumerator.DelayAction(0.2f, () =>
                {
                    IsInvincible = false;
                }));
                if (!_billboard.activeSelf)
                    _billboard.SetActive(true);
                
                //CurState.SwitchState(_stateMan.Hit());
                DmgResult dmgResult = HealthManager.calculateDamage(
                    CombatStats.ATK, CombatStats.DEF, info.CritChance, CombatStats.CritChanRdc,
                    CombatStats.DmgReduce, info.CritMult, CombatStats.CritDmgResis);
                int dmgShow = (int)Mathf.Floor(dmgResult.Dmg);
                _healthManager.CreateHealthMeg(new EnemyDmgInfo(dmgShow, dmgResult.IsCritHit, info.TextColor, DmgTextPos, gameObject));
                _healthManager.Damage(dmgResult.Dmg);
                OnStatsChanged?.Invoke();
                SendBuffToSelf();
            }
        }
    }

    public void DamagePlayer()
    {
        PlayerDmgInfo dmgInfo = new PlayerDmgInfo(
            CombatStats.ATK, CombatStats.CritChance, CombatStats.CritDmgMult, 
            Player.position - transform.position, 250f);
        dmgInfo.CallDamageable(Player.gameObject);
        SendBuffToPlayer(Player.gameObject);
    }

    public void DamagePlayer(Vector3 impact)
    {
        PlayerDmgInfo dmgInfo = new PlayerDmgInfo(
            CombatStats.ATK, CombatStats.CritChance, CombatStats.CritDmgMult, impact, 250f);
        dmgInfo.CallDamageable(Player.gameObject);
        SendBuffToPlayer(Player.gameObject);
    }

    void SendBuffToPlayer(GameObject player)
    {
        BuffSender[] senders = GetComponents<BuffSender>();
        if (senders.Length == 0) return;
        foreach (var sender in senders)
        {
            if (sender.howToAttachBuff == AttachBuffWays.Damage)
            {
                sender.SendBuff(player);
            }
        }
    }

    void SendBuffToSelf()
    {
        BuffSender[] senders = GetComponents<BuffSender>();
        if (senders.Length == 0) return;
        foreach (var sender in senders)
        {
            if (sender.howToAttachBuff == AttachBuffWays.Self)
            {
                sender.SendBuffToSelf();
            }
        }
    }

    public void OnDying()
    {
        Debug.Log($"{gameObject.name} dies");
        SoundManager.PlaySound("Dying");
        _methods.ResetAllAnimationTriggers();
        Agent.isStopped = true;
        IsDying = true;
        OnMonsterDeath?.Invoke();
        OnMonsterDeath = null;
        _billboard.SetActive(false);
        _anim.Play("Dying");
        StartCoroutine(ExtendIEnumerator.DelayAction(
            _enemyStaticStatScript._stats._dieTime, () => { 
                AfterDying();
        }));
        if (dropData != null)
        {
            PlayerBackpack backpack = GameObjectManager.TryGetPlayerComp<PlayerBackpack>();
            backpack.AddEnemyDrops(dropData);
        }
    }

    void AfterDying()
    {
        SoundManager.Mute(true);
        gameObject.SetActive(false);
    }

    NavMeshAgent _agent;
    Animator _anim;
    SoundManager _soundMan;
    AnimatorEventsEn _animEv;
    Transform _player;
    EnemyStaticStatsMono _enemyStaticStatScript;
    GameObject _billboard;
    AIMethods _methods;
    public GeneralStatsObj OriginStats;
    public GeneralCombatStats CombatStats = new();
    [SerializeField] EnemyDropData dropData;

    HealthManager _healthManager;
    AI_DefaultStrategy _defaultStrategy;

    [HideInInspector]
    public event OnMonsterDeathEvent OnMonsterDeath;
    [HideInInspector]
    public delegate void OnMonsterDeathEvent();
    [HideInInspector]
    public bool IsPlacedMonster;
    [HideInInspector]
    public float MoveSpeed;
    public event CombatBuffHandler.OnStatsChangedDelegate OnStatsChanged;
    public event Action OnRestartPatrolRequested;

    // getters and setters
    public AIMethods AIMethods { get { return _methods; } }
    public NavMeshAgent Agent { get { return _agent; } }
    public Animator Animator { get { return _anim; } }
    public SoundManager SoundManager { get { return _soundMan; } }
    public AnimatorEventsEn AnimatorEvents { get { return _animEv; } }
    public Transform Player { get { return _player; } }

    [HideInInspector] public bool chasePlayerForever = false;

    public bool IsInvincible { get; set; }
    public bool IsHit { get; set; }
    public bool IsDying { get; private set; }

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
