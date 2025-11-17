using System;
using UnityEngine;
using Unity.Cinemachine;

/// <summary>
/// Explosion after Driller Boss's dynamite is exploded
/// </summary>
public class DynamiteExplosion : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float explosionDuration;
    private double timer;
    
    void Start()
    {
    }

    /// <summary>
    /// Destroys itself when time is up
    /// </summary>
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > explosionDuration)
            Destroy(gameObject);
    }

    /// <summary>
    /// Deals damage to the player upon contact.
    /// Should do something to holes, not yet decided.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
        if (collision.CompareTag("Hole"))collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
        
    }

}
