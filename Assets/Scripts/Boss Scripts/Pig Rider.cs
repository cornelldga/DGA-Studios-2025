using System.Collections;
using UnityEditorInternal;
using UnityEngine;
using Unity.Cinemachine;

public enum State
{
    Charging, Targeting, Stunned, Marking, Bouncing
}

public class Pig_Rider : Boss
{
    [Header("Movement Settings")]
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float acceleration = 6f;
    [SerializeField] private float maxChargeSpeed = 10f;
    [SerializeField] private float impulseWeight = 1f;


    [Header("State Timing")]
    [SerializeField] private float targetingTime = 1f;
    [SerializeField] private float stunnedTime = 1f;
    [SerializeField] private float recoilForce = 2f;
    [Header("Attack Settings")]
    [Tooltip("Chance (0-1) that boss will mark instead of charge")]
    [SerializeField] private float markChance = 0.3f;
    [Tooltip("Chance (0-1) that boss will enter bounce mode instead of normal charge")]
    [SerializeField] private float bounceChance = 0.2f;
    [Tooltip("Chance (0-1) that boss will enter enraged bounce mode when below half health")]
    [SerializeField] private float enragedBounceChance = 1f;
    private float bounceSpeed;
    [SerializeField] private float baseBounceSpeed = 10f;


    [SerializeField] private float bouncesRemaining = 5f;
    [SerializeField] private BulletPattern markingBulletPattern;

    [Header("Bounce Mode Settings")]
    [Tooltip("Number of bounces charges during bouncing mode")]
    [SerializeField] private int bounces = 5;
    private bool isEnraged = false;
    private float damage = 1f;

    [Header("Screen Shake")]
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [Tooltip("Force multiplier for wall collision shake")]
    [SerializeField] private float wallShakeForce = 1f;
    [SerializeField] private float playersShakeForce = 0.5f;


    [Header("Victory Spin")]
    [Tooltip("Enable spin animation after successful marking attack")]
    [SerializeField] private bool enableVictorySpin = true;
    [SerializeField] private float spinDuration = 1f;
    [SerializeField] private float spinSpeed = 360;

    public State currentState;

    private Vector2 targetPosition;
    private Vector2 chargeDirection;
    private float currentSpeed;
    private float stateTimer;
    private Rigidbody2D rb;

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

        // Use Continuous collision detection for fast-moving objects
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        currentState = State.Targeting;
        stateTimer = targetingTime;
        bounceSpeed = baseBounceSpeed;

    }

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

    private void UpdateCharging()
    {
        currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxChargeSpeed);
        rb.linearVelocity = chargeDirection * currentSpeed;
    }
    private void UpdateBouncing()
    {
        currentSpeed = bounceSpeed;
        rb.linearVelocity = chargeDirection * currentSpeed;
    }



    private void UpdateStunned()
    {
        // Stop movement while stunned (recoil will gradually slow down)
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.deltaTime * 5f);

        // When stun time is up, go back to targeting
        if (stateTimer <= 0)
        {
            TransitionToTargeting();
        }
    }
   

    private void TransitionToTargeting()
    {
        currentState = State.Targeting;
        stateTimer = targetingTime;
        rb.linearVelocity = Vector2.zero;
    }

    private void TransitionToCharging()
    {
        currentState = State.Charging;

        chargeDirection =(targetPosition - (Vector2)transform.position).normalized;
        currentSpeed = baseSpeed;
    }

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
    private void TransitionToBouncing()
    {
        currentState = State.Bouncing;
        chargeDirection = (targetPosition - (Vector2)transform.position).normalized;
        bouncesRemaining = Random.Range(3, 7);
    }
    
    /// <summary>
    /// Sets the phase of pig rider based on its health percentage 
    /// </summary>
    /// <param name="healthPercent">Current health of pig rider as a percent of its maxHealth</param>
    public override void SetPhase(float healthPercent)
    {
        base.SetPhase(healthPercent);
        
        if (healthPercent <= 0.5f)
        {
            isEnraged = true;
            bounceChance = enragedBounceChance;
        }
    }




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

        // Victory spin after successful marking
        if (enableVictorySpin)
        {
            yield return StartCoroutine(VictorySpin());
        }

        // After marking (and spinning), return to targeting
        TransitionToTargeting();
    }

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

            if (collision.gameObject.CompareTag("Player") && impulseSource != null)
            {
                impulseSource.GenerateImpulse(playersShakeForce);
                collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.AddForce(chargeDirection * impulseWeight, ForceMode2D.Impulse);
                }
            }

            TransitionToStunned();
        }
        if (currentState == State.Bouncing && (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Player") ))
        {
            if (impulseSource != null)
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

            // Reflect the charge direction off the wall
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
        // Override to prevent base behavior
    }

}
