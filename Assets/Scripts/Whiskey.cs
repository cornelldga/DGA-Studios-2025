using UnityEngine;

public class Whiskey : Projectile
{
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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
