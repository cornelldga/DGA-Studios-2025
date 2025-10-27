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
    public float angle;
    public float bulletLife;
    public bool isCurved;
    private float horizSpeedMod = 1f;
    private float vertSpeedMod = 1f;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private BulletScriptable Bullet;

    void Start()
    {
        bulletSpeed = Bullet.bulletSpeed;
        horizontal = Bullet.horizontal;
        vertical = Bullet.vertical;
        bulletLife = Bullet.bulletLife;
        rb = Bullet.rb;
        angle = Bullet.angle;
        isCurved = Bullet.isCurved;
    }

    // Update is called once per frame
    void Update()
    {
        bulletLife -= Time.deltaTime;
        if (bulletLife <= 0 )
        {
            Destroy(this.gameObject);
        }
        if (isCurved)
        {
            horizSpeedMod = Mathf.Cos(bulletLife * angle);
            ///print(horizontal);
        }
    }
    private void FixedUpdate()
    {
            rb.linearVelocity = new Vector2(bulletSpeed * horizontal * horizSpeedMod * Time.deltaTime, bulletSpeed * vertical * vertSpeedMod * Time.deltaTime);
            print(rb.linearVelocityX);
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
        
    }
}
/* 
 To do in file:
 Sine wave
 Variable bullet speed
 Bullet shapes
 */
