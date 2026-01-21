using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")] // Values related to player movement
    [field: SerializeField] public float MoveSpeed { get; private set; } = 5.0f;

    Rigidbody2D _rb;
    InputManager _input;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Get reference to InputManager singleton
        _input = InputManager.Instance;
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    /// <summary>
    /// Applies movement to the player based on input.
    /// </summary>
    void ApplyMovement()
    {
        float moveInput = _input.MoveInput;
        _rb.linearVelocity = new Vector2(moveInput * MoveSpeed, _rb.linearVelocity.y);
    }
}
