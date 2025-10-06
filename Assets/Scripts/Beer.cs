using System;
using UnityEngine;

public class Beer : Projectile
{
    private Rigidbody2D rb;

    private void Start()
    {
        Destroy(gameObject, baseLifetime);
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.right * baseSpeed, ForceMode2D.Impulse);
    }

    public void SetProjectile(float speed, float lifetime)
    {
        baseSpeed *= speed;
        baseLifetime *= lifetime;
    }

}
