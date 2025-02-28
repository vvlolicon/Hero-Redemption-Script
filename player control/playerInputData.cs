using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class playerInputData : MonoBehaviour
{

	[Header("Movement Settings")]
	public bool analogMovement;

	[Header("Mouse Cursor Settings")]
	public GameObject inventoryUI;
	public GameObject equipmentUI;


	public bool InputJump { get; set; }
    public bool InputRun { get; private set; }
    public bool CursorLocked { get; private set; }
    public bool cursorInputForLook { get; set; }
    public Vector2 InputMove { get; private set; }
    public Vector2 InputLook { get; private set; }

    private void Start()
    {
		CursorLocked = true;
		cursorInputForLook = true;
    }

#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputValue value)
	{
		MoveInput(value.Get<Vector2>());
	}

	public void OnLook(InputValue value)
	{
		if(cursorInputForLook)
		{
			LookInput(value.Get<Vector2>());
		}
	}

	public void OnJump(InputValue value)
	{
		JumpInput(value.isPressed);
	}

	public void OnSprint(InputValue value)
	{
		SprintInput(value.isPressed);
	}

	public void OnOpenInventory()
	{
        inventoryUI.SetActive(true);
		equipmentUI.SetActive(true);

    }
#endif


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
		SetCursorState(CursorLocked);
	}

	private void SetCursorState(bool newState)
	{
		Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
	}
}