using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour, IDamage
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

    [Header("Double Jump Unlock")]
    [SerializeField] private bool hasDoubleJump = false;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundLayer;

    [Header("Wall Check")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckRadius;
    [SerializeField] private LayerMask wallLayer;

    [Header("Wall Jump Settings")]
    [SerializeField] private int maxWallJumps;
    [SerializeField] private float wallJumpForceX;
    [SerializeField] private float wallJumpForceY;

    [Header("Attack")]
    [SerializeField] private float attackDuration;
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private GameObject attackHitbox;

    [Header("Damage Feedback")]
    [SerializeField] private float knockbackForceX = 10f;
    [SerializeField] private float knockbackForceY = 6f;
    [SerializeField] private float invincibilityTime = 1f;
    [SerializeField] private float flashInterval = 0.1f;

    private bool isStunned;
    private bool isInvincible;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Color originalColor;

    private float moveInput;
    private bool isGrounded;
    private bool isTouchingWall;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private bool isDashing;
    private bool canDash = true;
    private int facingDirection = 1;

    private int currentHealth;
    private bool isDead;

    private bool isAttacking;
    private bool canAttack = true;

    private bool canUseDoubleJump;
    private int remainingWallJumps;

    private Vector3 attackPointStartPosition;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        // Gets the components the player needs to move and animate
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        originalColor = spriteRenderer.color;

        // Starts the player with full health
        currentHealth = maxHealth;
        isDead = false;

        // Saves the starting position of the attack point
        if (attackPoint != null)
        {
            attackPointStartPosition = attackPoint.localPosition;
        }

        // Turns the hitbox off when the game begins
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }

        // Sets the number of wall jumps the player can perform
        remainingWallJumps = maxWallJumps;
    }

    private void Update()
    {
        // Temporary testing keys for damage and healing
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(1);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            Heal(1);
        }

        // Stops the rest of the logic if the player is dead
        if (isDead)
        {
            return;
        }

        // Reads left and right movement input
        moveInput = Input.GetAxisRaw("Horizontal");

        // Checks if the player is touching the ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Checks if the player is touching a wall
        if (wallCheck != null)
        {
            isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer);
        }

        // Tracks which direction the player is facing
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

        // Moves the attack point to the correct side of the player
        if (attackPoint != null)
        {
            attackPoint.localPosition = new Vector3(
                Mathf.Abs(attackPointStartPosition.x) * facingDirection,
                attackPointStartPosition.y,
                attackPointStartPosition.z
            );
        }

        // Updates animation values
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("YVelocity", rb.linearVelocity.y);

        // Stops movement logic while dashing or attacking
        if (isDashing || isAttacking)
        {
            return;
        }

        // Resets jump abilities when touching the ground
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            canUseDoubleJump = hasDoubleJump;
            remainingWallJumps = maxWallJumps;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Saves jump input for a short time window
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Normal jump
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }

        // Double jump
        else if (jumpBufferCounter > 0f && hasDoubleJump && canUseDoubleJump && !isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canUseDoubleJump = false;
            jumpBufferCounter = 0f;
        }

        // Wall jump
        else if (jumpBufferCounter > 0f && isTouchingWall && !isGrounded && remainingWallJumps > 0)
        {
            rb.linearVelocity = new Vector2(-facingDirection * wallJumpForceX, wallJumpForceY);
            remainingWallJumps--;
            jumpBufferCounter = 0f;
        }

        // Shortens the jump if the button is released early
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        // Starts dash if the ability has been unlocked
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && hasDash)
        {
            StartCoroutine(Dash());
        }

        // Starts an attack when clicking the mouse
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            StartCoroutine(Attack());
        }
    }

    private void FixedUpdate()
    {
        // Stops movement if the player is stunned, dead, dashing, or attacking
        if (isDead || isDashing || isAttacking || isStunned)
        {
            return;
        }

        // Calculates the speed the player wants to move
        float targetSpeed = moveInput * moveSpeed;

        // Calculates the difference between current and target speed
        float speedDifference = targetSpeed - rb.linearVelocity.x;

        // Chooses acceleration depending on whether the player is grounded
        float accelRate;

        if (Mathf.Abs(targetSpeed) > 0.01f)
        {
            accelRate = isGrounded ? acceleration : acceleration * airControlMultiplier;
        }
        else
        {
            accelRate = deceleration;
        }

        // Applies force to move the player
        float movement = speedDifference * accelRate;
        rb.AddForce(Vector2.right * movement);

        // Prevents the player from moving faster than the max speed
        if (Mathf.Abs(rb.linearVelocity.x) > moveSpeed)
        {
            rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * moveSpeed, rb.linearVelocity.y);
        }

        // Makes falling feel heavier
        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }

        // Makes short jumps feel better
        else if (rb.linearVelocity.y > 0f && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    private IEnumerator Dash()
    {
        // Prevents another dash until cooldown finishes
        canDash = false;
        isDashing = true;

        // Temporarily turns off gravity during the dash
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // Launches the player forward
        rb.linearVelocity = new Vector2(facingDirection * dashSpeed, 0f);

        // Waits for the dash to finish
        yield return new WaitForSeconds(dashDuration);

        // Restores gravity and movement
        rb.gravityScale = originalGravity;
        isDashing = false;

        // Waits before allowing another dash
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private IEnumerator Attack()
    {
        // Prevents attacking again until cooldown finishes
        canAttack = false;
        isAttacking = true;

        // Stops horizontal movement during attack
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        // Plays the attack animation
        animator.SetTrigger("Attack");

        // Activates the attack hitbox
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(true);
        }

        // Turns on the attack hitbox
        yield return new WaitForSeconds(attackDuration);

        // Turns the hitbox off
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }

        isAttacking = false;

        // Waits before the player can attack again
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void UnlockDash()
    {
        // Unlocks the dash ability later in the game
        hasDash = true;
    }

    public void UnlockDoubleJump()
    {
        // Unlocks the double jump ability later in the game
        hasDoubleJump = true;
        canUseDoubleJump = true;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead || isInvincible)
            return;

        currentHealth -= damageAmount;

        if (currentHealth < 0)
            currentHealth = 0;

        Debug.Log("Player took damage. Current health: " + currentHealth);

        StartCoroutine(DamageRoutine());

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int healAmount)
    {
        // Stops healing if the player is already dead
        if (isDead)
        {
            return;
        }

        currentHealth += healAmount;

        // Keeps health from going above max health
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        Debug.Log("Player healed. Current health: " + currentHealth);
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Player has died.");
    }

    private void OnDrawGizmosSelected()
    {
        // Draws helper circles in the editor for ground and wall checks

        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
        }
    }

   IEnumerator DamageRoutine()
    {
        isInvincible = true;
        isStunned = true;

        // Apply knockback
        rb.linearVelocity = new Vector2(-facingDirection * knockbackForceX, knockbackForceY);

        float timer = 0f;

        while (timer < invincibilityTime)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(flashInterval);

            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashInterval);

            timer += flashInterval * 2;
        }


        spriteRenderer.color = originalColor;
        isStunned = false;
        isInvincible = false;
    }
}