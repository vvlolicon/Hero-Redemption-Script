using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
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
    GameObject pauseMenu;
    public GameObject testData;
    GameObject hotbarItemWindow;
	//public Canvas UIWindow;

    private PlayerStateExecutor _executor;
	public List<GameObject> UIWindows = new List<GameObject>();

	public bool InputJump { get; set; }
    public bool InputRun { get; private set; }
    public bool CursorLocked { get; private set; }
    public bool InputEnable { get; set; }
    public Vector2 InputMove { get; private set; }
    public Vector2 InputLook { get; private set; }

    private void Awake()
    {
        _executor = GetComponent<PlayerStateExecutor>();
        hotbarItemWindow = GameObject.FindGameObjectWithTag("Player_HotbarItem");
        inventoryUI = GetUI_Window("Player_Inventory", 7);
        equipmentUI = GetUI_Window("Player_Equipment", 7);
        pauseMenu = GetUI_Window("PauseMenu", 7);
    }

    GameObject GetUI_Window(string tag, int layer)
    {
        GameObject[] windows = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject window in windows)
        {
            if (window.layer == layer)
            {
                UIWindows.Add(window);
                return window;
            }
        }
        return null;
    }

    private void Start()
    {
        EnableAllInput(true);

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
        if (InputEnable)
        {
            inventoryUI.SetActive(true);
            equipmentUI.SetActive(true);
            EnableAllInput(false);
            _executor.OnOpenInventory();
        }
    }

	public void OnEscHit(InputValue value)
	{
        bool hasUIactive = false;
		foreach(var UI in UIWindows)
		{
            if (UI != null && UI.activeInHierarchy)
            {
                hasUIactive = true;
                UI.SetActive(false);
				//break;
            }
        }
        if (hasUIactive)
        {
            SetCursorState(true);
            InputEnable = true;
        }
        else
        {
            EnterPauseMenu();
        }
    }
    public void OnUseHotbar_1(InputValue value)
    {
        if (value.isPressed)
        {
            UseHotbarItemAtSlot(0);
        }
    }
    public void OnUseHotbar_2(InputValue value)
    {
        if (value.isPressed)
        {
            UseHotbarItemAtSlot(1);
        }
    }
    public void OnUseHotbar_3(InputValue value)
    {
        if (value.isPressed)
        {
            UseHotbarItemAtSlot(2);
        }
    }
    public void OnUseHotbar_4(InputValue value)
    {
        if (value.isPressed)
        {
            UseHotbarItemAtSlot(3);
        }
    }
    public void OnUseHotbar_5(InputValue value)
    {
        if (value.isPressed)
        {
            UseHotbarItemAtSlot(4);
        }
    }
    public void OnUseHotbar_6(InputValue value)
    {
        if (value.isPressed)
        {
            UseHotbarItemAtSlot(5);
        }
    }
#endif
    public void EnterPauseMenu()
    {
        pauseMenu.SetActive(true);
        EnableAllInput(false);
    }
    public void ExitPauseMenu()
    {
        pauseMenu.SetActive(false);
        EnableAllInput(true);
    }


    void UseHotbarItemAtSlot(int slotNum)
    {
        Debug.Log("Using hotbar at slot " + slotNum);
        Transform slot = hotbarItemWindow.transform.GetChild(slotNum);
        if (slot.childCount > 0)
        {
            slot.GetChild(0).GetComponent<ConsumableItem>().ConsumeItem();
        }
    }

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
            EnableAllInput(true);
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

    public void EnableAllInput(bool b)
    {
        SetCursorState(b);
        InputEnable = b;
    }
}