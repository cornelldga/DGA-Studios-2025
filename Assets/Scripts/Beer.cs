using System;
using UnityEngine;

public class Beer : MonoBehaviour
{
    public float baseSpeed = 10f;
    public float baseLifetime = 1f;
    public float baseCooldown = 0.5f;
    

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
