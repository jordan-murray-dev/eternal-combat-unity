using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LocomotionState
{
    Idle,
    Running,
    Attacking,
    Jumping,
    Hit,
    Death
}

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    private KeyCode keyMoveLeft, keyMoveRight, keyJump, keyAttack;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float airControlMultiplier = 0.5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float coyoteTime = 0.1f;          // Time after leaving ground when you can still jump
    [SerializeField] private float jumpBufferTime = 0.1f;      // Time early jump input is buffered
    [SerializeField] private float jumpCutMultiplier = 0.5f;   // How much to cut velocity when releasing jump early

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer; // Optional: assign for flipping

    private Rigidbody2D _rb;
    private AnimationController _animationController;
    private RoundManager _roundManager;
    private bool _roundLocked = false;

    private float _horizontalInput;
    private bool _jumpPressed;
    private bool _jumpHeld;
    private bool _attackPressed;

    private bool _isGrounded;
    private float _lastTimeOnGround;      // Time since last grounded
    private float _lastTimeJumpPressed;   // Time since jump was pressed

    private UnityEngine.Coroutine _attackCoroutine;
    private GameObject _attackHitDetection;
    private bool _hurt;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animationController = GetComponent<AnimationController>();
        _attackHitDetection = this.gameObject.transform.Find("AttackHitDetection").gameObject;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        InitKeys();
        _roundManager = FindAnyObjectByType<RoundManager>();
    }

    private void InitKeys()
    {
        if (tag == "player1")
        {
            keyMoveLeft = KeyCode.A;
            keyMoveRight = KeyCode.D;
            keyJump = KeyCode.W;
            keyAttack = KeyCode.LeftShift;
        }
        else if (tag == "player2")
        {
            keyMoveLeft = KeyCode.LeftArrow;
            keyMoveRight = KeyCode.RightArrow;
            keyJump = KeyCode.UpArrow;
            keyAttack = KeyCode.RightShift;
        }
        else
        {
            throw new System.Exception("Unidentified Key");
        }
    }

    private void Update()
    {
        if (_roundManager != null && _roundManager.IsRoundOver)
        {
            if (!_roundLocked)
            {
                _roundLocked = true;
                _horizontalInput = 0f;
                _jumpPressed = false;
                _jumpHeld = false;
                _attackPressed = false;

                if (_attackCoroutine != null)
                {
                    StopCoroutine(_attackCoroutine);
                    _attackCoroutine = null;
                }

                if (_animationController != null)
                    _animationController.SwitchAnimationState(CharacterAnimationState.Idle, true);
            }

            return;
        }

    _roundLocked = false;

        // --- Input using custom keys ---

        // Horizontal: -1, 0, or 1 based on left/right keys
        _horizontalInput = 0f;
        if (Input.GetKey(keyMoveLeft))
            _horizontalInput -= 1f;
        if (Input.GetKey(keyMoveRight))
            _horizontalInput += 1f;

        if (Mathf.Abs(_horizontalInput) == 0f)
            _animationController.SwitchAnimationState(CharacterAnimationState.Idle);
        else
            _animationController.SwitchAnimationState(CharacterAnimationState.Run);

        // Jump buffer
        if (Input.GetKeyDown(keyJump)&&!_hurt)
        {
            _jumpPressed = true;
            _lastTimeJumpPressed = jumpBufferTime;
            _animationController.SwitchAnimationState(CharacterAnimationState.Jump, true);
        }

        // Attack
        if (Input.GetKeyDown(keyAttack) && !_hurt)
        {
            _animationController.SwitchAnimationState(CharacterAnimationState.Attack, true);
            if (_attackCoroutine != null) StopCoroutine(_attackCoroutine);
            _attackCoroutine = StartCoroutine(AttackCoroutine());
        }

        _jumpHeld = Input.GetKey(keyJump);

        // Attack (optional; not used elsewhere yet)
        _attackPressed = Input.GetKeyDown(keyAttack);

        // timers
        if (_lastTimeOnGround > -Mathf.Infinity)
            _lastTimeOnGround -= Time.deltaTime;

        if (_lastTimeJumpPressed > -Mathf.Infinity)
            _lastTimeJumpPressed -= Time.deltaTime;

        // Flip sprite
        if (spriteRenderer && Mathf.Abs(_horizontalInput) > 0.01f && !_hurt)
        {
            spriteRenderer.flipX = !(_horizontalInput < 0f);
            bool leftside = _horizontalInput < 0f;
            _attackHitDetection.transform.localPosition = new Vector3(leftside ? -0.3f : 0.3f, 0.65f, 0);
        }
    }

    private void FixedUpdate()
    {
        if(_roundManager != null && _roundManager.IsRoundOver)
        {
            _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
            return;
        }

        CheckGrounded();
        HandleMovement();
        HandleJump();
    }

    private void CheckGrounded()
    {
        bool wasGrounded = _isGrounded;
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (_isGrounded)
        {
            _lastTimeOnGround = coyoteTime; // reset coyote timer
        }
        else
        {
            _lastTimeOnGround -= Time.fixedDeltaTime;
        }
    }

    private void HandleMovement()
    {
        float targetSpeed = _hurt ? 0f : _horizontalInput * moveSpeed;
        _rb.linearVelocity = new Vector2(targetSpeed, _rb.linearVelocity.y);
    }

    private void HandleJump()
    {
        if(_jumpPressed&&_isGrounded)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _hurt ? 0f : jumpForce);
        }

        if(_jumpPressed)
            _jumpPressed = false;
    }

    private IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        if(_roundManager != null && _roundManager.IsRoundOver)
        {
            _attackCoroutine = null;
            yield break;
        }

        BoxCollider2D b2d = _attackHitDetection.GetComponent<BoxCollider2D>();
        List<Collider2D> collider2ds = new List<Collider2D>();
        b2d.Overlap(collider2ds);

        foreach (var otherCollider in collider2ds)
        {
            if(_roundManager != null && _roundManager.IsRoundOver)
            {
                yield break;
            }

            if(otherCollider.gameObject.GetComponent<PlayerController2D>() == null) continue;
            if(otherCollider.gameObject == this.gameObject) continue;

            PlayerController2D otherPlayerController2D = otherCollider.GetComponent<PlayerController2D>();
            otherPlayerController2D.OnDamaged(this);
        }

        yield return null;
        _attackCoroutine = null;
    }

    public void OnDamaged(PlayerController2D attacker)
    {
        if(_roundManager != null && _roundManager.IsRoundOver)
        {
            return;
        }

        this._animationController.SwitchAnimationState(CharacterAnimationState.Hit);
        this.GetComponent<PlayerHealth>().OnDamaged();
        ScoreManager.Instance.AddHitPoints(attacker.gameObject.tag == "player1" ? 1 : 2);
    }

    private IEnumerator OnHurtCoroutine()
    {
        _hurt = true;
        yield return new WaitForSeconds(1);
        _hurt = false;
    }
}