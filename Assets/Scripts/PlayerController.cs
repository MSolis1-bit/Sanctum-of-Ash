using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour, IDamage, IHeal/*, IDataPersistence*/
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    private float originalMoveSpeed;

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

    [Header("Wall Slide Settings")]
    [SerializeField] private float wallSlideSpeed;

    [Header("Wall Jump Lockout")]
    [SerializeField] private float wallJumpLockTime;

    [Header("Attack")]
    [SerializeField] private float attackDuration;
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private GameObject attackHitbox;

    [Header("Damage Feedback")]
    [SerializeField] private float knockbackForceX;
    [SerializeField] private float knockbackForceY;
    [SerializeField] private float invincibilityTime;
    [SerializeField] private float flashInterval;

    [Header("Corner Correction")]
    [SerializeField] private Transform ceilingCheck;
    [SerializeField] private float ceilingCheckRadius;
    [SerializeField] private LayerMask ceilingLayer;
    [SerializeField] private float cornerCorrectionDistance;
    [SerializeField] private float cornerCorrectionStep;

    private bool isStunned;
    private bool isInvincible;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Color originalColor;

    private float moveInput;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool isWallJumping;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private float wallJumpLockCounter;
    private bool isPressingIntoWall;
    private bool isTouchingWallLeft;
    private bool isTouchingWallRight;
    private int wallDirection;

    private bool isDashing;
    private bool canDash = true;
    private int facingDirection = 1;

    private int currentHealth;
    private bool isDead;

    private bool isAttacking;
    private bool canAttack = true;
    private Vector3 attackHitboxStartPosition;
    private Vector3 attackPointStartPosition;

    private bool isTouchingCeilingLeft;
    private bool isTouchingCeilingRight;
    private bool isTouchingCeilingMiddle;

    private bool canUseDoubleJump;
    private int remainingWallJumps;

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

        // Saves the starting position of the attack point and attack hitbox
        if (attackPoint != null)
        {
            attackPointStartPosition = attackPoint.localPosition;
        }
        if (attackHitbox != null)
        {
            attackHitboxStartPosition = attackHitbox.transform.localPosition;
        }

        // Turns the hitbox off when the game begins
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }

        // Sets the number of wall jumps the player can perform
        remainingWallJumps = maxWallJumps;

        originalMoveSpeed = moveSpeed;
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

        // Counts down the short lockout after a wall jump
        if (wallJumpLockCounter > 0f)
        {
            wallJumpLockCounter -= Time.deltaTime;
        }
        else
        {
            isWallJumping = false;
        }

        // Reads left and right movement input
        moveInput = Input.GetAxisRaw("Horizontal");

        // Checks if the player is touching the ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Checks both sides of the player for a wall
        if (wallCheck != null)
        {
            Vector2 rightCheckPosition = wallCheck.position;

            Vector2 leftCheckPosition = new Vector2(
                transform.position.x - (wallCheck.position.x - transform.position.x),
                wallCheck.position.y
            );

            isTouchingWallRight = Physics2D.OverlapCircle(rightCheckPosition, wallCheckRadius, wallLayer);
            isTouchingWallLeft = Physics2D.OverlapCircle(leftCheckPosition, wallCheckRadius, wallLayer);

            isTouchingWall = isTouchingWallLeft || isTouchingWallRight;

            if (isTouchingWallRight)
            {
                wallDirection = 1;
            }
            else if (isTouchingWallLeft)
            {
                wallDirection = -1;
            }
            else
            {
                wallDirection = 0;
            }
        }
        else
        {
            isTouchingWall = false;
            isTouchingWallLeft = false;
            isTouchingWallRight = false;
            wallDirection = 0;
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

        // Moves the attack hitbox to the correct side of the player
        if (attackHitbox != null)
        {
            attackHitbox.transform.localPosition = new Vector3(
                Mathf.Abs(attackHitboxStartPosition.x) * facingDirection,
                attackHitboxStartPosition.y,
                attackHitboxStartPosition.z
            );
        }

        // Checks if the player is pressing toward the wall
        isPressingIntoWall = isTouchingWall && moveInput == wallDirection;

        // Checks if the player should slide on the wall
        isWallSliding = !isWallJumping && !isGrounded && isTouchingWall && isPressingIntoWall;

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

        // Wall jump
        else if (jumpBufferCounter > 0f && isTouchingWall && !isGrounded && remainingWallJumps > 0)
        {
            rb.linearVelocity = new Vector2(-wallDirection * wallJumpForceX, wallJumpForceY);
            remainingWallJumps--;
            jumpBufferCounter = 0f;

            // Gives the wall jump priority over other air jumps
            isWallJumping = true;
            wallJumpLockCounter = wallJumpLockTime;
        }

        // Double jump
        else if (jumpBufferCounter > 0f && hasDoubleJump && canUseDoubleJump && !isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canUseDoubleJump = false;
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
        if (Input.GetKeyDown(KeyCode.K) && canAttack)
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

        // Gives the wall jump time to push the player away cleanly
        if (isWallJumping)
        {
            return;
        }

        // Makes the player slide down the wall instead of getting stuck on it
        if (isWallSliding)
        {
            float clampedY = rb.linearVelocity.y;

            // Stops the upward climb when the player presses into the wall
            if (clampedY > 1f)
            {
                clampedY = 1f;
            }

            // Limits how fast the player falls while sliding
            clampedY = Mathf.Max(clampedY, -wallSlideSpeed);

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, clampedY);
        }

        HandleCornerCorrection();

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

        // Waits while the attack is active
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
        GameManager.instance.UpdatePlayerUI();
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
        GameManager.instance.UpdatePlayerUI();
        Debug.Log("Player healed. Current health: " + currentHealth);
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Player has died.");
    }

    private void HandleCornerCorrection()
    {
        // Only try to correct the player while moving upward
        if (rb.linearVelocity.y <= 0f)
        {
            return;
        }

        if (ceilingCheck == null)
        {
            return;
        }

        Vector2 middleCheck = ceilingCheck.position;
        Vector2 leftCheck = new Vector2(ceilingCheck.position.x - cornerCorrectionDistance, ceilingCheck.position.y);
        Vector2 rightCheck = new Vector2(ceilingCheck.position.x + cornerCorrectionDistance, ceilingCheck.position.y);

        isTouchingCeilingMiddle = Physics2D.OverlapCircle(middleCheck, ceilingCheckRadius, ceilingLayer);
        isTouchingCeilingLeft = Physics2D.OverlapCircle(leftCheck, ceilingCheckRadius, ceilingLayer);
        isTouchingCeilingRight = Physics2D.OverlapCircle(rightCheck, ceilingCheckRadius, ceilingLayer);

        // If the middle is blocked, but one side is open, nudge the player sideways
        if (isTouchingCeilingMiddle)
        {
            // Left side blocked, right side open, move right
            if (isTouchingCeilingLeft && !isTouchingCeilingRight)
            {
                transform.position += new Vector3(cornerCorrectionStep, 0f, 0f);
            }
            // Right side blocked, left side open, move left
            else if (isTouchingCeilingRight && !isTouchingCeilingLeft)
            {
                transform.position += new Vector3(-cornerCorrectionStep, 0f, 0f);
            }
        }
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

            Vector2 leftCheckPosition = new Vector2(
                transform.position.x - (wallCheck.position.x - transform.position.x),
                wallCheck.position.y
            );

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(leftCheckPosition, wallCheckRadius);
        }

        if (ceilingCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(ceilingCheck.position, ceilingCheckRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(
                new Vector2(ceilingCheck.position.x - cornerCorrectionDistance, ceilingCheck.position.y),
                ceilingCheckRadius
            );

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(
                new Vector2(ceilingCheck.position.x + cornerCorrectionDistance, ceilingCheck.position.y),
                ceilingCheckRadius
            );
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

    public void SetStunned(bool value)
    {
        isStunned = value;
    }

    public void SetInvincible(bool value)
    {
        isInvincible = value;
    }

    public void ModifySpeed(float multiplier)
    {
        moveSpeed = originalMoveSpeed * multiplier;
    }

    public void ResetSpeed()
    {
        moveSpeed = originalMoveSpeed;
    }

    //public void LoadData(GameData data)
    //{
    //    this.transform.position = data.playerPosition;
    //    this.maxHealth = data.maxHealth;
    //    this.hasDash = data.hasDash;
    //    this.hasDoubleJump = data.hasDoubleJump;
    //}

    //public void SaveData(ref GameData data)
    //{
    //    data.playerPosition = this.transform.position;
    //    data.maxHealth = this.maxHealth;
    //    data.hasDash = this.hasDash;
    //    data.hasDoubleJump = this.hasDoubleJump;

    //}
}