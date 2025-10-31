using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// The type of base
/// </summary>

public enum BaseType
{
    None,
    Lime,
    Pimiento,
    Ginger,
    Cider,
}


/// <summary>
/// Container for all bases that can be equipped and shot, using its stats and modifiers of the mixer
/// </summary>
public class PlayerBases : MonoBehaviour
{
    [SerializeField] Base[] bases;
    Dictionary<BaseType, Base> basesDict = new Dictionary<BaseType, Base>();

    Base selectedBase;

    private void Start()
    {
        if (bases.Length != sizeof(BaseType))
        {
            throw new InvalidImplementationException("Base array must match the size of BaseType and in order of base type");
        }
        for (int i = 0; i < bases.Length; i++)
        {
            basesDict[(BaseType)i] = bases[i];
        }
    }
    /// <summary>
    /// Sets the selected base
    /// </summary>
    /// <param name="type"></param>
    public void SelectBase(BaseType type)
    {
        selectedBase = basesDict[type];
    }
    /// <summary>
    /// Gets the selected base
    /// </summary>
    /// <returns>The selected base</returns>
    public Base GetBase()
    {
        return selectedBase;
    }
   
}
/// <summary>
/// A Base is a projectile that a player equips and fires, using mixer modifiers to affect certain projectile properties
/// </summary>
public class Base : Projectile
{
        
}

/// <summary>
/// A moving object that checks for a collision and applies damage
/// </summary>
public class Projectile : MonoBehaviour
{
    [SerializeField] string collisionTag;
    public float speed;
    public float lifeDuration;
    public float cooldown;
    public float damage;
    [Tooltip("0 is perfect accuracy")]
    [Range(0,180)]
    public float accuracy;

    Rigidbody2D rb;
    private void Start()
    {
        rb.GetComponent<Rigidbody2D>();
        transform.Rotate(0, 0, Random.Range(-accuracy, accuracy));
        rb.AddForce(transform.forward * speed, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(collisionTag))
        {
            Debug.Log(damage);
            //collision.GetComponent<IDamageable>(damage);
        }
    }


}
