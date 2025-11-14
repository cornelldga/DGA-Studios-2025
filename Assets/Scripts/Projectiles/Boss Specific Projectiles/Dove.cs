using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

/// <summary>
/// Enables a bullet to move in a sinusoidal pattern by adding forces perpendicular to its travel direction
/// </summary>
public class DoveMovement : Bullet
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float frequency;
    public float amplitude;
    private float timer;


    // Update is called once per frame
    private void FixedUpdate()
    {
        timer += 1f; 
        float waveForce = amplitude * Mathf.Cos(frequency * timer); 
        rb.AddForce(transform.up * waveForce, ForceMode2D.Impulse);
    }

    
}
