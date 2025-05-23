using Assets.SaveSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.SceneManagement;
using static BuffSender;

public class PlayerStateExecutor : MonoBehaviour, IDamageable, IPickItem
{
    void Awake()
    {
        _charCont = GetComponent<CharacterController>();
        _soundMan = GetComponent<SoundManager>();
        _animator = GetComponentInChildren<Animator>();
        _statDisplay = FindFirstObjectByType<PlayerStatDisplay>();
        _healthMan = GetComponent<HealthManager>();
        DistToGround = CharController.bounds.extents.y;
        AnimEvent = ChildPlayer.GetComponent<AnimatorEvents>();
        _backpack = GetComponent<PlayerBackpack>();
        PlayerOriginStats.InitializeStats();
        PlayerCombatStats.SetStats(PlayerOriginStats.GetCombatStats());

        if (SceneManager.GetActiveScene() == gameObject.scene)
        {
            ResetPlayerValue(); // load if current scene is dungeon
        }

        //_playerInput = new PlayerInputMethods();
        //_playerInput.PlayerControl.Move.started += OnMove;
        //_playerInput.PlayerControl.Move.performed += OnMove;
        //_playerInput.PlayerControl.Move.canceled += OnMove;
    }

    public void InitializeCombatStats()
    {
        PlayerCombatStats.HP = PlayerCombatStats.MaxHP;
        PlayerCombatStats.MP = PlayerCombatStats.MaxMP;
        OnStatsChanged?.Invoke();
        HasInitialized = true;
    }

    public void InitializePlayer()
    {
        TransportPlayerTo(_spawnPoint);
        ResetPlayerValue();
        _playerInput.Initialize();
        _backpack.PlayerLevel = 1;
        //_backpack.PlayerOwnedMoney = 0;
    }

    public void LoadPlayerData(PlayerData newPlayerData)
    {
        LoadPlayerDataNoTransport(newPlayerData);
        TransportPlayerTo(newPlayerData.playerPos);
    }

    public void LoadPlayerDataNoTransport(PlayerData newPlayerData)
    {
        PlayerCombatStats.SetStats(newPlayerData.combatStats);
        ExtraStats.SetStats(newPlayerData.extraStats);
        HasInitialized = true;
        ResetPlayerValue();
        _playerInput.Initialize();

        OnStatsChanged?.Invoke();
        _backpack.PlayerLevel = newPlayerData.playerLevel;
        _backpack.PlayerOwnedMoney = newPlayerData.playerMoney;
        _backpack.SetPlayerInventoryRange(newPlayerData.playerInventory);
        _backpack.SetPlayerEquippedItemsRange(newPlayerData.playerEquipment);
        _backpack.SetPlayerHotbarItemsRange(newPlayerData.playerHotbar);

    }

    void ResetPlayerValue()
    {
        IsHit = false;
        JumpPressed = false;
        MovePressed = false;
        RunPressed = false;
        AttackPressed = false;
        MovementY = 0;
        WasGrounded = false;
        Attacking = false;

        Impact = Vector3.zero;
        InputMoveXZ = Vector2.zero;
        CanJump = true;
        CanMove = true;
        CanRun = true;
        AttackTimer = 0;

        RunSpeedMult = PlayerStaticData._runSpeedMult;
        AtkSpeedMult = PlayerStaticData._atkSpeedMult;
        JumpSpeed = PlayerStaticData._jumpSpeed;
        Gravity = PlayerStaticData._gravity;
        GroundedGravity = PlayerStaticData._groundedGravity;
        Mass = PlayerStaticData._mass;
        MaxDashTime = PlayerStaticData._maxDashTime;
        DashSpeed = PlayerStaticData._dashSpeed;

        _currentDashTime = MaxDashTime;
    }
    void Start()
    {
        ChildPlayeriniPos = ChildPlayer.transform.localPosition;
        _stateMan = new PlayerStatsManager(this);
        CurState = _stateMan.Grounded();
        CurState.EnterState();
    }


    // Update is called once per frame
    void Update()
    {
        if (_playerDying) return;
        PlayerSpeed = PlayerCombatStats.Speed / 10;
        CurState.UpdateStates();
        //testove the controller
        if(!TransportPlayer)
            CharController.Move(CurMovement * Time.deltaTime);
        _atkTimeCD = PlayerStaticData._iniAtkSpeed * (1 / GetAtkSpeedMult());
        if (AttackTimer > 0)
        {
            AttackTimer -= Time.deltaTime;
            //Debug.Log("AttackTimer: " + AttackTimer + " / atkTimeCD: " + _atkTimeCD);
        }
        if (test_showState == null || test_MovementY == null) return;
        if (test_showState.gameObject.activeInHierarchy)
        {
            test_showState.text = "Current State:" + CurState.CurStateType();
            if (CurState.SubState != null)
                test_showState.text += "\n" + "current Sub State: " + CurState.SubState.CurStateType();
        }
        if (test_MovementY.gameObject.activeInHierarchy)
        {
            test_MovementY.text = "CanJump: " + CanJump + " , StartJump: " + StartJump + " , CanRun: " + CanRun + " , IsDashing: " + IsDashing()
           + "\n" + " , curDashTime: " + _currentDashTime + " , isGrounded: " + CharCont.isGrounded + ", MovePressed: " + MovePressed + ", PlayerSpeed: " + PlayerSpeed;
        }
        // M
    }

