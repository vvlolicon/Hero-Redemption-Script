using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class PlayerStateExecutor : MonoBehaviour, IDamageable, IPickItem
{
    void Awake()
    {
        _charCont = GetComponent<CharacterController>();
        _soundMan = GetComponent<SoundManager>();
        _animator = GetComponentInChildren<Animator>();
        _playerInput = GetComponent<PlayerInputData>();
        _statDisplay = FindFirstObjectByType<PlayerStatDisplay>();
        _healthMan = GetComponent<HealthManager>();
        _healthMan._playerExecutor = this;
        DistToGround = _charCont.bounds.extents.y;
        setValue();
        AnimEvent = ChildPlayer.GetComponent<AnimatorEvents>();
        //_playerInput = new PlayerInputMethods();
        //_playerInput.PlayerControl.Move.started += OnMove;
        //_playerInput.PlayerControl.Move.performed += OnMove;
        //_playerInput.PlayerControl.Move.canceled += OnMove;
    }

    void setValue()
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

        RunSpeedMult = _playerStaticData._runSpeedMult;
        AtkSpeedMult = _playerStaticData._atkSpeedMult;
        JumpSpeed = _playerStaticData._jumpSpeed;
        Gravity = _playerStaticData._gravity;
        GroundedGravity = _playerStaticData._groundedGravity;
        Mass = _playerStaticData._mass;
        MaxDashTime = _playerStaticData._maxDashTime;
        DashSpeed = _playerStaticData._dashSpeed;

        _currentDashTime = MaxDashTime;
    }
    void Start()
    {
        ChildPlayeriniPos = ChildPlayer.transform.localPosition;
        _stateMan = new PlayerStatsManager(this);
        CurState = _stateMan.Grounded();
        CurState.EnterState();
        //setValue();
    }


    // Update is called once per frame
    void Update()
    {
        PlayerSpeed = _playerStats.SPEED / 10;
        CurState.UpdateStates();
        //test
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
        // Move the controller
        _charCont.Move(CurMovement * Time.deltaTime);
        _atkTimeCD = _playerStaticData._iniAtkSpeed * (1 / GetAtkSpeedMult());
        if (AttackTimer > 0)
        {
            AttackTimer -= Time.deltaTime;
            Debug.Log("AttackTimer: " + AttackTimer + " / atkTimeCD: " + _atkTimeCD);
        }
    }

    public float GetAtkSpeedMult()
    {
        return Mathf.Min((1 + _playerStats.AttackTime), 10f);
    }

    void FixedUpdate()
    {
        // for every MP regen frequency(default 1s), recover player MP for value of MP_Regen
        // to ensure actual time frequency, use fixed update for this
        if (_mpRegenTimer > _playerStaticData._mpRegenFreq)
        {
            // ensure MP does not exceed maximum MP
            PlayerStats.MP = Mathf.Min(PlayerStats.MaxMP, PlayerStats.MP + PlayerStats.MP_Regen);
            _mpRegenTimer = 0;
        } else
        {
            _mpRegenTimer += Time.deltaTime;
        }
    }

    public void OnJumpPressed()
    {
        if (CanJump && !Attacking)
        {
            //Animator.ResetTrigger("Exit");
            //Animator.SetTrigger("Jump");
            //Animator.SetFloat("SpeedY", 8f);
            //Animator.Play("StartJump");
            //NewRoroutine(DelayAction(0.4f));
            JumpPressed = true;
        }
    }
    public IEnumerator DelayAction(float delay, Action delayAction )
    {
        // Wait for the specified delay time
        yield return new WaitForSeconds(delay);

        // Action to perform after the delay
        delayAction?.Invoke();
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
            if (_charCont.isGrounded && !IsDashing())
            {
                AttackTimer = _atkTimeCD;
                AttackPressed = value;
            }
        }
    }

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
                info.ATK, PlayerStats.DEF, info.CritChance, PlayerStats.CritChanRdc, 
                PlayerStats.DmgReduce, info.CritMult, PlayerStats.CritDmgResis);
            _healthMan.Damage(dmgResult.Dmg);
            Debug.Log("You get damage: " + dmgResult.Dmg + " Is critial hit: " + dmgResult.IsCritHit);
        }
    }
    public void EnableMove(bool camMoveT) //Enables or disables the character movement
    {
        if (!IsHit)
            CanMove = camMoveT;
    }

    public void OnPickItem(InstantEffectPickupItem pickedItem)
    {
        PlayerBaseMethods.ChangePlayerStats(PlayerStats, pickedItem.itemAttributes);
    }

    public void NewRoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    public void OnLanding()
    {
        _playerInput.InputEnable = false;
        StartCoroutine(DelayAction(0.7f, () => {
            _playerInput.InputEnable = true;
            Animator.SetTrigger("Exit");
        }));
    }

    public void OnDying()
    {
        // TODO: player dead state;
        // TODO: player dead animation and UI window

        // TODO: SL system;
    }

    //public void OnEnable()
    //{
    //    _playerInput.PlayerControl.Enable();
    //}

    public GameObject _childPlayer;
    public Camera _camera;
    public GameObject _movIndicator; //Where is the character moving to
    public PlayerStaticData _playerStaticData;

    float _currentDashTime;
    Vector3 _dashDir;
    Vector3 _groundNormal;
    float _mpRegenTimer;
    float _atkTimeCD;

    CharacterController _charCont;
    Animator _animator;
    SoundManager _soundMan;
    PlayerInputData _playerInput;
    HealthManager _healthMan;

    //states variables
    PlayerStatsManager _stateMan;
    PlayerStatDisplay _statDisplay;

    //test
    public TMP_Text test_showState;
    public TMP_Text test_MovementY;

    public GeneralStatsObj _playerStats;


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
    public GeneralStatsObj PlayerStats { get { return _playerStats; } }
    public AnimatorEvents AnimEvent { get; private set; }

    public bool IsDashing() //Checks if player if dashing
    {
        return _currentDashTime < MaxDashTime;
    }
}
