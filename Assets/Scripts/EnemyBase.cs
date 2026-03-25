using UnityEngine;
using System.Collections;



//This code was copied from Enemy.cs and edited to avoid conflicts

public class EnemyBase : MonoBehaviour, IDamage
{
    [SerializeField] public Animator anim;
    private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;
    public Transform player;
    [SerializeField] GameObject body;


    [Header("Health")]
    public float maxHealth;
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
    public Transform firePoint;
    public float attackCooldown;
    private float attackTimer;

    [Header("Damage Feedback")]
    [SerializeField] private float knockbackForceX;
    [SerializeField] private float knockbackForceY;
    [SerializeField] private float flashDuration;
    [SerializeField] private Color hitFlashColor = Color.red;
    [SerializeField] private float deathDelay;



    [Header("Patrol")]
    [SerializeField] private Transform[] waypoints;

    public bool facingRight;
    private int waypointIndex;
    private float idleTimer;

    private Color originalColor;
    private bool isDying;


    public enum State { Idle, Patrol, Chase, Attack }
    public State currentState = State.Idle;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentHealth = maxHealth;
        facingRight = false;
        originalColor = sr.color;
        
    }

    private void Update()
    {
        Debug.Log(rb.linearVelocity.x);
        HandleStateMachine();
        HandleFlip();
    }

    private void HandleStateMachine()
    {   // runs the correct state logic every frame
        switch (currentState)
        {
            case State.Idle: HandleIdle(); break;
            case State.Patrol: HandlePatrol(); break;
            case State.Chase: HandleChase(); break;
            case State.Attack: HandleAttack(); break;
        }
    }
    private void HandleIdle()
    {
        // changes states when player is seen
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        anim.SetBool("isMoving", false);
        if (CanSeePlayer()) { EnterState(State.Chase); return; }

        idleTimer += Time.deltaTime;
        if (idleTimer >= idleDuration && waypoints.Length > 0)
            EnterState(State.Patrol);
    }

    private void HandlePatrol()
    {
        // walks to points untill player is seen
        if (CanSeePlayer()) { EnterState(State.Chase); return; }

        Vector2 target = waypoints[waypointIndex].position;
        MoveTowards(target, patrolSpeed);
        anim.SetBool("isMoving", true);
        // when waypoint is hit sits then goes to the next 
        if (Mathf.Abs(transform.position.x - target.x) <= waypointTolerance)
        {
            waypointIndex = (waypointIndex + 1) % waypoints.Length;
            EnterState(State.Idle);
        }
    }
    private void HandleChase()
    {
        // follows player switches to attack when close and gives up when the player is to far
        if (player == null) { EnterState(State.Idle); return; }
        anim.SetBool("isMoving", true);
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > detectionRange * 1.5f) { EnterState(State.Patrol); return; }

        if (dist <= attackRange) { EnterState(State.Attack); return; }

        MoveTowards(player.position, chaseSpeed);
    }

    private void HandleAttack() // stops moving and shoots fireballs at the player until they leave attack range
    {

        if (player == null) { EnterState(State.Idle); return; }


        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        float dist = Vector2.Distance(transform.position, player.position);


        if (dist > attackRange) { EnterState(State.Chase); return; }

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            Attack();
        }

        if (player.position.x > transform.position.x && !facingRight)
            Flip();
        else if (player.position.x < transform.position.x && facingRight)
            Flip();
    }

    private void EnterState(State newState)  // resets timers and switches to a new state
    {
        currentState = newState;
        idleTimer = 0f;
        attackTimer = 0f;
    }
    private void MoveTowards(Vector2 target, float speed)
    { // moves the enemy toward a target position at a changed speed
        Vector2 dir = (target - (Vector2)transform.position).normalized;
        rb.linearVelocity = new Vector2(dir.x * speed, rb.linearVelocity.y);
    }
    private bool CanSeePlayer()
    { // returns true if the player is inside detection range
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.position) <= detectionRange;
    }

    protected virtual void Attack()
    {
       
    }


    // I dont need to put notes for the ones below since It should be self explainatory <--- But if you have a question please Dm me  :3
    private void HandleFlip()
    {
        if (player == null) return;
        if (rb.linearVelocity.x > 0 && !facingRight) {
            Flip();
        }
        else if (rb.linearVelocity.x < 0 && facingRight) { 
        Flip();
        }
    }

    private void Flip()
    {
        Debug.Log("FLIP");
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    public void TakeDamage(int amount)
    {
        if (isDying)
        {
            return;
        }

        anim.SetTrigger("hurt");
        currentHealth -= amount;

        StartCoroutine(HitFlash());
        ApplyKnockback();

        if (currentHealth <= 0)
        {
            anim.SetTrigger("death");
            StartCoroutine(DieRoutine());
        }
    }

    private IEnumerator DieRoutine()
    {
        isDying = true;

        rb.linearVelocity = Vector2.zero;

        sr.color = hitFlashColor;

        yield return new WaitForSeconds(deathDelay);

        Destroy(gameObject);
    }


    //private void OnDrawGizmos()  //  <----   THIS JUST CHANGES THE COLOR FOR BOTH RANGES <-- Yellow is Detection Range:  Red <--- red is the fireball range might change this to one later
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, detectionRange);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, attackRange);
    //}
    private IEnumerator HitFlash()
    {
        sr.color = hitFlashColor;
        yield return new WaitForSeconds(flashDuration);
        sr.color = originalColor;
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
}

