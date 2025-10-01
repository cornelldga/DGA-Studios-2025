using UnityEngine;

public class Gin : MonoBehaviour
{
    public float baseSpeed = 20f;
    public float baseLifetime = 2f;
    public float baseCooldown = 1.5f;

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
