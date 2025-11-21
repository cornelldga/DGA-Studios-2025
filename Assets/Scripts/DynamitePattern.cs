using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Dynamite used by Driller Boss: 
/// Will be thrown into holes (not yet implemented) and creates a ring explosion (circle for now)
/// </summary>
public class DynamitePattern : Projectile
{
    [SerializeField] GameObject dynamitePrefab;
    [SerializeField] float timeBeforeExplosion;
    private float timer;
    public void ThrowAt(Vector2 target, Boss boss)
    {
        timer += Time.deltaTime;
        if (timer > cooldown)
        {
            Vector2 start = boss.bulletOrigin.position;
            Vector2 end = target;

            GameObject dynamite = Instantiate(dynamitePrefab,start,boss.bulletOrigin.rotation);
            Rigidbody2D rb = dynamite.GetComponent<Rigidbody2D>();

            float g = Physics2D.gravity.y;
            float vx = (end.x - start.x) / timeBeforeExplosion;
            float vy = (end.y - start.y - 0.5f * g * timeBeforeExplosion * timeBeforeExplosion) / timeBeforeExplosion;
            // Vector2 velocity = new Vector2(vx, vy);
            rb.linearVelocityX = vx;
            rb.linearVelocityY = vy;

            timer = 0;
        }
       
    }
}
