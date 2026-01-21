using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages user input through a singleton pattern.
/// </summary>
public class InputManager : MonoBehaviour
{
    // --- Singleton Setup ---
    // Private static instance
    private static InputManager instance;
    // Public static instance for global access from other scripts
    public static InputManager Instance => instance;

    // Private PlayerInput reference
    private PlayerInput _inputActions;

    // --- Properties to Expose Input Values ---
    public float MoveInput { get; private set; }
    public bool IsJumpPressed { get; private set; }

    // --- Events for Input Actions ---
    public event Action OnJumpStarted;
    public event Action OnJumpCanceled;

    void Awake()
    {
        // --- Initialize Singleton and Prevent Duplicates ---
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
        // Subscribe to input action events
        _inputActions.actions["Move"].performed += OnMove;
        _inputActions.actions["Move"].canceled += OnMove;
        _inputActions.actions["Jump"].performed += OnJump;
        _inputActions.actions["Jump"].canceled += OnJump;
    }

    void OnDisable()
    {
        // Unsubscribe to prevent memory leaks when disabled
        _inputActions.actions["Move"].performed -= OnMove;
        _inputActions.actions["Move"].canceled -= OnMove;
        _inputActions.actions["Jump"].performed -= OnJump;
        _inputActions.actions["Jump"].canceled -= OnJump;
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
}
