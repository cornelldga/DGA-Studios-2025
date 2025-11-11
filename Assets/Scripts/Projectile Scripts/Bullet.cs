using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// Projectiles that the bosses shoot
/// </summary>

public class Bullet : Projectile
{
    [Tooltip("True if the player whipped this projectile")]
    bool whipped;

    /// <summary>
    /// Sets this projectile as 'whipped' to reverse collision logic
    /// and sets its damage based on the whipDamageMultiplier
    /// </summary>
    public void WhipProjectile(float whipDamageMultiplier)
    {
        whipped = true;
        damage *= whipDamageMultiplier;
    }

    /// <summary>
    /// Returns if this projectile was whipped
    /// </summary>
    public bool Whipped()
    {
        return whipped;
    }
    public override void OnProjectileHit(Collider2D collision)
    {
        if(whipped && collision.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
        }
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
        }
        base.OnProjectileHit(collision);
    }
}
