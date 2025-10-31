using UnityEngine;

public class WhipCollisionHandler : MonoBehaviour
{
    private int damageMod;

    private void Start()
    {
        damageMod = 1;
    }

    public void SetDamageMod(int mod)
    {
        damageMod = mod;
    }

    public void ResetDamageMod()
    {
        damageMod = 1; 
    }
    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            collision.gameObject.tag = "PlayerBullet";
            float horizontal = collision.gameObject.GetComponent<BulletBehavior>().horizontal;
            float vertical = collision.gameObject.GetComponent<BulletBehavior>().vertical;
            collision.gameObject.GetComponent<BulletBehavior>().setDamageMod(damageMod);
            collision.gameObject.GetComponent<BulletBehavior>().horizontal = -horizontal;
            collision.gameObject.GetComponent <BulletBehavior>().vertical = -vertical;
        }
    }
}
