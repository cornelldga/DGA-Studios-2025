using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

//All possible states for the bull boss.



public class Pig_Rider : Boss
{
    public enum State
    {
        Charging, Targeting, Stunned, Marking, Bouncing
    }
    public State currentState;

    [Header("Movement Settings")]
    //Base speed when charging (regular)
    private float baseSpeed = 5f;
    //Acceleration amount for charging (accelerating while charging).
    private float acceleration = 6f;
    //Maximum speed to cap given acceleration.
    private float maxChargeSpeed = 10f;


    [Header("State Timing")]
    //How much time to get a lock on player.
    private float targetingTime = 1f;
    //How long we should stun upon collision.
    private float stunnedTime = 1f;
    //How much to recoil upon collision or player collision.
    private float recoilForce = 2f;

    [Header("Attack Settings")]
    [Tooltip("The layers that represent walls for collision detection")]
    public LayerMask wallLayers;
    [Tooltip("Chance (0-1) that boss will mark instead of charge")]
    [Range(0f, 1f)]
    [SerializeField] private float markChance = 0.3f;

    [Tooltip("Chance (0-1) that boss will enter bounce mode instead of normal charge")]
    [Range(0f, 1f)]
    [SerializeField] private float bounceChance = 0.2f;

    [Tooltip("Chance (0-1) that boss will enter enraged bounce mode when below half health")]
    [Range(0f, 1f)]
    [SerializeField] private float enragedBounceChance = 1f;
    //How fast to move during bouncing state.
    private float bounceSpeed;
    private float baseBounceSpeed = 10f;

    //How many more bounces we should take in bounce mode.
    private float bouncesRemaining = 5f;
    //The marking bullet pattern.
    [SerializeField] private BulletPattern markingBulletPattern;

    [Header("Bounce Mode Settings")]
    [Tooltip("Minimum number of bounces in bounce mode")]
    [SerializeField] private int minBounces = 3;

    [Tooltip("Maximum number of bounces in bounce mode")]
    [SerializeField] private int maxBounces = 7;
    private float damage = 1f;
    private bool isEnraged = false;


    [Header("Screen Shake")]
    private CinemachineImpulseSource impulseSource;
    [Tooltip("Force multiplier for wall collision shake")]
    private float wallShakeForce = 1f;
    [Tooltip("Force multiplier for player collision shake")]
    private float playersShakeForce = 0.5f;

    private Vector2 targetPosition;
    //direction of current charge.
    private Vector2 chargeDirection;

    private float currentSpeed;
    //Time until we should change states.
    private float stateTimer;
    private Rigidbody2D rb;
    // Marking logic
    private bool isMarked;
    private float markTimer;

    private Animator animator;
    private SpriteRenderer sprite;

