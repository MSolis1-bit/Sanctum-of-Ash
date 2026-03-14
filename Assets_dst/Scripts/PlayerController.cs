using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    [Header("Movement Feel")]
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    [SerializeField] private float airControlMultiplier;

    [Header("Jump Feel")]
    [SerializeField] private float coyoteTime;
    [SerializeField] private float jumpBufferTime;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float lowJumpMultiplier;

    [Header("Dash")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldown;

    [Header("Dash Unlock")]
    [SerializeField] private bool hasDash = false;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private float moveInput;
    private bool isGrounded;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private bool isDashing;
    private bool canDash = true;
    private int facingDirection = 1;

    private void Awake()
    {
        // Gets the parts needed for movement and visuals
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Gets left and right input from the player
        moveInput = Input.GetAxisRaw("Horizontal");

        // Checks if the player is touching the ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Keeps track of which way the player is facing
        if (moveInput > 0f)
        {
            facingDirection = 1;
            spriteRenderer.flipX = false;
        }
        else if (moveInput < 0f)
        {
            facingDirection = -1;
            spriteRenderer.flipX = true;
        }

        // Stops normal update logic while dashing
        if (isDashing)
        {
            return;
        }

        // Gives the player a small grace period after leaving the ground
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Saves jump input for a short moment
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Lets the player jump if the timing window is still valid
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }

        // Makes the jump shorter if the jump button is released early
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        // Starts a dash only if the player has unlocked it
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && hasDash)
        {
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {
        // Stops normal movement while dashing
        if (isDashing)
        {
            return;
        }

        // Decides the speed the player is trying to reach
        float targetSpeed = moveInput * moveSpeed;

        // Finds the difference between current speed and target speed
        float speedDifference = targetSpeed - rb.linearVelocity.x;

        // Uses different movement strength depending on if the player is grounded
        float accelRate;

        if (Mathf.Abs(targetSpeed) > 0.01f)
        {
            accelRate = isGrounded ? acceleration : acceleration * airControlMultiplier;
        }
        else
        {
            accelRate = deceleration;
        }

        // Pushes the player toward the target speed
        float movement = speedDifference * accelRate;
        rb.AddForce(Vector2.right * movement);

        // Keeps the player from moving faster than the max move speed
        if (Mathf.Abs(rb.linearVelocity.x) > moveSpeed)
        {
            rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * moveSpeed, rb.linearVelocity.y);
        }

        // Makes falling feel heavier
        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        // Makes short jumps feel better when the jump button is released early
        else if (rb.linearVelocity.y > 0f && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    private IEnumerator Dash()
    {
        // Prevents another dash until the cooldown is done
        canDash = false;
        isDashing = true;

        // Temporarily turns off gravity during the dash
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // Launches the player in the direction they are facing
        rb.linearVelocity = new Vector2(facingDirection * dashSpeed, 0f);

        // Waits for the dash to finish
        yield return new WaitForSeconds(dashDuration);

        // Restores normal gravity and movement
        rb.gravityScale = originalGravity;
        isDashing = false;

        // Waits before allowing another dash
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void UnlockDash()
    {
        // Unlocks the dash ability for later progression
        hasDash = true;
    }

    private void OnDrawGizmosSelected()
    {
        // Draws the ground check in the editor so it is easier to place
        if (groundCheck == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
