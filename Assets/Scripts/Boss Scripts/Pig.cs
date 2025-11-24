using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class Pig : MonoBehaviour
{
    [SerializeField] PigRider pigRider;

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

    [Header("Patrol Settings")]
    [Tooltip("Speed while patrolling side to side")]
    [SerializeField] private float patrolSpeed = 2f;
    [Tooltip("Distance to patrol left and right from starting point")]
    [SerializeField] private float patrolDistance = 1f;
    private float patrolDirection = 1f; // 1 for right, -1 for left
    private float leftBoundary;
    private float rightBoundary;

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

    private float damage = 1f;
    private float recoilForce = 2f;

    public enum State
    {
        Patrolling, Targeting, Charging, Returning
    }

    public State currentState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        thisCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ignoredColliders = new List<Collider2D>();

        startingPoint = new Vector2(transform.position.x, transform.position.y);
        leftBoundary = startingPoint.x - patrolDistance;
        rightBoundary = startingPoint.x + patrolDistance;

        FlipSprite();

        TransitionToPatrolling();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Patrolling:
                UpdatePatrolling();
                break;
            case State.Targeting:
                UpdateTargeting();
                break;
            case State.Charging:
                UpdateCharging();
                break;
            case State.Returning:
                UpdateReturning();
                break;
        }
    }

    /// <summary>
    /// Used to clear all colliders (players, enemies) ignored while not charging.
    /// </summary>
    private void clearIgnoredColliders()
    {
        for (int i = ignoredColliders.Count - 1; i >= 0; i--)
        {
            // Reenables collision
            Physics2D.IgnoreCollision(ignoredColliders[i], thisCollider, false);
            ignoredColliders.RemoveAt(i);
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

        if (pigRider != null && GameManager.Instance.player != null && !pigRider.IsMarked() && !GameManager.Instance.player.IsMarked())
        {
            TransitionToReturning();
        }
    }

    /// <summary>
    /// Pig patrols side to side between boundaries, flipping sprite based on direction.
    /// Transitions to targeting when a valid target is marked.
    /// </summary>
    private void UpdatePatrolling()
    {
        if (pigRider != null && GameManager.Instance.player != null)
        {
            bool validTarget = pigRider.IsMarked() || GameManager.Instance.player.IsMarked();
            if (validTarget)
            {
                TransitionToTargeting();
                return;
            }
        }

        Vector2 movement = new Vector2(patrolDirection * patrolSpeed, rb.linearVelocity.y);
        rb.linearVelocity = movement;

        if (patrolDirection > 0 && transform.position.x >= rightBoundary)
        {
            patrolDirection = -1f;
            FlipSprite();
        }
        else if (patrolDirection < 0 && transform.position.x <= leftBoundary)
        {
            patrolDirection = 1f;
            FlipSprite();
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

    private void UpdateTargeting()
    {
        // Script references are not set up properly.
        if (pigRider == null || GameManager.Instance.player == null)
        {
            return;
        }

        bool validTarget = pigRider.IsMarked() || GameManager.Instance.player.IsMarked();
        if (pigRider.IsMarked())
        {
            targetPosition = pigRider.transform.position;
        }
        else if (GameManager.Instance.player.IsMarked())
        {
            targetPosition = GameManager.Instance.player.transform.position;
        }

        if (validTarget)
        {
            TransitionToCharging();
        }
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

    private void TransitionToTargeting()
    {
        currentState = State.Targeting;
    }

    private void TransitionToReturning()
    {
        currentState = State.Returning;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    private void TransitionToPatrolling()
    {
        currentState = State.Patrolling;
    }

    private void TransitionToCharging()
    {
        currentState = State.Charging;
        // Reenable colliders when charging
        clearIgnoredColliders();

        chargeDirection = (targetPosition - (Vector2)transform.position).normalized;
        currentSpeed = baseSpeed;
    }

    /// <summary>
    /// Handles collision during charge state. Applies recoil, damage, and screen shake.
    /// Transitions to returning state after collision.
    /// </summary>
    /// <param name="collision">The collision data from OnCollisionEnter2D</param>
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
        if (collision.gameObject.CompareTag("Wall") && impulseSource != null)
        {
            impulseSource.GenerateImpulse(wallShakeForce);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.player.TakeDamage(damage);
            GameManager.Instance.player.removeMark();

            if (impulseSource != null)
            {
                impulseSource.GenerateImpulse(playersShakeForce);
            }
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            PigRider pigRider = collision.gameObject.GetComponent<PigRider>();

            if (pigRider != null)
            {
                pigRider.TakeDamage(damage);
                pigRider.removeMark();
            }

            if (impulseSource != null)
            {
                impulseSource.GenerateImpulse(enemyShakeForce);
            }
        }

        TransitionToReturning();
    }

    /// <summary>
    /// Decides what should happen depending on state and if collision is with wal or player.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        bool isPlayer = collision.gameObject.CompareTag("Player");
        bool isEnemy = collision.gameObject.CompareTag("Enemy");

        // When not charging, ignore collisions with player or other enemies.
        if (currentState != State.Charging && (isPlayer || isEnemy))
        {
            Physics2D.IgnoreCollision(collision.collider, thisCollider);
            ignoredColliders.Add(collision.collider);
        }
        // Normal charge mode
        if (currentState == State.Charging && (collision.gameObject.CompareTag("Wall") || isPlayer || isEnemy))
        {
            HandleCharge(collision);
        }
    }
}
