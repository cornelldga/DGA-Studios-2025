using System.Collections;
using NUnit.Framework.Constraints;
using UnityEngine;

/// <summary>
/// Molotov used by Ash: 
/// Can be thrown at n targeted locations with specified path time.
/// </summary>
public class Molotov : Projectile
{
    [SerializeField] float heightY;
    [SerializeField] GameObject molotovPrefab;
    [SerializeField] AnimationCurve curve;
    [SerializeField] float collisionRadius = 0.5f;

    /// <summary>
    /// Spawns a molotov and make it travel in a prabolic path using animation curve.
    /// </summary>
    /// <param name="target">Target location of the parabolic path </param>
    /// <param name="boss"></param>
    /// <returns></returns>
    public IEnumerator ThrowRoutine (Vector3 start, Vector3 target)
    {
        //spawns dynamite object
        GameObject molotov = Instantiate(molotovPrefab, start, Quaternion.identity);
        float timer = 0;

        //parabolic path
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float linearT = timer/duration; // between 0 and 1
            float heightT = curve.Evaluate(linearT);//value from curve
            float height = Mathf.Lerp(0, heightY, heightT); //interpolates b/w 0 and heightY
            molotov.transform.position =Vector3.Lerp(start, target, linearT) + new Vector3(0f, height, 0f);
            yield return null; //this waits for next frame
        }

        //get bush collided and set it on fire
        Collider2D[] colliders = Physics2D.OverlapCircleAll(molotov.transform.position, collisionRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Bush"))
            {
                Debug.Log("Molotov hit a bush!");
                Bush bush = collider.GetComponent<Bush>();
                bush.setFire(true);
            }
        }
        Destroy(molotov);
    }
}
