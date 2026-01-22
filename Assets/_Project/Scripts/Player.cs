using UnityEngine;

public class Player : MonoBehaviour
{
    [field: Header("Movement Settings")] // Values related to player movement
    [field: SerializeField] public float MoveSpeed { get; private set; } = 5.0f;

    [field: Header("Jump Settings")] // Values related to player jumping
    [field: SerializeField] public float JumpForce { get; private set; } = 12.8f;
    [field: SerializeField] public float JumpCutMultiplier { get; private set; } = 0.309f;
    [field: SerializeField] public float CoyoteTime { get; private set; } = 0.05f;
    [field: SerializeField] public float JumpBufferTime { get; private set; } = 0.05f;
    private float _coyoteTimer = 0.0f;
    private float _jumpBufferTimer = 0.0f;
    private bool _isJumping = false;

    [Header("Ground Check Settings")] // Values related to ground detection
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Vector2 _groundCheckSize = new(0.425f, 0.05f);
    [SerializeField] private LayerMask _groundLayer;
    private bool _isGrounded = false;

    Rigidbody2D _rb;
    InputManager _input;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            // Unsubscribe first to prevent duplicate subscriptions
            InputManager.Instance.OnJumpStarted -= ApplyJump;
            InputManager.Instance.OnJumpCanceled -= CutJump;

            InputManager.Instance.OnJumpStarted += ApplyJump;
            InputManager.Instance.OnJumpCanceled += CutJump;
        }
    }

    void Start()
    {
        // Get reference to InputManager singleton
        _input = InputManager.Instance;
    }

    void Update()
    {
        HandleTimers();
    }

    void FixedUpdate()
    {
        CheckGroundedStatus();
        UpdatePhysicsState();
        ApplyJumpBuffer();
        ApplyMovement();
    }

    void OnDrawGizmos()
    {
        // Visualize the ground check area in the editor
        if (_groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_groundCheck.position, _groundCheckSize);
        }
    }

    void OnDisable()
    {
        // Unsubscribe from input events to prevent memory leaks
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnJumpStarted -= ApplyJump;
            InputManager.Instance.OnJumpCanceled -= CutJump;
        }
    }

    /// <summary>
    /// Handles timers such as coyote timer.
    /// </summary>
    void HandleTimers()
    {
        if (!_isGrounded)
        {
            if (_coyoteTimer > 0) _coyoteTimer -= Time.deltaTime;
            else _coyoteTimer = 0;
        }
        else
        {
            _coyoteTimer = CoyoteTime;
        }

        if (_jumpBufferTimer > 0) _jumpBufferTimer -= Time.deltaTime;
        else _jumpBufferTimer = 0;
    }

    /// <summary>
    /// Applies movement to the player based on input.
    /// </summary>
    void ApplyMovement()
    {
        float moveInput = _input.MoveInput;
        _rb.linearVelocity = new Vector2(moveInput * MoveSpeed, _rb.linearVelocity.y);
    }

    /// <summary>
    /// Checks if the player is grounded.
    /// </summary>
    void CheckGroundedStatus()
    {
        _isGrounded = Physics2D.OverlapBox(_groundCheck.position, _groundCheckSize, 0, _groundLayer);
    }

    /// <summary>
    /// Updates the player's physics state, including jumping status.
    /// </summary>
    void UpdatePhysicsState()
    {
        if (_isGrounded && _rb.linearVelocity.y <= 0)
        {
            _isJumping = false;
        }
    }

    /// <summary>
    /// Applies jump if pressed jump button just before landing.
    /// </summary>
    void ApplyJumpBuffer()
    {
        if (_jumpBufferTimer > 0 && (_isGrounded || _coyoteTimer > 0) && !_isJumping)
        {
            ApplyJump();
        }
    }

    /// <summary>
    /// Executes the physical jump action.
    /// </summary>
    void ExcuteJump()
    {
        _isJumping = true;
        _jumpBufferTimer = 0;

        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0); // Reset vertical velocity before jumping.
        _rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Applies jump force to the player when jump input is initiated.
    /// </summary>
    void ApplyJump()
    {
        _jumpBufferTimer = JumpBufferTime;
        if ((_isGrounded || _coyoteTimer > 0) && !_isJumping)
        {
            ExcuteJump();
        }
    }

    /// <summary>
    /// Cuts the jump height when jump input is canceled.
    /// </summary>
    void CutJump()
    {
        if (_rb.linearVelocity.y > 0)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y * JumpCutMultiplier);
        }
    }
}
