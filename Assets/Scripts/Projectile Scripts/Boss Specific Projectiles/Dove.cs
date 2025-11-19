using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

/// <summary>
/// Enables a bullet to move in a sinusoidal pattern by adding forces perpendicular to its travel direction
/// </summary>
public class Dove : Bullet
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Tooltip("Width of the Projectile's arcs")]
    public float frequency;
    [Tooltip("Height of the Projectile's arcs")]
    public float amplitude;
    private float timer;

    // Update is called once per frame

    private void FixedUpdate()
    {
        timer += Time.deltaTime; 
        float waveForce = amplitude * Mathf.Cos(2 * Mathf.PI / frequency * timer); 
        //rb.linearVelocityY = waveForce;
        rb.AddForce(transform.up * waveForce, ForceMode2D.Force);
    }
}
