using System;
using UnityEngine;
using Unity.Cinemachine;
using System.Threading;

/// <summary>
/// Explosion after Driller Boss's dynamite is exploded
/// </summary>
public class DynamiteExplosion : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float explosionDuration;
    [SerializeField] float impulseForce;
    [SerializeField] CinemachineImpulseSource impulseSource;
    
    /// <summary>
    /// Shakes screen upon spawning, and destroys itself after explosionDuration has passed.
    /// </summary>
    void Start()
    { 
        if (impulseSource != null)
            impulseSource.GenerateImpulse(impulseForce);
        Destroy(gameObject, explosionDuration);
    }

    /// <summary>
    /// Deals damage to the player and hole upon contact.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
        if (collision.CompareTag("Hole"))collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
    }
}