    /// <summary>
    /// On start, we set the rigid body, and change its attributes. Immediately enter targeting.
    /// </summary>
    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        currentState = State.Targeting;
        stateTimer = targetingTime;
        bounceSpeed = baseBounceSpeed;
    }
    /// <summary>
    /// Updating of the statemachine.
    /// </summary>
    public override void Update()
    {
        base.Update();

        stateTimer -= Time.deltaTime;

        if (isMarked)
        {
            markTimer -= Time.deltaTime;
            if (markTimer <= 0)
            {
                removeMark();
            }
        }

        switch (currentState)
        {
            case State.Targeting:
                UpdateTargeting();
                break;
            case State.Charging:
                UpdateCharging();
                break;
            case State.Stunned:
                UpdateStunned();
                break;
            case State.Marking:
                // Handled by coroutine
                break;
            case State.Bouncing:
                UpdateBouncing();
                break;
        }
    }

    public bool IsMarked()
    {
        return isMarked;
    }

    public void ApplyMark(float markDuration)
    {
        isMarked = true;
        markTimer = markDuration;
    }

    public void removeMark()
    {
        isMarked = false;
    }

    /// <summary>
    /// Handles logic for targeting mode.
    /// </summary>
    private void UpdateTargeting()
    {
        // Track the player's position
        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            targetPosition = GameManager.Instance.player.transform.position;
        }

        // When targeting time is up, decide what to do
        if (stateTimer <= 0)
        {
            float randomValue = Random.value;

            if (randomValue < markChance)
            {
                TransitionToMarking();
            }
            else if (randomValue < markChance + bounceChance)
            {
                TransitionToBouncing();
            }
            else
            {
                TransitionToCharging();
            }
        }
    }
    /// <summary>
    /// Boss behavior in charging mode. Accelerating to a max speed.
    /// </summary>
    private void UpdateCharging()
    {
        currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxChargeSpeed);
        rb.linearVelocity = chargeDirection * currentSpeed;
    }
    /// <summary>
    /// Boss behavior in bouncing mode.
    /// </summary>
    private void UpdateBouncing()
    {
        currentSpeed = bounceSpeed;
        rb.linearVelocity = chargeDirection * currentSpeed;
        if (chargeDirection.x > 0) { sprite.flipX = true; }
        else if (chargeDirection.x < 0) { sprite.flipX = false; }
    }


    /// <summary>
    /// Boss behavior while stunned.
    /// </summary>
    private void UpdateStunned()
    {
        // Stop movement while stunned (recoiling)
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.deltaTime * 5f);

        // When stun time is up, go back to targeting
        if (stateTimer <= 0)
        {
            TransitionToTargeting();
        }
    }

    /// <summary>
    /// Setting state to targeting.
    /// </summary>
    private void TransitionToTargeting()
    {
        animator.SetBool("IsCharging", false);
        currentState = State.Targeting;
        stateTimer = targetingTime;
        rb.linearVelocity = Vector2.zero;
    }
    /// <summary>
    /// Setting state to charing. Direction of charge is in direction of target. (which was set earlier, once)
    /// </summary>
    private void TransitionToCharging()
    {
        currentState = State.Charging;
        chargeDirection = (targetPosition - (Vector2)transform.position).normalized;
        currentSpeed = baseSpeed;

        if (chargeDirection.x > 0) { sprite.flipX = true; }
        else if (chargeDirection.x < 0) { sprite.flipX = false; }

        animator.SetBool("IsCharging", true);
    }
    /// <summary>
    /// Setting state to marking. Uses a coroutine to perform the marking attack. Handles null case.
    /// </summary>
    private void TransitionToMarking()
    {
        animator.SetBool("IsCharging", false);
        currentState = State.Marking;
        rb.linearVelocity = Vector2.zero;

        if (markingBulletPattern != null)
        {
            StartCoroutine(PerformMarkingAttack());
        }
        else
        {
            // If no bullet pattern assigned, just go back to targeting
            TransitionToTargeting();
        }
    }
    /// <summary>
    /// Setting state to stunned.
    /// </summary>
    private void TransitionToStunned()
    {
        animator.SetBool("IsCharging", false);
        if (currentState == State.Bouncing)
        {
            currentState = State.Stunned;
            stateTimer = stunnedTime * 2; //Stun for longer if bouncing.
            return;
        }
        currentState = State.Stunned;
        stateTimer = stunnedTime;
    }
    /// <summary>
    /// Setting state to bouncing, and choosing some amount of bounces.
    /// </summary>
    private void TransitionToBouncing()
    {
        animator.SetBool("IsCharging", true);
        currentState = State.Bouncing;
        chargeDirection = (targetPosition - (Vector2)transform.position).normalized;
        bouncesRemaining = Random.Range(minBounces, maxBounces);
    }

    /// <summary>
    /// Sets the phase of bull rider based on the health percent.
    /// </summary>
    public override void SetPhase()
    {

        if (currentPhase == 1 && !isEnraged)
        {
            isEnraged = true;
            bounceChance = enragedBounceChance;
        }
    }

    /// <summary>
    /// Marking attack targeting and execution.
    /// </summary>
    private IEnumerator PerformMarkingAttack()
    {
        // Point towards player before attacking
        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            bulletOrigin.transform.right = GameManager.Instance.player.transform.position
                - bulletOrigin.transform.position;

            if (bulletOrigin.transform.right.x > 0) { sprite.flipX = true; }
            else if (bulletOrigin.right.x < 0) { sprite.flipX = false; }
        }

        // Execute the bullet pattern
        yield return StartCoroutine(markingBulletPattern.DoBulletPattern(this));

        // After marking (and spinning), return to targeting
        TransitionToTargeting();
    }
    private void HandleCharge(Collision2D collision)
    {
        // Stop current velocity first
        rb.linearVelocity = Vector2.zero;

        // Calculate recoil direction (bounce back from the surface)
        Vector2 collisionNormal = collision.contacts[0].normal;
        Vector2 recoilDirection = collisionNormal;

        // Apply recoil impulse
        rb.AddForce(recoilDirection * recoilForce, ForceMode2D.Impulse);

        // Trigger screen shake on wall hit
        if (((1 << collision.gameObject.layer) & wallLayers) != 0 && impulseSource != null)
        {
            impulseSource.GenerateImpulse(wallShakeForce);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
            if (impulseSource != null)
            {
                impulseSource.GenerateImpulse(playersShakeForce);
            }
        }
    }
    private void HandleBounce(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            impulseSource.GenerateImpulse(playersShakeForce);
            // Stop and recoil when hitting player
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
            rb.linearVelocity = Vector2.zero;
            Vector2 collisionNormal = collision.contacts[0].normal;
            rb.AddForce(collisionNormal * recoilForce, ForceMode2D.Impulse);
            bounceSpeed = baseBounceSpeed;
            TransitionToStunned();
            return;
        }

        float shakeIntensity = wallShakeForce * (bounceSpeed / baseBounceSpeed);
        impulseSource.GenerateImpulse(shakeIntensity);

        // Reflect the charge direction off the wall Increase our speed.
        Vector2 wallNormal = collision.contacts[0].normal;
        chargeDirection = Vector2.Reflect(chargeDirection, wallNormal);
        bounceSpeed += 1;
        bouncesRemaining--;

        // Trigger screen shake on each bounce (gets stronger with speed)

        //Stun when out of bounces.
        if (bouncesRemaining <= 0)
        {
            // Stop and recoil when bounces run out
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(wallNormal * recoilForce, ForceMode2D.Impulse);

            bounceSpeed = baseBounceSpeed;
            TransitionToStunned();
        }
    }
    /// <summary>
    /// Decides what should happen depending on state and if collision is with wal or player.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Normal charge mode - get stunned on collision
        if (currentState == State.Charging && (((1 << collision.gameObject.layer) & wallLayers) != 0 || collision.gameObject.CompareTag("Player")
            || collision.gameObject.CompareTag("Enemy")))
        {
            HandleCharge(collision);
            TransitionToStunned();
        }
        if (currentState == State.Bouncing && (((1 << collision.gameObject.layer) & wallLayers) != 0 || collision.gameObject.CompareTag("Player")))
        {
            HandleBounce(collision);
        }
    }

    public override void Attack()
    {
        // This boss uses its own state machine instead of the base attack system
    }

}
