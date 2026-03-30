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

    private bool isAttacking = false;
    private bool isDead = false;

    [SerializeField] Animator anim;
    private Rigidbody2D rb;
    private Transform player;

    public enum State { Idle, Patrol, Chase, Melee, Ranged }
    public State currentState = State.Idle;



    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentHealth = maxHealth;
    }


    private void EnterState(State newState)
    {  // resets timers and switches to a new state
        currentState = newState;
        idleTimer = 0f;
        rangedTimer = 0f;
        meleeTimer = 0f;
    }

    private void HandleStateMachine()
    {
        if (isDead) return;

        switch (currentState)
        { // runs the correct state logic every frame
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
    {   // stands still and waits, then patrols or chases if player is spotted
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        if (CanSeePlayer()) { EnterState(State.Chase); return; }

        idleTimer += Time.deltaTime;
        if (idleTimer >= idleDuration && waypoints.Length > 0)
            EnterState(State.Patrol);
    }

    private void HandlePatrol()
    { // walks to points untill player is seen
        if (CanSeePlayer()) { EnterState(State.Chase); return; }

        Vector2 target = waypoints[waypointIndex].position;
        MoveTowards(target, patrolSpeed);

        // reached the waypoint, move to the next one and idle for a moment
        if (Mathf.Abs(transform.position.x - target.x) <= waypointTolerance)
        { 
            waypointIndex = (waypointIndex + 1) % waypoints.Length;
            EnterState(State.Idle);
        }
    }
    private void HandleChase()
    { // follows the player and decides which attack to use based on distance
        if (player == null) { EnterState(State.Idle); return; }

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist > detectionRange * 1.5f) { EnterState(State.Patrol); return; }
        if (dist <= meleeRange) { EnterState(State.Melee); return; }
        if (dist <= rangedRange) { EnterState(State.Ranged); return; }

        MoveTowards(player.position, chaseSpeed);
    }
    private void HandleMelee()
    { // stops and swings at the player when in melee range
        if (player == null) { EnterState(State.Idle); return; }

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > meleeRange) { EnterState(State.Chase); return; }

        if (isAttacking) return;

        meleeTimer += Time.deltaTime;
        if (meleeTimer >= meleeCooldown / AttackSpeedMultiplier)
        {
            meleeTimer = 0f;
            PerformMelee();
        }
    }
   
    private void HandleRanged()
    { // stops and casts spell at the player when in range
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
    }
    public void StartMeleeHitbox()
    {  // called by animation event when the sword hits
        StartCoroutine(ActivateMeleeHitbox());
    }

    private System.Collections.IEnumerator ActivateMeleeHitbox()
    {  // turns the hitbox on briefly then off, called by animation event
        isAttacking = true;

        yield return new WaitForSeconds(0.2f);
        if (meleeHitbox != null) meleeHitbox.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        if (meleeHitbox != null) meleeHitbox.SetActive(false);

        isAttacking = false;
    }

    private void PerformRanged()
    { // triggers the ranged cast animation, spell spawns from animation event
        anim?.SetTrigger("RangedAttack");
    }

    public void SpawnClaw()
    { // called by the cast animation event to spawn the spell on the player
        if (spellPrefab == null || player == null) return;

        Vector3 spawnPos = new Vector3(player.position.x, player.position.y + spellSpawnOffsetY, 0f);
        Instantiate(spellPrefab, spawnPos, Quaternion.identity);
    }

    private void MoveTowards(Vector2 target, float speed)
    { // moves the boss to a target position at a given speed
        Vector2 dir = (target - (Vector2)transform.position).normalized;
        rb.linearVelocity = new Vector2(dir.x * speed, rb.linearVelocity.y);
    }
    private bool CanSeePlayer()
    { // returns true if the player is within detection range
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
        if (isDead) return;

        
        currentHealth -= amount;
        anim?.SetTrigger("Hurt");
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
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Mini boss died!");
        anim?.SetTrigger("Death");
        Destroy(gameObject, 2f);
        if (GameManager.instance.winArea != null)
        {
            winArea.instance.OpenExitDoor();
        }
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
