using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
#endif

public class PlayerInputData : Singleton_LastIn<PlayerInputData>
{
    private PlayerStateExecutor _executor;
    PlayerBackpack _playerBackpack;
    public bool LockAllInput = false;

    public bool InputJump { get; set; }
    public bool InputRun { get; private set; }
    public bool CursorLocked { get; private set; }
    public bool InputEnable { get; set; }
    public Vector2 InputMove { get; private set; }
    public Vector2 InputLook { get; private set; }
    UI_Controller UI_Controller { get { return UI_Controller.Instance; }}

    private void Start()
    {
        _executor = GetComponent<PlayerStateExecutor>();
        _playerBackpack = GetComponent<PlayerBackpack>();
        EnableAllInput(true);
        if (gameObject.scene == SceneManager.GetActiveScene())
        {
            Initialize(); // load if current scene is dungeon
        }
    }
    public void Initialize()
    {
        EnableAllInput(true);

        // deactive ui windows after setting up vars
    }

#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputValue value)
	{
        if (LockAllInput) return;
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
        if (LockAllInput) return;
        if (CursorLocked)
		{
			LookInput(value.Get<Vector2>());
		}
	}
	public void OnJump(InputValue value)
    {
        if (LockAllInput) return;
        if (InputEnable)
		{
			_executor.OnJumpPressed();
		}
    }
	public void OnSprint(InputValue value)
    {
        if (LockAllInput) return;
        if (InputEnable)
		{
			SprintInput(value.isPressed);
		}
	}
    public void OnRun(InputValue value)
    {
        if (LockAllInput) return;
        if (InputEnable)
        {
            _executor.OnRunPressed();
        }
        //test_runPress.text = "run pressed: " + _runPressed;
    }
	public void OnAttack(InputValue value)
    {
        if (LockAllInput) return;
        if (InputEnable && CursorLocked)
        {
            _executor.OnAttackPressed(value.isPressed);
        }
    }
    public void OnOpenInventory()
    {
        if (LockAllInput) return;
        if (InputEnable)
        {
            Debug.Log("UI_Controller is " + UI_Controller.name);
            UI_Controller.OpenInventoryUI(UI_Window.InventoryUI,
                _playerBackpack.GetPlayerBackpackItems());
            UI_Controller.OpenInventoryUI(UI_Window.EquipmentUI,
                _playerBackpack.GetPlayerEquippedItems());
            EnableAllInput(false);
            _executor.OnOpenInventory();
        }
    }
	public void OnEscHit(InputValue value)
    {
        if (LockAllInput) return;
        if (UI_Controller.HasClosableWindowActive())
        {
            UI_Controller.CloseAllClosableWindows();
            UI_Controller.ShowInteractTooltipIfHasInteractable();
        }
        else
        {
            OpenPauseMenu();
        }
    }
    public void OnUseHotbar_1(InputValue value)
    {
        if (LockAllInput) return;
        if (value.isPressed)
        {
            UseHotbarItemAtSlot(0);
        }
    }
    public void OnUseHotbar_2(InputValue value)
    {
        if (LockAllInput) return;
        if (value.isPressed)
        {
            UseHotbarItemAtSlot(1);
        }
    }
    public void OnUseHotbar_3(InputValue value)
    {
        if (LockAllInput) return;
        if (value.isPressed)
        {
            UseHotbarItemAtSlot(2);
        }
    }
    public void OnUseHotbar_4(InputValue value)
    {
        if (LockAllInput) return;
        if (value.isPressed)
        {
            UseHotbarItemAtSlot(3);
        }
    }
    public void OnUseHotbar_5(InputValue value)
    {
        if (LockAllInput) return;
        if (value.isPressed)
        {
            UseHotbarItemAtSlot(4);
        }
    }
    public void OnUseHotbar_6(InputValue value)
    {
        if (LockAllInput) return;
        if (value.isPressed)
        {
            UseHotbarItemAtSlot(5);
        }
    }
    public void OnInteract()
    {
        if (LockAllInput) return;
        if (UI_Controller.HasClosableWindowActive()) return;
        if (UI_Controller.GetUIScript<InteractObject>().InteractWithObject())
        {
            EnableAllInput(false);
        }
    }

#endif
    public void OpenPauseMenu()
    {
        UI_Controller.SetUIActive(UI_Window.PauseMenu, true);
        Time.timeScale = 0;
        EnableAllInput(false);
    }
    public void ExitPauseMenu()
    {
        UI_Controller.GetUIScript<PauseMenu>().OnExitWindow();
    }


    void UseHotbarItemAtSlot(int slotNum)
    {
        Transform hotbarItemWindow = UI_Controller.Hotbar.transform.GetChild(0);
        Debug.Log("Using hotbar at slot " + slotNum);
        Transform slot = hotbarItemWindow.GetChild(slotNum);
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
        if (!UI_Controller.HasClosableWindowActive())
        {
            EnableAllInput(true);
        }
    }

    public void OnShowTestData(InputValue value)
	{
        UI_Controller.TestStatics.SetActive(!UI_Controller.TestStatics.activeSelf);
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
        _executor.RunPressed = false;
        _executor.OnMovePressed(Vector2.zero);
    }
}