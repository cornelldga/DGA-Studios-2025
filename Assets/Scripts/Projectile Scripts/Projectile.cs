using UnityEngine;

/// <summary>
/// A moving object that checks for a collision and applies damage
/// </summary>
public abstract class Projectile : MonoBehaviour
{
    [Header("Stats")]
    public float speed;
    public float lifeDuration;
    public float cooldown;
    public float damage;
    [Tooltip("0 is perfect accuracy")]
    [Range(0, 180)]
    public float accuracy;
    [Tooltip("The layers that the projectiles will destroy themselves on impact")]
    public LayerMask collisionLayers;



    protected Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.Rotate(0, 0, Random.Range(-accuracy, accuracy));
        rb.AddForce(transform.right * speed, ForceMode2D.Impulse);
        Destroy(gameObject, lifeDuration);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnProjectileHit(collision);
    }
    /// <summary>
    /// Logic for when a projectile hits an object with a collider
    /// </summary>
    public virtual void OnProjectileHit(Collider2D collision)
    {
        if (Physics2D.Raycast(transform.position, transform.right, 
            transform.localScale.magnitude, collisionLayers))
        {
            Destroy(gameObject);
        }
    }
}