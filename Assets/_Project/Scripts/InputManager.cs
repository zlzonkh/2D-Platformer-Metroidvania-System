using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;
    public static InputManager Instance => instance;

    private PlayerInput _inputActions;

    public float MoveInput { get; private set; }
    public bool IsJumpPressed { get; private set; }

    public event Action OnJumpStarted;
    public event Action OnJumpCanceled;
    public event Action OnAttackStarted;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject); return;
        }

        _inputActions = GetComponent<PlayerInput>();
    }

    void OnEnable()
    {
        _inputActions.actions["Move"].performed += OnMove;
        _inputActions.actions["Move"].canceled += OnMove;
        _inputActions.actions["Jump"].performed += OnJump;
        _inputActions.actions["Jump"].canceled += OnJump;
        _inputActions.actions["Attack"].performed += OnAttack;
    }

    void OnDisable()
    {
        _inputActions.actions["Move"].performed -= OnMove;
        _inputActions.actions["Move"].canceled -= OnMove;
        _inputActions.actions["Jump"].performed -= OnJump;
        _inputActions.actions["Jump"].canceled -= OnJump;
        _inputActions.actions["Attack"].performed -= OnAttack;
    }

    void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<float>();
    }

    void OnJump(InputAction.CallbackContext context)
    {
        IsJumpPressed = context.ReadValueAsButton();

        if (context.performed)
        {
            OnJumpStarted?.Invoke();
        }
        else if (context.canceled)
        {
            OnJumpCanceled?.Invoke();
        }
    }

    void OnAttack(InputAction.CallbackContext context)
    {
        IsJumpPressed = context.ReadValueAsButton();

        if (context.performed)
        {
            OnAttackStarted?.Invoke();
        }
    }
}
