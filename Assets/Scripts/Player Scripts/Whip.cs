using UnityEngine;

public class Whip : MonoBehaviour, IProjectileInteractable
{
    [SerializeField] float whipSpeedMultiplier;
    [SerializeField] ParticleSystem destroyParticle;
    public float damageMultiplier;
    public bool reflecting = false;
    public int frameNumber = 0;
    /// <summary>
    /// Ends the whip
    /// </summary>
    public void EndWhip()
    {
        GameManager.Instance.player.AnimationEndWhip();
    }

    public void EnableReflect()
    {
        reflecting = true;
    }

    public void DisableReflect()
    {
        reflecting = false;
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
                destroyParticle.Play();
                Destroy(projectile.gameObject);
            }

        }
        return false;
    }
}