using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float bulletSpeed;
    public float vertical;
    public float horizontal;
    private int damageMod;

    public void setDamageMod(int mod)
    {
        damageMod = mod;
    }
    
    public float angle;
    public bool isCurved;
    private float horizSpeedMod = 1f;
    private float vertSpeedMod = 1f;
    Rigidbody2D rb;

    [SerializeField] private BulletScriptable Bullet;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bulletSpeed = Bullet.bulletSpeed;
        horizontal = Bullet.horizontal;
        vertical = Bullet.vertical;
        Destroy(gameObject, Bullet.bulletLife);
        angle = Bullet.angle;
        isCurved = Bullet.isCurved;
        damageMod = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        BulletMove();
    }

    public void BulletMove()
    {
        if (isCurved)
        {
            horizSpeedMod = Mathf.Cos(Time.time * angle) * 10;
        }
        /*
        Vector2 forward = transform.forward;
        Vector2 perp = transform.right;
        */
        rb.linearVelocity = new Vector2(bulletSpeed * horizontal * horizSpeedMod * Time.deltaTime, bulletSpeed * vertical * vertSpeedMod * Time.deltaTime);
        
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
 Sine wave
 Variable bullet speed
 Bullet shapes
 */
