using UnityEngine;

public class Player : MonoBehaviour
{
    [field: Header("Movement")]
    [field: SerializeField] public float MoveSpeed { get; private set; } = 6.0f;

    [field: Header("Jump")]
    [field: SerializeField] public float JumpForce { get; private set; } = 18.5f;
    [field: SerializeField] public float JumpCutMultiplier { get; private set; } = 0.309f;
    [field: SerializeField] public float CoyoteTime { get; private set; } = 0.05f;
    [field: SerializeField] public float JumpBufferTime { get; private set; } = 0.05f;

    [Header("Detection")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Vector2 _groundCheckSize = new(0.425f, 0.05f);
    [SerializeField] private LayerMask _groundLayer;

    [Header("Combat")]
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private Vector2 _attackRange = new(2.0f, 1.0f);


    private Rigidbody2D _rb;
    private InputManager _input;
    private SpriteRenderer _sr;

    private float _coyoteTimer;
    private float _jumpBufferTimer;
    private bool _isJumping;
    private bool _isGrounded;
    private bool _isFacingLeft;

    private bool CanJump => (_isGrounded || _coyoteTimer > 0) && !_isJumping;

    #region Unity Lifecycle

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnJumpStarted -= HandleJumpInput;
            InputManager.Instance.OnJumpCanceled -= CutJump;
            InputManager.Instance.OnAttackStarted -= HandleAttackInput;

            InputManager.Instance.OnJumpStarted += HandleJumpInput;
            InputManager.Instance.OnJumpCanceled += CutJump;
            InputManager.Instance.OnAttackStarted += HandleAttackInput;
        }
    }

    void Start()
    {
        _input = InputManager.Instance;
    }

    void Update()
    {
        UpdateTimers();
        UpdateFacingDirection();
    }

    void FixedUpdate()
    {
        CheckGroundedStatus();
        UpdatePhysicsState();
        ProcessJumpBuffer();
        ApplyMovement();
    }

    void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnJumpStarted -= HandleJumpInput;
            InputManager.Instance.OnJumpCanceled -= CutJump;
            InputManager.Instance.OnAttackStarted -= HandleAttackInput;
        }
    }

    void OnDrawGizmos()
    {
        if (_groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_groundCheck.position, _groundCheckSize);
        }

        if (_attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_attackPoint.position, _attackRange);
        }
    }

    #endregion

    #region Environment & State

    void CheckGroundedStatus()
    {
        _isGrounded = Physics2D.OverlapBox(_groundCheck.position, _groundCheckSize, 0, _groundLayer);
    }

    void UpdatePhysicsState()
    {
        if (_isGrounded && _rb.linearVelocity.y <= 0.01f)
        {
            _isJumping = false;
        }
    }

    void UpdateFacingDirection()
    {
        float moveInput = _input.MoveInput;
        if (moveInput < 0 && !_isFacingLeft)
        {
            _isFacingLeft = true;
        }
        else if (moveInput > 0 && _isFacingLeft)
        {
            _isFacingLeft = false;
        }

        if (_attackPoint != null)
        {
            float xOffset = Mathf.Abs(_attackPoint.localPosition.x);
            _attackPoint.localPosition = new Vector3(_isFacingLeft ? -xOffset : xOffset, _attackPoint.localPosition.y, 0);
        }

        _sr.flipX = _isFacingLeft;
    }

    #endregion

    #region Horizontal Movement

    void ApplyMovement()
    {
        float moveInput = _input.MoveInput;
        _rb.linearVelocity = new Vector2(moveInput * MoveSpeed, _rb.linearVelocity.y);
    }

    #endregion

    #region Jump Mechanics

    void UpdateTimers()
    {
        _coyoteTimer = _isGrounded ? CoyoteTime : Mathf.Max(0, _coyoteTimer - Time.deltaTime);
        _jumpBufferTimer = Mathf.Max(0, _jumpBufferTimer - Time.deltaTime);
    }

    void ProcessJumpBuffer()
    {
        if (_jumpBufferTimer > 0 && CanJump)
        {
            ExecuteJump();
        }
    }

    void ExecuteJump()
    {
        _isJumping = true;
        _coyoteTimer = 0;
        _jumpBufferTimer = 0;

        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0);
        _rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
    }

    void HandleJumpInput()
    {
        _jumpBufferTimer = JumpBufferTime;
        if (CanJump)
        {
            ExecuteJump();
        }
    }

    void CutJump()
    {
        if (_rb.linearVelocity.y > 0)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y * JumpCutMultiplier);
        }
    }

    #endregion

    #region Attack Mechanics

    void HandleAttackInput()
    {
        // TODO: Implement attack input handling.
        Debug.Log("Attack input received.");
    }

    #endregion
}
