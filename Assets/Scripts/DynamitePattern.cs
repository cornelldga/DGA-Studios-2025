using UnityEngine;

/// <summary>
/// Dynamite used by Driller Boss: 
/// Can be thrown at n targeted locations with specified path time.
/// </summary>
public class DynamitePattern : Projectile
{
    [SerializeField] GameObject dynamitePrefab;
    [SerializeField] float timeBeforeExplosion;
    private float timer;
    public void ThrowAt(Vector2[] targets, Boss boss)
    {
        timer += Time.deltaTime;
        if (timer > cooldown)
        {
            foreach (Vector2 target in targets)
            {
                Vector2 start = boss.bulletOrigin.position;
                Vector2 end = target;

                GameObject dynamite = Instantiate(dynamitePrefab,start,boss.bulletOrigin.rotation);
                Rigidbody2D rb = dynamite.GetComponent<Rigidbody2D>();

                float g = Physics2D.gravity.y;
                float vx = (end.x - start.x) / timeBeforeExplosion;
                float vy = (end.y - start.y - 0.5f * g * timeBeforeExplosion * timeBeforeExplosion) / timeBeforeExplosion;
                rb.linearVelocityX = vx;
                rb.linearVelocityY = vy;
            }   
            timer = 0;
        }  
    }
}
