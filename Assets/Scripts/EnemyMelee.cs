using System.Collections;
using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    [SerializeField] SpriteRenderer sr;
    private Transform player;

    [Header("Health")]
    [SerializeField] private float maxHealth;
    private float currentHealth;

    [Header("Movement")]
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float waypointTolerance;
    [SerializeField] private float idleDuration;

    [Header("Detection")]
    [SerializeField] private float detectionRange;
    [SerializeField] private float attackRange;

    [Header("Attack")]
    [SerializeField] private int damage;
    [SerializeField] private float attackCooldown;
    private float attackTimer;

    [Header("Patrol")]
    [SerializeField] private Transform[] waypoints;

    [Header("Damage Feedback")]
    [SerializeField] private float knockbackForceX;
    [SerializeField] private float knockbackForceY;
    [SerializeField] private float flashDuration;
    [SerializeField] private Color hitFlashColor = Color.red;
    [SerializeField] private float deathDelay;

    [Header("Death FeedBack")]
    [SerializeField] private float fadeDuration;
    [SerializeField] private Color deathColor = Color.grey;

    [Header("Death Sprite")]
    [SerializeField] private Sprite deathSprite;

    private Color originalColor;
    private bool facingRight = true;
    private int waypointIndex;
    private float idleTimer;
    private bool isDying;

    public enum State
    {
        Idle,
        Patrol,
        Chase,
        Attack
    }

    public State currentState = State.Idle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        facingRight = false;
        currentHealth = maxHealth;
        originalColor = sr.color;
    }

    private void Update()
    {
        if (isDying)
        {
            return;
        }

        HandleStateMachine();
        HandleFlip();
    }

    private void HandleStateMachine()
    {
        switch (currentState)
        {
            case State.Idle:
                HandleIdle();
                break;

            case State.Patrol:
                HandlePatrol();
                break;

            case State.Chase:
                HandleChase();
                break;

            case State.Attack:
                HandleAttack();
                break;
        }
    }

    private void HandleIdle()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (CanSeePlayer())
        {
            EnterState(State.Chase);
            return;
        }

        idleTimer += Time.deltaTime;

        if (idleTimer >= idleDuration && waypoints.Length > 0)
        {
            EnterState(State.Patrol);
        }
    }

    private void HandlePatrol()
    {
        if (CanSeePlayer())
        {
            EnterState(State.Chase);
            return;
        }

        if (waypoints.Length == 0)
        {
            EnterState(State.Idle);
            return;
        }

        Vector2 target = waypoints[waypointIndex].position;
        MoveTowards(target, patrolSpeed);

        if (Mathf.Abs(transform.position.x - target.x) <= waypointTolerance)
        {
            waypointIndex = (waypointIndex + 1) % waypoints.Length;
            EnterState(State.Idle);
        }
    }

    private void HandleChase()
    {
        if (player == null)
        {
            EnterState(State.Idle);
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > detectionRange * 1.5f)
        {
            EnterState(State.Patrol);
            return;
        }

        if (distance <= attackRange)
        {
            EnterState(State.Attack);
            return;
        }

        MoveTowards(player.position, chaseSpeed);
    }

    private void HandleAttack()
    {
        if (player == null)
        {
            EnterState(State.Idle);
            return;
        }

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            EnterState(State.Chase);
            return;
        }

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        Debug.Log("Enemy attacking object: " + player.name);
        if (player == null)
        {
            return;
        }

        PlayerController playerScript = player.GetComponent<PlayerController>();

        if (playerScript != null)
        {
            playerScript.TakeDamage(damage);
        }
    }

    private void EnterState(State newState)
    {
        currentState = newState;
        idleTimer = 0f;
        attackTimer = 0f;
    }

    private void MoveTowards(Vector2 target, float speed)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);
    }

    private bool CanSeePlayer()
    {
        if (player == null)
        {
            return false;
        }

        return Vector2.Distance(transform.position, player.position) <= detectionRange;
    }

    private void HandleFlip()
    {
        if (rb.linearVelocity.x > 0f && !facingRight)
        {
            Flip();
        }
        else if (rb.linearVelocity.x < 0f && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        transform.Rotate(0f, 180f, 0f);
        facingRight = !facingRight;
    }

    public void TakeDamage(float amount)
    {
        if (isDying)
        {
            return;
        }

        currentHealth -= amount;

        StartCoroutine(HitFlash());
        ApplyKnockback();

        if (currentHealth <= 0f)
        {
            StartCoroutine(DieRoutine());
        }
    }

    private IEnumerator HitFlash()
    {
        sr.color = hitFlashColor;
        yield return new WaitForSeconds(flashDuration);

        // Do not reset back if the enemy is already dying
        if (!isDying)
        {
            sr.color = originalColor;
        }
    }

    private void ApplyKnockback()
    {
        if (player == null)
        {
            return;
        }

        float direction = transform.position.x < player.position.x ? -1f : 1f;
        rb.linearVelocity = new Vector2(direction * knockbackForceX, knockbackForceY);
    }

    private IEnumerator DieRoutine()
    {
        isDying = true;

        // Stop movement
        rb.linearVelocity = Vector2.zero;

        // Stop animation
        if (anim != null)
        {
            anim.SetFloat("Speed", 0f);
            anim.enabled = false;
        }

        // Change to death sprite + grey color
        sr.color = deathColor;

        if (deathSprite != null)
        {
            sr.sprite = deathSprite;
        }



        //Destroys gameObject after a certain amount of time to avoid lag
        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject);

        // Disable script
        enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}