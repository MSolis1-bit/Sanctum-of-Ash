using UnityEngine;

public class MiniBoss : MonoBehaviour, IDamage
{
    [Header("Health")]
    [SerializeField] private float maxHealth;
    private float currentHealth;
    private bool isPhaseTwo = false;

    [Header(" Second Phase")]
    [SerializeField] private float phaseTwoAttackSpeedMultiplier;

    [Header("Movement")]
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float waypointTolerance;
    [SerializeField] private float idleDuration;

    [Header("Detection")]
    [SerializeField] private float detectionRange;
    [SerializeField] private float meleeRange;
    [SerializeField] private float rangedRange;

    [Header("Ranged")]
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private float spellSpawnOffsetY;
    [SerializeField] private float rangedCooldown;

    [Header("Melee")]
    [SerializeField] private GameObject meleeHitbox;
    [SerializeField] private float meleeCooldown;

    [Header("Patrol")]
    [SerializeField] private Transform[] waypoints;

    private float meleeTimer;
    private float rangedTimer;

    private bool facingRight = true;
    private int waypointIndex;
    private float idleTimer;

    private Animator anim;
    private Rigidbody2D rb;
    private Transform player;

    public enum State { Idle, Patrol, Chase, Melee, Ranged }
    public State currentState = State.Idle;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentHealth = maxHealth;
    }

    private void EnterState(State newState)
    {
        currentState = newState;
        idleTimer = 0f;
        rangedTimer = 0f;
        meleeTimer = 0f;
    }

    private void HandleStateMachine()
    {
        switch (currentState)
        {
            case State.Idle: HandleIdle(); break;
            case State.Patrol: HandlePatrol(); break;
            case State.Chase: HandleChase(); break;
            case State.Melee: HandleMelee(); break;
            case State.Ranged: HandleRanged(); break;
        }
    }
    private void Update()
    {
        HandleStateMachine();
        HandleFlip();
        anim?.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }
    private void HandleIdle()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        if (CanSeePlayer()) { EnterState(State.Chase); return; }

        idleTimer += Time.deltaTime;
        if (idleTimer >= idleDuration && waypoints.Length > 0)
            EnterState(State.Patrol);
    }

    private void HandlePatrol()
    {
        if (CanSeePlayer()) { EnterState(State.Chase); return; }

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
        if (player == null) { EnterState(State.Idle); return; }

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist > detectionRange * 1.5f) { EnterState(State.Patrol); return; }
        if (dist <= meleeRange) { EnterState(State.Melee); return; }
        if (dist <= rangedRange) { EnterState(State.Ranged); return; }

        MoveTowards(player.position, chaseSpeed);
    }
    private void HandleMelee()
    {
        if (player == null) { EnterState(State.Idle); return; }

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > meleeRange) { EnterState(State.Chase); return; }

        meleeTimer += Time.deltaTime;
        if (meleeTimer >= meleeCooldown / AttackSpeedMultiplier)
        {
            meleeTimer = 0f;
            PerformMelee();
        }
    }
   
    private void HandleRanged()
    {
        if (player == null) { EnterState(State.Idle); return; }

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= meleeRange) { EnterState(State.Melee); return; }
        if (dist > rangedRange) { EnterState(State.Chase); return; }

        rangedTimer += Time.deltaTime;
        
        if (rangedTimer >= rangedCooldown / AttackSpeedMultiplier)
        {
            rangedTimer = 0f;
            PerformRanged();
        }
    }

    private void PerformMelee()
    {
        anim?.SetTrigger("Attack");
        StartCoroutine(ActivateMeleeHitbox());
    }

    private System.Collections.IEnumerator ActivateMeleeHitbox()
    {
        yield return new WaitForSeconds(0.1f);
        if (meleeHitbox != null) meleeHitbox.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        if (meleeHitbox != null) meleeHitbox.SetActive(false);
    }

    private void PerformRanged()
    {
        anim?.SetTrigger("RangedAttack");
    }

    public void SpawnClaw()
    {
        if (spellPrefab == null || player == null) return;

        Vector3 spawnPos = new Vector3(player.position.x, player.position.y + spellSpawnOffsetY, 0f);
        Instantiate(spellPrefab, spawnPos, Quaternion.identity);
    }

    private void MoveTowards(Vector2 target, float speed)
    {
        Vector2 dir = (target - (Vector2)transform.position).normalized;
        rb.linearVelocity = new Vector2(dir.x * speed, rb.linearVelocity.y);
    }
    private bool CanSeePlayer()
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.position) <= detectionRange;
    }
    private void HandleFlip()
    {
        if (CanSeePlayer() && player != null)
        {
            
            if (player.position.x > transform.position.x && !facingRight) Flip();
            else if (player.position.x < transform.position.x && facingRight) Flip();
        }
        else
        {
            
            if (rb.linearVelocity.x > 0 && !facingRight) Flip();
            else if (rb.linearVelocity.x < 0 && facingRight) Flip();
        }
    }
    private void Flip()
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }


    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Mini boss took damage, current health: " + currentHealth);

        
        if (!isPhaseTwo && currentHealth <= maxHealth / 2)
            EnterPhaseTwo();

        if (currentHealth <= 0)
            Die();
    }

    private void EnterPhaseTwo()
    {
        isPhaseTwo = true;
        anim?.SetBool("IsPhaseTwo", true);
        anim?.SetTrigger("PhaseTwo");
    }

    private void Die()
    {
        Debug.Log("Mini boss died!");
        Destroy(gameObject);
    }

    public float AttackSpeedMultiplier => isPhaseTwo ? phaseTwoAttackSpeedMultiplier : 1f;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangedRange);
    }

}