    public float GetAtkSpeedMult()
    {
        return Mathf.Min((1 + PlayerCombatStats.AttackTime), 10f);
    }

    void FixedUpdate()
    {
        if (_playerDying) return;
        // for every MP regen frequency(default 1s), recover player MP for value of MP_Regen
        // to ensure actual time frequency, use fixed update for this
        if (_mpRegenTimer > PlayerStaticData._mpRegenFreq)
        {
            // ensure MP does not exceed maximum MP
            PlayerCombatStats.MP = Mathf.Min(PlayerCombatStats.MaxMP, PlayerCombatStats.MP + PlayerCombatStats.MP_Regen);
            _mpRegenTimer = 0;
        } else
        {
            _mpRegenTimer += Time.deltaTime;
        }
    }
    #region player input events
    public void OnJumpPressed()
    {
        if (CanJump && !Attacking)
        {
            JumpPressed = true;
        }
    }
    public void OnRunPressed()
    {
        if (!Attacking && CanRun)
            RunPressed = !RunPressed;
        //test_runPress.text = "run pressed: " + _runPressed;
    }
    public void OnMovePressed(Vector2 value)
    {
        InputMoveXZ = value;
        MovePressed = InputMoveXZ.x != 0 || InputMoveXZ.y != 0;
    }

    public void OnOpenInventory()
    {
        InputMoveXZ = Vector2.zero;
        MovePressed = false;
    }

    public void OnAttackPressed(bool value)
    {
        if (CanMove && !Attacking && AttackTimer <= 0)
        {
            if (CharController.isGrounded && !IsDashing())
            {
                AttackTimer = _atkTimeCD;
                AttackPressed = value;
            }
        }
    }
#endregion
    public void ApplyDamage(DmgInfo data)
    {
        if (data!= null && data is PlayerDmgInfo)
        {
            PlayerDmgInfo info = (PlayerDmgInfo)data;
            if (!IsHit)
            {
                PlayerHitState hitState = (PlayerHitState)_stateMan.Hit();
                CurState.SwitchState(hitState);
                hitState.ApplyDamage(info);
            }
            // apply damage whether or not player is at hit state
            DmgResult dmgResult = HealthManager.calculateDamage(
                info.ATK, PlayerCombatStats.DEF, info.CritChance, PlayerCombatStats.CritChanRdc,
                PlayerCombatStats.DmgReduce, info.CritMult, PlayerCombatStats.CritDmgResis);
            _healthMan.Damage(dmgResult.Dmg);
            OnStatsChanged?.Invoke();
            //Debug.Log("You get damage: " + dmgResult.Dmg + " Is critial hit: " + dmgResult.IsCritHit);
        }
    }

    public void OnExternalStatChange() => OnStatsChanged?.Invoke();

    public void ChangePlayerCombatStat(List<ItemAttribute> attributes)
    {
        PlayerCombatStats.ChangePlayerStats(attributes);
        OnStatsChanged?.Invoke();
    }

    public void ChangePlayerExtraStat(List<ItemAttribute> attributes)
    {
        ExtraStats.ChangePlayerStats(attributes);
        OnStatsChanged?.Invoke();
    }

    public void DamageEnemy(GameObject victim, Color dmgColor)
    {
        EnemyDmgInfo dmgInfo = new EnemyDmgInfo(
                        PlayerCombatStats.ATK,
                        PlayerCombatStats.CritChance,
                        PlayerCombatStats.CritDmgMult,
                        dmgColor, transform, victim);
        dmgInfo.CallDamageable(victim);
        SendBuffToTarget(victim);
    }

    void SendBuffToTarget(GameObject victim)
    {
        BuffSender[] senders = GetComponents<BuffSender>();
        if (senders.Length == 0) return;
        foreach (var sender in senders)
        {
            if (sender.howToAttachBuff == AttachBuffWays.Damage)
            {
                sender.SendBuff(victim);
            }
        }
    }
    public void EnableMove(bool camMoveT) //Enables or disables the character movement
    {
        if (!IsHit)
            CanMove = camMoveT;
    }

    public void OnPickItem(List<ItemAttribute> itemAttributes, Action callback)
    {
        ExtraStats.ChangePlayerStats(itemAttributes);
        OnStatsChanged?.Invoke();
        StringBuilder sb = new StringBuilder("Power shard picked: ");
        foreach (var attr in itemAttributes)
        {
            sb.Append($"{attr.AtrbName} +{attr.AtrbValue}, ");
        }
        UI_Controller.Instance.PopMessage(sb.ToString().Substring(0, sb.Length - 2));

        callback();
    }

