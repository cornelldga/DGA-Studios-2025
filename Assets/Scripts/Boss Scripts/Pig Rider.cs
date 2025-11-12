using System.Collections;
using UnityEditorInternal;
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
    [Tooltip("Chance (0-1) that boss will mark instead of charge")]
    [SerializeField] private float markChance = 0.3f;
    [Tooltip("Chance (0-1) that boss will enter bounce mode instead of normal charge")]
    [SerializeField] private float bounceChance = 0.2f;
    //How fast to move during bouncing state.
    private float bounceSpeed;
    private float baseBounceSpeed = 10f;

    //How many more bounces we should take in bounce mode.
    private float bouncesRemaining = 5f;
    //The marking bullet pattern.
    private BulletPattern markingBulletPattern;

    [Header("Bounce Mode Settings")]
    private float damage = 1f;

    [Header("Screen Shake")]
    private CinemachineImpulseSource impulseSource;
    [Tooltip("Force multiplier for wall collision shake")]
    private float wallShakeForce = 1f;
    [Tooltip("Force multiplier for player collision shake")]
    private float playersShakeForce = 0.5f;


    [Header("Victory Spin")]
    private bool enableVictorySpin = true;
    private float spinDuration = 1f;
    private float spinSpeed = 360;

    private Vector2 targetPosition;
    //direction of current charge.
    private Vector2 chargeDirection;

    private float currentSpeed;
    //Time until we should change states.
    private float stateTimer;
    private Rigidbody2D rb;

/// <summary>
/// On start, we set the rigid body, and change its attributes. Immediately enter targeting.
/// </summary>
    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.bodyType = RigidbodyType2D.Dynamic;

        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        currentState = State.Targeting;
        stateTimer = targetingTime;
        bounceSpeed = baseBounceSpeed;
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }
    /// <summary>
    /// Updating of the statemachine.
    /// </summary>
    public override void Update()
    {
        base.Update();

        stateTimer -= Time.deltaTime;

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

        chargeDirection =(targetPosition - (Vector2)transform.position).normalized;
        currentSpeed = baseSpeed;
    }
    /// <summary>
    /// Setting state to marking. Uses a coroutine to perform the marking attack. Handles null case.
    /// </summary>
    private void TransitionToMarking()
    {
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
        currentState = State.Bouncing;
        chargeDirection = (targetPosition - (Vector2)transform.position).normalized;
        bouncesRemaining = Random.Range(3, 7);
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
        }

        // Execute the bullet pattern
        yield return StartCoroutine(markingBulletPattern.DoBulletPattern(this));

        // Visual spin after successful marking
        if (enableVictorySpin)
        {
            yield return StartCoroutine(VictorySpin());
        }

        // After marking (and spinning), return to targeting
        TransitionToTargeting();
    }
    /// <summary>
    /// Spinning in a full cicrle after attack.
    /// </summary>
    private IEnumerator VictorySpin()
    {
        float elapsedTime = 0f;

        while (elapsedTime < spinDuration)
        {
            elapsedTime += Time.deltaTime;
            float rotationAmount = spinSpeed * Time.deltaTime;
            transform.Rotate(0f, 0f, rotationAmount);
            yield return null;
        }
    }
    /// <summary>
    /// Decides what should happen depending on state and if collision is with wal or player.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Normal charge mode - get stunned on collision
        if (currentState == State.Charging && (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Player")))
        {
            // Stop current velocity first
            rb.linearVelocity = Vector2.zero;

            // Calculate recoil direction (bounce back from the surface)
            Vector2 collisionNormal = collision.contacts[0].normal;
            Vector2 recoilDirection = collisionNormal;

            // Apply recoil impulse
            rb.AddForce(recoilDirection * recoilForce, ForceMode2D.Impulse);

            // Trigger screen shake on wall hit
            if (collision.gameObject.CompareTag("Wall") && impulseSource != null)
            {
                impulseSource.GenerateImpulse(wallShakeForce);
            }
            else if (collision.gameObject.CompareTag("Player") && impulseSource != null){
                collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
                impulseSource.GenerateImpulse(playersShakeForce);
            }

            TransitionToStunned();
        }
        if (currentState == State.Bouncing && (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Player") ))
        {
            //Send impulse to cinemachine.
            if (impulseSource != null && collision.gameObject.CompareTag("Player"))
            {
                impulseSource.GenerateImpulse(playersShakeForce);
            }
            if (collision.gameObject.CompareTag("Player"))
            {
                // Stop and recoil when hitting player
                collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
                rb.linearVelocity = Vector2.zero;
                Vector2 collisionNormal = collision.contacts[0].normal;
                rb.AddForce(collisionNormal * recoilForce, ForceMode2D.Impulse);

                bounceSpeed = baseBounceSpeed;
                TransitionToStunned();
                return;
            }

            // Reflect the charge direction off the wall Increase our speed.
            Vector2 wallNormal = collision.contacts[0].normal;
            chargeDirection = Vector2.Reflect(chargeDirection, wallNormal);
            bounceSpeed += 1;
            bouncesRemaining--;

            // Trigger screen shake on each bounce (gets stronger with speed)
            if (impulseSource != null)
            {
                float shakeIntensity = wallShakeForce * (bounceSpeed / baseBounceSpeed);
                impulseSource.GenerateImpulse(shakeIntensity);
            }
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
    }

    public override void Attack()
    {
        // This boss uses its own state machine instead of the base attack system
    }

}
