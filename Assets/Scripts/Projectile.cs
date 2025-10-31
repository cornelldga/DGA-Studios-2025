using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected float baseSpeed;
    [SerializeField] protected float baseLifetime;
    [SerializeField] protected float baseCooldown;
    [SerializeField] protected float baseDamage;
    [SerializeField] protected float baseAccuracy;

    private int damageMod;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        damageMod = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float getCooldown() { return baseCooldown; }

    public float getAccuracy() { return baseAccuracy; }

    public void SetDamageMod(int mod)
    {
        damageMod = mod;
    }

    public void ResetDamageMod()
    {
        damageMod = 1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<BossManager>().setHealth(collision.gameObject.GetComponent<BossManager>().getHealth() - 1 * damageMod);

            Destroy(this.gameObject);
        }
    }
}
