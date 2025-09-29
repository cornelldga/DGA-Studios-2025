using System;
using UnityEngine;

public class Beer : MonoBehaviour
{
    private float baseSpeed = 10f;
    private float baseLifetime = 5f;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.right * baseSpeed, ForceMode2D.Impulse);
    }

    public void SetProjectile(float speed, float lifetime)
    {
        baseSpeed *= speed;
        baseLifetime *= lifetime;
    }

}
