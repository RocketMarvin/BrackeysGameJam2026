using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputReader input;
    [SerializeField] float jumpForce;
    [SerializeField] float moveSpeed;

    Rigidbody2D rb;
    private bool IsGrounded = true;
    private float JumpBuffer = 0f;
    private readonly float JumpBufferTimer = 0.3f;

    Vector2 MoveDirection;

    void Start()
    {
        Physics2D.gravity = new Vector2(0, -15f);
        rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(MoveDirection.x * moveSpeed, rb.linearVelocityY);
        TurnToCursor();

        if (JumpBuffer > 0 && IsGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
            JumpBuffer = 0;
            Jump();
            JumpBuffer = 0;
        }
        JumpBuffer -= Time.deltaTime;
    }

    private void PauseGame()
    {
        input.SetUIActions();
    }

    private void ResumeGame()
    { 
        input.SetGameplayActions();
    }

    void TurnToCursor()
    {
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (mouseWorldPos.x >= transform.position.x)
        {
            transform.localScale = new Vector3(1, 1f, 1f);
        }
        else if (mouseWorldPos.x <= transform.position.x)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    private void Jump()
    {
        JumpBuffer = JumpBufferTimer;
    }

    private void MovePlayer(Vector2 axisIn)
    {
        MoveDirection = axisIn;
    }

    private void MovePlayerCancelled()
    { 
        MoveDirection = Vector2.zero;
    }

    private void JumpCancel()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * .5f);
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!IsGrounded)
        {
            if (collision.gameObject.CompareTag("Ground") && JumpBuffer > 0)
            {
                IsGrounded = true;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsGrounded)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                IsGrounded = false;
            }
        }
    }




    private void OnEnable()
    {
        input.MoveEvent += MovePlayer;
        input.JumpEvent += Jump;
        input.JumpCancelEvent += JumpCancel;
        input.PauseEvent += PauseGame;
        input.ResumeEvent += ResumeGame;
        input.MoveCancelEvent += MovePlayerCancelled;
    }
    private void OnDisable()
    {
        input.MoveEvent -= MovePlayer;
        input.JumpEvent -= Jump;
        input.JumpCancelEvent -= JumpCancel;
        input.PauseEvent -= PauseGame;
        input.ResumeEvent -= ResumeGame;
        input.MoveCancelEvent -= MovePlayerCancelled;
    }





}
