using Unity.VisualScripting;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float bulletSpeed;
    public float vertical;
    public float horizontal;
    public float bulletLife;
    [SerializeField] private Rigidbody2D rb;

    private int damageMod;
    void Start()
    {
        damageMod = 1;
    }

    public void setDamageMod(int mod)
    {
        damageMod = mod;
    }

    // Update is called once per frame
    void Update()
    {
        bulletLife -= Time.deltaTime;
        if (bulletLife <= 0 )
        {
            Destroy(this.gameObject);
        }

    }
    private void FixedUpdate()
    {
       rb.linearVelocity = new Vector2(bulletSpeed * horizontal * Time.deltaTime, bulletSpeed * vertical * Time.deltaTime);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Destroy(this.gameObject);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(this.gameObject);
        }
        if (this.gameObject.CompareTag("PlayerBullet") && collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<BossManager>().setHealth(collision.gameObject.GetComponent<BossManager>().getHealth() - 1 * damageMod);
            Destroy(this.gameObject);
        }
    }
}
/* 
 To do in file:
 
 */
