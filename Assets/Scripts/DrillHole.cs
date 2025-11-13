using UnityEngine;
using System.Collections;

/// <summary>
/// Represents a single hole in the ground object caused from Drill boss entering and exiting states
/// </summary>
public class DrillHole : Obstacle, IDamageable
{
    // if this hole is initialized exiting underground
    private bool emersionHole;

    [Header("Rock Splash Settings")]
    [SerializeField] private float spawnOffset = 0.2f;

    // pass this onto the rock debris objects, its the radius of the splash attack
    [SerializeField] private float radius = 3f;
    [SerializeField] private int debrisCount = 10;
    [SerializeField] private float debrisSpeed = 5f;

    void Start()
    {
        base.Start();
        // TODO: Should the emersion rock debris damage be called here?
        if (emersionHole)
        {
            StartCoroutine(BlastDebris());
        }
    }

    void Update()
    {
        base.Update();
    }

    public void TakeDamage(float damage)
    {
        Deactivate();
    }
    
    private IEnumerator BlastDebris()
    {
        for (int i = 0; i < debrisCount; i++)
        {
            float angle = i * (360f / debrisCount) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * spawnOffset;
            Vector3 spawnPosition = transform.position + offset;
            Vector3 velocity = offset.normalized * debrisSpeed;

            // TODO: instantiate the rock Debris objects, after we do PRs for the other objects
        }
        yield return null;
    }
}
