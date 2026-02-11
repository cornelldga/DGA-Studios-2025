using System.IO.Pipes;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// A moving object that checks for a collision and applies damage
/// </summary>
public abstract class Projectile : MonoBehaviour
{
    [Header("Stats")]
    public float speed;
    public float duration;
    public float damage;
    [Tooltip("0 is perfect accuracy")]
    [Range(0, 180)]
    public float accuracy;
    [Tooltip("The sprite that's created on impact")]
    [SerializeField] ImpactSprite impactSprite;



    protected Rigidbody2D rb;
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.Rotate(0, 0, Random.Range(-accuracy, accuracy));
        rb.AddForce(transform.right * speed, ForceMode2D.Impulse);
        Destroy(gameObject, duration);
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
        if (impactSprite != null)
        {
            Instantiate(impactSprite, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}