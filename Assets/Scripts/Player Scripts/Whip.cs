using UnityEngine;

public class Whip : MonoBehaviour, IProjectileInteractable
{
    [SerializeField] float whipSpeedMultiplier;
    public float damageMultiplier;
    public bool reflecting = false;
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
            if(reflecting && !bullet.Whipped())
            {
                bullet.WhipBullet(damageMultiplier);
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                rb.linearVelocity = -whipSpeedMultiplier * rb.linearVelocity;
            } else
            {
                //figure out way to destroy bullets
                Destroy(projectile.gameObject);
            }

        }
        return false;
    }
}