using UnityEngine;
using Unity.Cinemachine;

/// <summary>
/// Explosion after Driller Boss's dynamite is exploded. Releases 3 rocks on impact and 6 if it's a hole
/// </summary>
public class DynamiteExplosion : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float explosionDuration;
    [SerializeField] float impulseForce;
    [SerializeField] CinemachineImpulseSource impulseSource;
    [SerializeField] Bullet dynamiteBulletPrefab;

    bool hitHole;
    int angle = 0;


    /// <summary>
    /// Shakes screen upon spawning, and destroys itself after explosionDuration has passed.
    /// </summary>
    void Start()
    { 
        angle += Random.Range(0, 120);
        if (impulseSource != null)
            impulseSource.GenerateImpulse(impulseForce);
        for (int i = 0; i < 3; i++)
        {
            Bullet dynamiteBullet = Instantiate(dynamiteBulletPrefab, transform.position, Quaternion.identity);
            dynamiteBullet.transform.Rotate(new Vector3(0, 0, angle), Space.World);
            angle += 120;
        }
        Destroy(gameObject, explosionDuration);
    }

    /// <summary>
    /// Deals damage to the player and hole upon contact.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage);
        if (collision.CompareTag("Hole"))
        {
            angle += 60;
            for (int i = 0; i < 3; i++)
            {
                Bullet dynamiteBullet = Instantiate(dynamiteBulletPrefab, transform.position, Quaternion.identity);
                dynamiteBullet.transform.Rotate(new Vector3(0, 0, angle), Space.World);
                angle += 120;
            }
        }   
    }

    public void changeImpulse(float impulseForce) 
    {
        this.impulseForce = impulseForce;
    }
}
