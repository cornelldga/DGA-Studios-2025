using UnityEngine;
using System.Collections;

/// <summary>
/// Represents a single hole in the ground object caused from Drill boss entering and exiting states
/// </summary>
public class DrillHole : MonoBehaviour, IDamageable
{
    // if this hole is initialized exiting underground
    private Rigidbody2D rb;
    [SerializeField] private bool emersionHole;

    [Header("Rock Splash Settings")]
    // change to rock debris when possible
    [SerializeField] private Base debrisPrefab;
    [SerializeField] private float spawnOffset = 0.2f;

    // the radius of the splash attack, to be passed onto the rock debris objects
    [SerializeField] private float maxRadius = 3f;
    [SerializeField] private int debrisCount = 10;
    [SerializeField] private float debrisSpeed = 5f;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // TODO: Should the emersion rock debris damage be called here?
        if (emersionHole)
        {
            StartCoroutine(BlastDebris());
        }
    }

    public void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        Destroy(gameObject);
    }

    public Vector3 GetPosition()
    {
        return rb.position;
    }
    
    /// <summary>
    /// Initializes debris projectiles evenly in a ring around the hole that damage player
    /// Should use maxRadius to specify how far projectiles will travel
    /// </summary>
    /// <returns></returns>
    private IEnumerator BlastDebris()
    {
        for (int i = 0; i < debrisCount; i++)
        {
            float angle = i * (360f / debrisCount) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * spawnOffset;
            Vector3 spawnPosition = transform.position + offset;
            Vector3 velocity = offset.normalized * debrisSpeed;
            Quaternion direction = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg);

            // TODO: instantiate the rock Debris objects, after we do PRs for the other objects
            Base debris = Instantiate(debrisPrefab, spawnPosition, direction);
            Rigidbody2D debrisRb = debris.GetComponent<Rigidbody2D>();
            if (debrisRb != null)
            {
                debrisRb.linearVelocity = velocity;
            }
        }
        yield return null;
    }
}
