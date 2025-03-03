using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInputData : MonoBehaviour
{

	//[Header("Movement Settings")]
	//public bool analogMovement;

    [Header("UI settings")]
    GameObject inventoryUI;
    GameObject equipmentUI;
    public GameObject testData;
	//public Canvas UIWindow;

    private PlayerStateExecutor _executor;
	List<GameObject> UIWindows = new List<GameObject>();

	public bool InputJump { get; set; }
    public bool InputRun { get; private set; }
    public bool CursorLocked { get; private set; }
    public bool InputEnable { get; set; }
    public Vector2 InputMove { get; private set; }
    public Vector2 InputLook { get; private set; }

    private void Awake()
    {
        _executor = GetComponent<PlayerStateExecutor>();
        GameObject[] windows = GameObject.FindGameObjectsWithTag("UIWindow");
        UIWindows.AddRange(windows);
    }

    private void Start()
    {
        SetCursorState(true);
		InputEnable = true;
        
        // deactive ui windows after setting up vars
        foreach (GameObject ui in UIWindows)
        {
            ui.SetActive(false);
        }
    }

#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputValue value)
	{
        if (InputEnable)
        {
            _executor.OnMovePressed(value.Get<Vector2>());
        }
        else
        {
            _executor.OnMovePressed(Vector2.zero);
        }

    }

	public void OnLook(InputValue value)
	{
		if(CursorLocked)
		{
			LookInput(value.Get<Vector2>());
		}
	}

	public void OnJump(InputValue value)
	{
		if (InputEnable)
		{
			_executor.OnJumpPressed();
		}
    }

	public void OnSprint(InputValue value)
	{
		if (InputEnable)
		{
			SprintInput(value.isPressed);
		}
	}

    public void OnRun(InputValue value)
    {
		if (InputEnable)
        {
            _executor.OnRunPressed();
        }
        //test_runPress.text = "run pressed: " + _runPressed;
    }

	public void OnAttack(InputValue value)
	{
        if (InputEnable && CursorLocked)
        {
            _executor.OnAttackPressed(value.isPressed);
        }
    }

    public void OnOpenInventory()
	{
        foreach (GameObject ui in UIWindows)
        {
            ui.SetActive(true);
        }
        SetCursorState(false);
		InputEnable = false;
        _executor.OnOpenInventory();
    }

	public void OnEscHit(InputValue value)
	{
		foreach(var UI in UIWindows)
		{
            if (UI.activeInHierarchy)
            {
                UI.SetActive(false);
				//break;
            }
        }
        SetCursorState(true);
        InputEnable = true;
    }
#endif

	public void OnUIQuit()
	{
        StartCoroutine(CheckUIWindowActive());
    }

    public IEnumerator CheckUIWindowActive()
	{
        yield return new WaitForSeconds(0.1f);
        bool hasWindowActive = false;
        foreach (var UI in UIWindows)
        {
            if (UI.activeInHierarchy)
            {
                hasWindowActive = true;
                break;
            }
        }
        if (!hasWindowActive)
        {
            SetCursorState(true);
            InputEnable = true;
        }
    }

    public void OnShowTestData(InputValue value)
	{
        testData.SetActive(!testData.activeInHierarchy);
    }


	public void MoveInput(Vector2 newMoveDirection)
	{
        InputMove = newMoveDirection;
	} 

	public void LookInput(Vector2 newLookDirection)
	{
        InputLook = newLookDirection;
	}

	public void JumpInput(bool newJumpState)
	{
        InputJump = newJumpState;
	}

	public void SprintInput(bool newSprintState)
	{
        InputRun = newSprintState;
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		SetCursorState(hasFocus);
	}

	private void SetCursorState(bool newState)
	{
        CursorLocked = newState;
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
	}
}