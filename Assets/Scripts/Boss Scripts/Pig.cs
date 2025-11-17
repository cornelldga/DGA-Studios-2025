using Unity.Cinemachine;
using UnityEngine;

public class Pig : MonoBehaviour
{
    Player player;
    [SerializeField] Pig_Rider pigRider;

    [Header("Screen Shake")]
    private CinemachineImpulseSource impulseSource;
    [Tooltip("Force multiplier for wall collision shake")]
    private float wallShakeForce = 1f;
    [Tooltip("Force multiplier for player collision shake")]
    private float playersShakeForce = 0.5f;

    [Header("Movement Settings")]
    //Base speed when charging (regular)
    private float baseSpeed = 5f;
    //Acceleration amount for charging (accelerating while charging).
    private float acceleration = 6f;
    //Maximum speed to cap given acceleration.
    private float maxChargeSpeed = 10f;

    private Vector2 chargeDirection;
    private Vector2 startingPoint;
    private float currentSpeed;
    private Rigidbody2D rb;

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
        player = GameManager.Instance.player;
        rb = GetComponent<Rigidbody2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        currentState = State.Patrolling;
        startingPoint = new Vector2(transform.position.x, transform.position.y);
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
    /// Boss behavior in charging mode. Accelerating to a max speed.
    /// </summary>
    private void UpdateCharging()
    {
        currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxChargeSpeed);
        rb.linearVelocity = chargeDirection * currentSpeed;

        //Remember to turn collisions back on between the boss and the player while charging 
        //They will need to be turned off again when switching to other states.
    }

    private void UpdatePatrolling()
    {

    }

    private void UpdateTargeting()
    {

    }

    private void UpdateReturning()
    {

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
        if (collision.gameObject.CompareTag("Wall") && impulseSource != null)
        {
            impulseSource.GenerateImpulse(wallShakeForce);
        }
        else if (collision.gameObject.CompareTag("Player") && impulseSource != null)
        {
            player.TakeDamage(damage);
            impulseSource.GenerateImpulse(playersShakeForce);
            player.removeMark();
        }
    }

    /// <summary>
    /// Decides what should happen depending on state and if collision is with wal or player.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Normal charge mode
        if (currentState == State.Charging && (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Player")))
        {
            HandleCharge(collision);
        }
    }
}
