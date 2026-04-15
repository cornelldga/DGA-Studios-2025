using UnityEngine;

public class Whip : MonoBehaviour, IProjectileInteractable
{
    [SerializeField] float whipSpeedMultiplier;
    public float damageMultiplier;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<Bullet>(out Bullet bullet))
        {
            if (!bullet.Whipped())
            {
                bullet.WhipBullet(damageMultiplier);
                collision.GetComponent<Rigidbody2D>().linearVelocity = -whipSpeedMultiplier * collision.GetComponent<Rigidbody2D>().linearVelocity;
            }
        }

        else if(collision.gameObject.TryGetComponent<Bush>(out Bush bush))
        {
           if(bush.isFire())
            {
                bush.WhipBush();
            }
        }
        
        
    }
    /// <summary>
    /// Ends the whip
    /// </summary>
    public void EndWhip()
    {
        GameManager.Instance.player.AnimationEndWhip();
    }

    public bool ProjectileInteraction(Projectile projectile)
    {
        if (projectile.gameObject.TryGetComponent<Bullet>(out Bullet bullet))
        {
            if (!bullet.Whipped())
            {
                bullet.WhipBullet(damageMultiplier);
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                rb.linearVelocity = -whipSpeedMultiplier * rb.linearVelocity;
            }
        }
        return false;
    }
}