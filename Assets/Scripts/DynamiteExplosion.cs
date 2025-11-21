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
    private double timer;
    void Start()
    {
        impulseSource.GenerateImpulse(impulseForce);
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
    /// Currently removes hole upon contact. Might change later.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
        if (collision.CompareTag("Hole"))collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
    }
}
