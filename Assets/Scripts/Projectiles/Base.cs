using UnityEngine;

/// <summary>
/// A Base is a projectile that a player equips and fires, using mixer modifiers to affect certain projectile properties
/// </summary>
public class Base : Projectile
{
    public override void OnProjectileHit(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
            Destroy(gameObject);
        }
        base.OnProjectileHit(collision);
    }
}