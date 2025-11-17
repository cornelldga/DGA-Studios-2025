using Unity.Cinemachine;
using UnityEngine;

public class Pig : MonoBehaviour, IDamageable
{
    Player player;
    Boss pigRider;
    private CinemachineImpulseSource impulseSource;
    private Vector2 chargeDirection;
    private float currentSpeed;
    private Rigidbody2D rb;

    //Base speed when charging (regular)
    private float baseSpeed = 5f;
    //Acceleration amount for charging (accelerating while charging).
    private float acceleration = 6f;
    //Maximum speed to cap given acceleration.
    private float maxChargeSpeed = 10f;
    private Vector2 startingPoint;

    State currentState;

    public enum State
    {
        Charging, Targeting, Patrolling
    }

    public void TakeDamage(float damage)
    {
        //TODO: damage logic here
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameManager.Instance.player;
        rb = GetComponent<Rigidbody2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        currentState = State.Patrolling;
        startingPoint = new Vector2(transform.position.x, transform.position.y);
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
    /// Decides what should happen depending on state and if collision is with wal or player.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Normal charge mode - get stunned on collision
        if (currentState == State.Charging && (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Player")))
        {
            HandleCharge(collision);
            TransitionToStunned();
        }
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
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
            impulseSource.GenerateImpulse(playersShakeForce);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player.IsMarked())
        {
            //enter charging
            //then charge
        }
    }

    //hitting marked player unmarks them
}
