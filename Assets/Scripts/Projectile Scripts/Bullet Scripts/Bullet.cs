using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// Projectiles that the bosses shoot
/// </summary>

public class Bullet : Projectile
{
    bool whipped;
    [Tooltip("True if the player whipped this projectile")]

    /// <summary>
    /// Sets this projectile as 'whipped' to reverse collision logic
    /// and sets its damage based on the whipDamageMultiplier
    /// </summary>
    public void WhipBullet(float whipDamageMultiplier)
    {
        gameObject.layer = LayerMask.NameToLayer("Base");
        damage *= whipDamageMultiplier;

        // Stop from homing to player if reflected
        Homing homingScript = this.GetComponent<Homing>();
        if (homingScript != null)
        {
            homingScript.enabled = false;
        }
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
        if (!collision.CompareTag("Whip"))
        {
            base.OnProjectileHit(collision);
        }
    }
}
