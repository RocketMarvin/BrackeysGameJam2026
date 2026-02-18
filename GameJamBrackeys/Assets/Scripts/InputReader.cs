using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR.Haptics;

[CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Objects/InputReader")]
public class InputReader : ScriptableObject, GameInput.IUIActions, GameInput.IGameplayActions
{
    private GameInput _gameInput;

    private void OnEnable()
    {
        if (_gameInput == null)
        {
            _gameInput = new GameInput();

            _gameInput.Gameplay.SetCallbacks(this);
            _gameInput.UI.SetCallbacks(this);

            SetGameplayActions();
        }
    }

    private void OnDisable()
    {
        _gameInput.UI.Disable();
        _gameInput.Gameplay.Disable();
    }
    /// <summary>
    /// Zet UI controls uit en gameplay controls aan.
    /// </summary>
    public void SetGameplayActions()
    {
        _gameInput.Gameplay.Enable();
        _gameInput.UI.Disable();
    }

    /// <summary>
    /// Zet input uit.
    /// </summary>
    public void DisableInput()
    {
        _gameInput.Gameplay.Disable();
        _gameInput.UI.Disable();
    }
    /// <summary>
    /// Zet Gameplay controls uit en UI controls aan.
    /// </summary>
    public void SetUIActions()
    {
        _gameInput.UI.Enable();
        _gameInput.Gameplay.Disable();
    }

    // Gameplay Action Events
    public event Action<Vector2> MoveEvent;
    public event Action MoveCancelEvent;
    public event Action UseEvent;
    public event Action JumpEvent;
    public event Action JumpCancelEvent;
    public event Action InteractEvent;
    public event Action PauseEvent;

    // UI Action Events
    public event Action<Vector2> PointerMoveEvent;
    public event Action ResumeEvent;
    public event Action ClickEvent;
    public event Action RightClickEvent;
    public event Action SubmitEvent;


    // Gameplay actions
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            UseEvent?.Invoke();
        }
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            InteractEvent?.Invoke();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            JumpEvent?.Invoke();
        }
        else if (context.phase == InputActionPhase.Canceled)
        { 
            JumpCancelEvent?.Invoke();
        }
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            MoveCancelEvent?.Invoke();
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            PauseEvent?.Invoke();
        }
    }
    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            RightClickEvent?.Invoke();
        }
    }

    // UIActions
    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            SubmitEvent?.Invoke();
        }
    }
    public void OnResume(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        { 
            ResumeEvent?.Invoke();
        }
    }
    public void OnPoint(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            PointerMoveEvent?.Invoke(context.ReadValue<Vector2>());
        }
    }
    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            ClickEvent?.Invoke();
        }
    }
}
