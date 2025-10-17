using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected float baseSpeed;
    [SerializeField] protected float baseLifetime;
    [SerializeField] protected float baseCooldown;
    [SerializeField] protected float baseDamage;
    [SerializeField] protected float baseAccuracy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float getCooldown() { return baseCooldown; }

    public float getAccuracy() { return baseAccuracy; }
}
