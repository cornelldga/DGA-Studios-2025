using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Projectile thrown by the boss.
/// </summary>
public class Dynamite : MonoBehaviour
{
    private double timer;
    [SerializeField] float timeBeforeEplosion; //should be same as dynamite pattern's. not sure how to change
    [SerializeField] GameObject explosionPrefab;


    /// <summary>
    /// After a certain amoutn of time, spawn an explosion and delete the dynamite.
    /// This time should be the same time used to calculate dynamite path in dynamite pattern.
    /// </summary>
    void Update()
    {
         timer += Time.deltaTime;
         if (timer> timeBeforeEplosion)
         {
             Instantiate(explosionPrefab, transform.position, Quaternion.identity);
             Destroy(gameObject);
            }
    }

    /// <summary>
    /// This projectile shouldn't damage the player. 
    /// Maybe trigger something if it hits a hole?
    /// </summary>
    public virtual void OnProjectileHit(Collider2D collision)
    {
        //do nothing for now
    }
}
