using UnityEngine;

public class Whip : MonoBehaviour
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
        }
}