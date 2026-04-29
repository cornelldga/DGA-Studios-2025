using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class Pig : MonoBehaviour
{
    [SerializeField] Drover pigRider;
    [SerializeField] float ramDamage = 1f;
    [Tooltip("The layers that represent walls for collision detection")]
    public LayerMask wallLayers;

    [Header("Screen Shake")]
    private CinemachineImpulseSource impulseSource;
    [Tooltip("Force multiplier for wall collision shake")]
    private float wallShakeForce = 0.05f;
    [Tooltip("Force multiplier for player collision shake")]
    private float playersShakeForce = 0.1f;
    [Tooltip("Force multiplier for boss collision shake")]
    private float enemyShakeForce = 0.05f;

    [Header("Movement Settings")]
    //Base speed when charging (regular)
    private float baseSpeed = 5f;
    //Acceleration amount for charging (accelerating while charging).
    private float acceleration = 6f;
    //Maximum speed to cap given acceleration.
    private float maxChargeSpeed = 10f;

    [Header("Movement Settings")]
    [Tooltip("Speed while patrolling side to side")]
    [SerializeField] private float patrolSpeed = 2f;
    [Tooltip("Distance to patrol left and right from starting point")]
    [SerializeField] private float patrolDistance = 1f;
    [Tooltip("Elevation offset while patrolling")]
    [SerializeField] private float patrolElevation = 1f;
    [Tooltip("Minimum random delay before pig starts moving")]
    [SerializeField] private float minStartDelay = 0f;
    [Tooltip("Maximum random delay before pig starts moving")]
    [SerializeField] private float maxStartDelay = 2f;
    [Tooltip("Seconds pig is stunned after colliding")]
    [SerializeField] float stunTime;
    [Tooltip("Distance at which pig registers a hit on Pig Rider")]
    [SerializeField] private float pigRiderHitDistance = 0.8f;
    [Tooltip("Knockback force when hit by the whip")]
    [SerializeField] private float whipKnockbackForce = 8f;
    private float patrolDirectionX = 1f; // 1 for right, -1 for left
    private float patrolDirectionY = 1f; // Same meaning for as patrolDirectionX
    private float leftBoundary;
    private float rightBoundary;
    private float upBoundary;
    private float downBoundary;
    private bool isInitialized = false;

    [Header("Visuals")]
    [Tooltip("Smoke trail GameObject shown while pig is charging")]
    [SerializeField] private GameObject smokeTrail;

    [Header("Return Settings")]
    [Tooltip("Speed when returning to starting point")]
    [SerializeField] private float returnSpeed = 4f;
    [Tooltip("Distance threshold to consider pig has arrived at starting point")]
    [SerializeField] private float arrivalThreshold = 0.1f;

    private Vector2 targetPosition;
    private Vector2 chargeDirection;
    private Vector2 startingPoint;
    private float currentSpeed;
    private Rigidbody2D rb;
    private Collider2D thisCollider;
    private List<Collider2D> ignoredColliders;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector3 smokeTrailPos;
    private Vector3 smokeTrailScale;

    BoxCollider2D pigCollider;

    public enum State
    {
        Patrolling, Charging, Returning, Stunned
    }

    public State currentState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pigCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        thisCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        ignoredColliders = new List<Collider2D>();

        if (smokeTrail != null)
        {
            smokeTrailPos = smokeTrail.transform.localPosition;
            smokeTrailScale = smokeTrail.transform.localScale;
        }

        startingPoint = new Vector2(transform.position.x, transform.position.y);
        leftBoundary = startingPoint.x - patrolDistance;
        rightBoundary = startingPoint.x + patrolDistance;
        upBoundary = startingPoint.y + patrolElevation;
        downBoundary = startingPoint.y - patrolElevation;

        FlipSprite();

        // Start with a random delay
        float randomDelay = Random.Range(minStartDelay, maxStartDelay);
        StartCoroutine(InitializeWithDelay(randomDelay));
    }

    /// <summary>
    /// Initializes the pig after a random delay to desynchronize multiple pigs.
    /// </summary>
    private System.Collections.IEnumerator InitializeWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isInitialized = true;
        TransitionToPatrolling();
    }

    // Update is called once per frame
    void Update()
    {
        if (smokeTrail != null)
        {
            smokeTrail.SetActive(currentState == State.Charging);
            Vector3 pos = smokeTrailPos;
            Vector3 scale = smokeTrailScale;
            if (spriteRenderer.flipX)
            {
                pos.x = -pos.x;
                scale.x = -scale.x;
            }
            smokeTrail.transform.localPosition = pos;
            smokeTrail.transform.localScale = scale;
        }

        // Don't do anything until initialized
        if (!isInitialized && currentState != State.Stunned && currentState != State.Charging)
        {
            return;
        }

        switch (currentState)
        {
            case State.Patrolling:
                UpdatePatrolling();
                break;
            case State.Charging:
                UpdateCharging();
                break;
            case State.Returning:
                UpdateReturning();
                break;
            case State.Stunned:
                // Handled by a coroutine
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                break;
        }
    }

    /// <summary>
    /// Boss behavior in charging mode. Accelerating to a max speed.
    /// If targets lose their marks, transition to returning instead of patrolling.
    /// </summary>
    private void UpdateCharging()
    {
        currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxChargeSpeed);
        rb.linearVelocity = chargeDirection * currentSpeed;

        if (pigRider.IsMarked() && Vector2.Distance(transform.position, pigRider.transform.position) < pigRiderHitDistance)
        {
            HitPigRider();
        }
    }

    private void HitPigRider()
    {
        currentState = State.Stunned;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        animator.SetBool("isStunned", true);
        pigRider.isInvulnerable = false;
        impulseSource.GenerateImpulse(enemyShakeForce);
        StartCoroutine(StunCoroutine());
    }

    /// <summary>
    /// Pig patrols side to side between boundaries, flipping sprite based on direction.
    /// Transitions to targeting when a valid target is marked.
    /// </summary>
    private void UpdatePatrolling()
    {
        if (currentState == State.Charging)
        {
            return;
        }
        if (pigRider.IsMarked())
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
            ChargeTarget(pigRider.transform.position);
            return;
        }

        float elevation = Mathf.PingPong(Time.time, patrolElevation * 2) - patrolElevation;

        Vector2 movement = new Vector2(patrolDirectionX * patrolSpeed, patrolDirectionY * elevation);
        rb.linearVelocity = movement;

        if (patrolDirectionX > 0 && transform.position.x >= rightBoundary)
        {
            patrolDirectionX = -1f;
            FlipSprite();
        }
        else if (patrolDirectionX < 0 && transform.position.x <= leftBoundary)
        {
            patrolDirectionX = 1f;
            FlipSprite();
        }

        if (patrolDirectionY > 0 && transform.position.y >= upBoundary)
        {
            patrolDirectionY = -1f;
        }
        else if (patrolDirectionY < 0 && transform.position.y <= downBoundary)
        {
            patrolDirectionY = 1f;
        }
    }

    /// <summary>
    /// Flips the sprite horizontally to face the movement direction.
    /// </summary>
    private void FlipSprite()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }
    /// <summary>
    /// Chooses the target for the pig and transitions to charging
    /// </summary>
    /// <param name="targetPos"></param>
    public void ChargeTarget(Vector3 targetPos)
    {
        targetPosition = targetPos;
        TransitionToCharging();
    }

    /// <summary>
    /// Pig returns to starting position after charging. Transitions to patrolling upon arrival.
    /// </summary>
    private void UpdateReturning()
    {
        Vector2 directionToStart = (startingPoint - (Vector2)transform.position).normalized;

        rb.linearVelocity = directionToStart * returnSpeed;

        float distanceToStart = Vector2.Distance(transform.position, startingPoint);
        if (distanceToStart <= arrivalThreshold)
        {
            rb.linearVelocity = Vector2.zero;
            transform.position = startingPoint;

            TransitionToPatrolling();
        }
    }

    private void TransitionToReturning()
    {
        pigCollider.enabled = false;
        currentState = State.Returning;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    private void TransitionToPatrolling()
    {
        pigCollider.enabled = true;
        currentState = State.Patrolling;
        animator.SetBool("isCharging", false);
    }

    private void TransitionToCharging()
    {
        pigCollider.enabled = true;
        animator.SetBool("isCharging", true);
        currentState = State.Charging;

        chargeDirection = (targetPosition - (Vector2)transform.position).normalized;
        currentSpeed = baseSpeed;
    }

    /// <summary>
    /// Handles collision during charge state. Applies recoil, damage, and screen shake.
    /// Transitions to stunned state, then to returning state after stun duration.
    /// </summary>
    /// <param name="collision">The collision data from OnCollisionEnter2D</param>
    private void HandleCharge(Collision2D collision)
    {
        currentState = State.Stunned;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        animator.SetBool("isStunned", true);

        if (collision.gameObject == pigRider.gameObject)
        {
            pigRider.isInvulnerable = false;
            impulseSource.GenerateImpulse(enemyShakeForce);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.player.TakeDamage(ramDamage);
            if (impulseSource != null)
            {
                impulseSource.GenerateImpulse(playersShakeForce);
            }
        }
        else
        {
            impulseSource.GenerateImpulse(wallShakeForce);
        }
        StartCoroutine(StunCoroutine());
    }

    /// <summary>
    /// Coroutine that handles the stun state with a freeze duration.
    /// </summary>
    private System.Collections.IEnumerator StunCoroutine()
    {
        yield return new WaitForSeconds(stunTime);
        animator.SetBool("isStunned", false);
        TransitionToReturning();
    }

    /// <summary>
    /// Decides what should happen depending on state and if collision is with wal or player.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == State.Charging)
        {
            HandleCharge(collision);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Whip"))
        {
            Vector2 knockbackDir = ((Vector2)transform.position - (Vector2)collision.transform.position).normalized;
            chargeDirection = knockbackDir;
            rb.linearVelocity = knockbackDir * whipKnockbackForce;
            impulseSource.GenerateImpulse(playersShakeForce);
        }
    }
}
