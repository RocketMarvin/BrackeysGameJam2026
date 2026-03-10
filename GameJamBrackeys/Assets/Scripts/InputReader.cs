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

    public void SetUIAndGameplayActions()
    {
        _gameInput.Gameplay.Enable();
        _gameInput.UI.Enable();
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

    /// <summary> Gameplay Event </summary>
    public event Action<Vector2> MoveEvent;
    /// <summary> Gameplay Event </summary>
    public event Action MoveCancelEvent;
    /// <summary> Gameplay Event </summary>
    public event Action UseEvent;
    /// <summary> Gameplay Event </summary>
    public event Action JumpEvent;
    /// <summary> Gameplay Event </summary>
    public event Action JumpCancelEvent;
    /// <summary> Gameplay Event </summary>
    public event Action InteractEvent;
    /// <summary> Gameplay Event </summary>
    public event Action PauseEvent;

    /// <summary> UI Event </summary>
    public event Action<Vector2> PointerMoveEvent;
    /// <summary> UI Event </summary>
    public event Action ResumeEvent;
    /// <summary> UI Event </summary>
    public event Action ClickEvent;
    /// <summary> UI Event </summary>
    public event Action RightClickEvent;
    /// <summary> UI Event </summary>
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
