using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputReader input;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float jumpCutMultiplier = 0.5f; 
    [SerializeField] private float coyoteTimer = 0.15f;
    [SerializeField] private float jumpBufferTimer = 0.3f;
    [SerializeField] private float cancelJumpTimer = 0.2f;
    [SerializeField] private float groundCheckDistance = 0.6f;
    [SerializeField] private LayerMask groundLayer;
    //[SerializeField] private Sprite normalSprite;
    //[SerializeField] private Sprite oceanSprite;

    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float coyoteCounter = 0;
    private float jumpBufferCounter;
    private bool isGrounded;
    private bool isHoldingJump = false;
    private float cancelJumpCounter = 0f;
    private SpriteRenderer sprite;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Physics2D.gravity = new Vector2(0, -13f);
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        //Ground check
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);

        isGrounded = (hit.collider != null);

        if (isGrounded)
        {
            coyoteCounter = coyoteTimer;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }
        
        jumpBufferCounter -= Time.deltaTime;
        cancelJumpCounter -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        if (isGrounded)
        { 
            
        }

        animator.SetBool("isWalking", Mathf.Abs(moveInput.x) > 0f && isGrounded);

        if (isGrounded)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
        }

        if (jumpBufferCounter > 0 && coyoteCounter > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            coyoteCounter = 0;
            jumpBufferCounter = 0;
            animator.SetBool("isJumping", true);
        }

        if (!isHoldingJump && cancelJumpCounter < 0f)
        {
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
                animator.SetBool("isFalling", true);
                animator.SetBool("isJumping", false);
            }
        }
    }

    private void OnEnable()
    {
        input.MoveEvent += OnMove;
        input.MoveCancelEvent += OnMoveCancel;
        input.JumpEvent += OnJumpPressed;
        input.JumpCancelEvent += OnJumpReleased;
    }

    private void OnDisable()
    {
        input.MoveEvent -= OnMove;
        input.MoveCancelEvent -= OnMoveCancel;
        input.JumpEvent -= OnJumpPressed;
        input.JumpCancelEvent -= OnJumpReleased;
    }

    private void OnMove(Vector2 direction)
    {
        moveInput = direction;
        sprite.flipX = direction.x > 0;
    }

    private void OnMoveCancel()
    {
        moveInput = Vector2.zero;
    }

    private void OnJumpPressed()
    {
        jumpBufferCounter = jumpBufferTimer;
        cancelJumpCounter = cancelJumpTimer;
        isHoldingJump = true;
    }

    private void OnJumpReleased()
    {
        animator.SetBool("isJumping", false);
        animator.SetBool("isFalling", true);

        isHoldingJump = false;
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Ocean"))
    //    {
    //        sprite.sprite = oceanSprite;
    //        print(sprite.sprite.name);
    //    }
    //}
    //void OnTriggerExit2D(Collider2D other)
    //{
    //    if (other.CompareTag("Ocean"))
    //    {
    //        sprite.sprite = normalSprite;
    //    }
    //}

}