    public void OnLanding()
    {
        _playerInput.InputEnable = false;
        StartCoroutine(ExtendIEnumerator.DelayAction(0.7f, () => {
            _playerInput.InputEnable = true;
            Animator.SetTrigger("Exit");
        }));
    }

    public void OnDying()
    {
        Animator.SetTrigger("Exit");
        Animator.Play("Dying");
        _playerDying = true;
        UI_Controller uiContr = UI_Controller.Instance;
        uiContr.CloseAllClosableWindows();
        StartCoroutine(ExtendIEnumerator.DelayAction(2.1f, () => {
            _childPlayer.SetActive(false);
            _playerInput.EnableAllInput(false);
            _playerInput.LockAllInput = true;
            uiContr.SetUIActive(UI_Window.LoseUI, true);
            uiContr.SetUIActive(UI_Window.BossHUD, false);
        }));

    }

    public void TransportPlayerTo(Transform newPos)
    {
        transform.rotation = Quaternion.Euler(0, newPos.rotation.eulerAngles.y, 0);
        GetComponent<CameraControl>().ResetCameraTarget();
        TransportPlayerTo(newPos.position);
    }

    public void TransportPlayerTo(Vector3 newPos)
    {
        transform.position = newPos;
        TransportPlayer = true;
        CharController.enabled = false;
        StartCoroutine(ExtendIEnumerator.ActionInNextFrame(() =>
        {
            TransportPlayer = false;
            CharController.enabled = true;
        }));
    }

    //public void OnEnable()
    //{
    //    _playerInput.PlayerControl.Enable();
    //}

    public GameObject _childPlayer;
    [SerializeField] Camera _camera;
    [SerializeField] GameObject _movIndicator; //Where is the character moving to
    public PlayerStaticData PlayerStaticData;
    public Transform _rangeAtkAim;
    [SerializeField] Transform _spawnPoint;

    float _currentDashTime;
    Vector3 _dashDir;
    Vector3 _groundNormal;
    float _mpRegenTimer;
    float _atkTimeCD;
    bool TransportPlayer = false;
    [HideInInspector] public bool _playerDying = false;
    [HideInInspector] public bool HasInitialized = false;

    CharacterController _charCont;
    Animator _animator;
    SoundManager _soundMan;
    PlayerInputData _playerInput { get{ return PlayerInputData.Instance; } }
    PlayerBackpack _backpack;
    HealthManager _healthMan;

    //states variables
    PlayerStatsManager _stateMan;
    PlayerStatDisplay _statDisplay;

    //test
    public TMP_Text test_showState;
    public TMP_Text test_MovementY;

    public GeneralStatsObj PlayerOriginStats;
    public GeneralCombatStats ExtraStats = new();
    public GeneralCombatStats PlayerCombatStats = new();
    public event CombatBuffHandler.OnStatsChangedDelegate OnStatsChanged;


    //getter and setter
    public bool StartJump { get; set; }
    public float PlayerSpeed { get; private set; }//Base Speed of the player
    public float RunSpeedMult { get; private set; }
    public float AtkSpeedMult { get; private set; }
    public float Gravity { get; private set; }
    public float GroundedGravity { get; private set; }
    public float JumpSpeed { get; private set; }
    public float Mass { get; private set; }
    public float MaxDashTime { get; private set; }
    public float DashSpeed { get; private set; }

    public PlayerBaseState CurState { get; set; }
    public bool JumpPressed { get; set; }
    public bool MovePressed { get; set; }
    public bool RunPressed { get; set; }
    public bool AttackPressed { get; set; }
    public Vector3 CurMovement { get; set; }
    public Vector2 InputMoveXZ { get; private set; }
    public float MovementY { get; set; }
    public float CurPlayerSpeed { get; set; }//current Speed (including multiply of running / attacking)
    public Vector3 Impact { get; set; }
    public float DistToGround { get; set; } //Distance to the ground for check if there is ground under the character
    public bool CanJump { get; set; }
    public bool CanMove { get; set; }
    public bool CanRun { get; set; }
    public bool WasGrounded { get; set; }
    public float FallTime { get; set; } //Time the player is falling
    public bool Attacking { get; set; }
    public bool IsHit { get; set; }
    public float AttackTimer { get; set; }
    public Vector3 ChildPlayeriniPos{ get; private set; }

    public Animator Animator { get { return _animator; } }
    public GameObject MovIndicator { get { return _movIndicator; } }
    public GameObject ChildPlayer { get { return _childPlayer; } }
    public CharacterController CharCont { get { return _charCont; } }
    public Camera Camera { get { return _camera; } }
    public SoundManager SoundMan { get { return _soundMan; } }

    public CharacterController CharController
    {
        get
        {
            if(_charCont == null || _charCont.IsDestroyed())
            {
                _charCont = GetComponent<CharacterController>();
            }
            return _charCont;
        }
        set
        {
            _charCont = value;
        }
    }


    public AnimatorEvents AnimEvent { get; private set; }

    public bool IsDashing() //Checks if player if dashing
    {
        return _currentDashTime < MaxDashTime;
    }
}
