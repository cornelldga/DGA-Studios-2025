using UnityEngine;

public class Whip : MonoBehaviour
{

    public float damageMultiplier;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            EnemyProjectile projectile = collision.gameObject.GetComponent<EnemyProjectile>();
            if (!projectile.Whipped())
            {
                projectile.WhipProjectile(damageMultiplier);
                collision.transform.RotateAround(transform.position, transform.up, 180f); ;
            }
        }
    }
}