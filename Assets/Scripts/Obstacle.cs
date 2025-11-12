using UnityEngine;

/// <summary>
/// A physical obstacle present on the boss field.
/// This obstacle is destroyable, either by player/NPC action
/// </summary>
public abstract class Obstacle : MonoBehaviour
{
    protected Rigidbody2D rb;
    // whether this obstacle's physics/damage should be active
    protected bool active;
    // whether this obstacle should be destroyed once inactive
    protected bool hasInactivePeriod;
    protected bool doesDamage;

    // by default im just gonna say obstacles are destroyed once inactive
    // and do NOT do damage
    // children of this can override
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        active = true;
        doesDamage = false;
        hasInactivePeriod = false;
    }

    public virtual void Update()
    {
        if (!active)
        {
            Destroy(gameObject);
        }
    }
    public Vector3 GetPosition()
    {
        return rb.position;
    }

    public virtual void Deactivate()
    {
        active = false;
    }
}
