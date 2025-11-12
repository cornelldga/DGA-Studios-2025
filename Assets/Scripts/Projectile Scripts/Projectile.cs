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
    public float lifeDuration;
    public float cooldown;
    public float damage;
    public int extraBullets;
    [Tooltip("0 is perfect accuracy")]
    [Range(0, 180)]
    public float accuracy;
    [Tooltip("The layers that the projectiles will destroy themselves on impact")]
    public LayerMask collisionLayers;



    Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.Rotate(0, 0, Random.Range(-accuracy, accuracy));
        rb.AddForce(transform.right * speed, ForceMode2D.Impulse);
        float angleChange = 5;
        for (int i = 0; i < extraBullets; i++)
        {
            if (i % 2 == 0)
            {
                float newAngle = transform.rotation.eulerAngles.z - angleChange;
                Quaternion newRotation = Quaternion.Euler(0, 0, newAngle);
                Projectile proj = Instantiate(this, transform.position, newRotation);
                proj.extraBullets = 0;
            }
            if (i % 2 == 1)
            {
                float newAngle = transform.rotation.eulerAngles.z + angleChange;
                Quaternion newRotation = Quaternion.Euler(0, 0, newAngle);
                Projectile proj = Instantiate(this, transform.position, newRotation);
                proj.extraBullets = 0;
                angleChange += 5;
            }
        }
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