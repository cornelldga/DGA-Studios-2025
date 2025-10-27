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
    }

    // Update is called once per frame
    void Update()
    {
        if (isCurved)
        {
            horizSpeedMod = Mathf.Cos(Time.time * angle);
        }
    }
    private void FixedUpdate()
    {
        BulletMove();
    }

    public void BulletMove()
    {
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
        
    }
}
/* 
 To do in file:
 Sine wave
 Variable bullet speed
 Bullet shapes
 */
