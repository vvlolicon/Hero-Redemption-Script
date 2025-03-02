using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Rendering.InspectorCurveEditor;

public class EnemyStateExecutor : MonoBehaviour, IDamageable
{
    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponentInChildren<Animator>();
        _animEv = GetComponentInChildren<AnimatorEventsEn>();
        _soundMan = GetComponent<SoundManager>();
        _enemyStaticStats = GetComponent<EnemyStaticStatsMono>();
        _healthManager = GetComponent<HealthManager>();
        _healthManager._enemyExecutor = this;
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
        
    }

    void OnEnable()
    {
        // reset stats everytime it enable
        initailizeStats();
        
    }

    void initailizeStats()
    {
        IsInvincible = false;
        ATK = _enemyStats.ATK;
        DEF = _enemyStats.DEF;
        Speed = _enemyStats.SPEED / 10f;
        MaxHP = _enemyStats.MaxHP;
        HP = MaxHP;

        Agent.speed = Speed;
        if(CurState!= null) {
            CurState.EnterState();
            if (CurState.SubState.CurStateType() == AIStates.PATROL)
            {
                Agent.SetDestination(PatrolPoints[0].transform.position);
            }
        }
    }

    void Start()
    {
        _stateMan = new EnemyStateManager(this);
        CurState = _stateMan.Ground();
        initailizeStats();
        //healthManager.Initialize();
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
                if(!_billboard.activeSelf)
                    _billboard.SetActive(true);
                IsInvincible = true;
                CurState.SwitchState(_stateMan.Hit());
                DmgResult dmgResult = HealthManager.calculateDamage(info.ATK, _enemyStats.DEF, info.CritChance, _enemyStats.CritChanRdc, DmgReduc, info.CritMult, _enemyStats.CritDmgResis);
                int dmgShow = (int)Mathf.Floor(dmgResult.Dmg);
                _healthManager.createHealthMeg(new EnemyDmgInfo(dmgShow, dmgResult.IsCritHit, info.TextColor, DmgTextPos, gameObject));
                _healthManager.Damage(dmgResult.Dmg);
            }
        }
    }

    public void OnDying()
    {
        transform.position = _enemyStaticStats.PatrolPoints[0].transform.position;
        CurState.SwitchState(_stateMan.Ground());
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
    EnemyStaticStatsMono _enemyStaticStats;
    public GameObject _billboard;

    //public TMP_Text test_showState;
    public GeneralStatsObj _enemyStats;

    HealthManager _healthManager;

    // getters and setters
    public EnemyBaseStates CurState { get; set; }
    public NavMeshAgent Agent { get { return _agent; } }
    public Animator Animator { get { return _anim; } }
    public SoundManager SoundManager { get { return _soundMan; } }
    public AnimatorEventsEn AnimatorEvents { get { return _animEv; } }
    public Transform Player { get { return _player; } }
    public bool IsInvincible { get; set; }

    // the stats below will change for different enemy individual,
    // so it cannot get the stats from static object directly, it needs to store locally
    public float HP { get; set; }
    public float MaxHP { get; set; }
    public float Speed { get; set; }
    public float ATK { get; set; }
    public float DEF { get; set; }
    // these stats below will not change for same enemy type, so it can get directly from Script Object
    public float DmgReduc { get { return _enemyStats.DmgReduction; } }
    public float CritChance { get { return _enemyStats.CritChance; } }
    public float CritResis { get { return _enemyStats.CritDmgResis; } }
    public float CritMult { get { return _enemyStats.CritDmgMult; } }

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
