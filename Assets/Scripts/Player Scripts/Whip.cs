using UnityEngine;

public class Whip : MonoBehaviour
{

    public float damageMultiplier;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            Bullet projectile = collision.gameObject.GetComponent<Bullet>();
            if (!projectile.Whipped())
            {
                projectile.WhipProjectile(damageMultiplier);
                collision.GetComponent<Rigidbody2D>().linearVelocity = -4 * collision.GetComponent<Rigidbody2D>().linearVelocity;
            }
        }
        }
}