using UnityEngine;

public class TutorialSpeedBullet : Bullet
{
    [SerializeField] Vector3 position;

    public override void OnProjectileHit(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.position = position;
        }
    }
}