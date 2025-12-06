using System.Collections;
using UnityEngine;

/// <summary>
/// Dynamite used by Driller Boss: 
/// Can be thrown at n targeted locations with specified path time.
/// </summary>
public class DynamitePattern : Projectile
{
    public float duration; //duration can be changed
    [SerializeField] float heightY;
    [SerializeField] GameObject dynamitePrefab;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] AnimationCurve curve;

    /// <summary>
    /// Spawns a dynamite and make it travel in a prabolic path using animation curve.
    /// At the end of the path, spawn an explosion
    /// </summary>
    /// <param name="target">Target location of the parabolic path </param>
    /// <param name="boss"></param>
    /// <returns></returns>
    public IEnumerator ThrowRoutine (Vector3 start, Vector3 target)

    {
        //spawns dynamite object
        GameObject dynamite = Instantiate(dynamitePrefab, start, Quaternion.identity);
        float timer = 0;

        //parabolic path
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float linearT = timer/duration; // between 0 and 1
            float heightT = curve.Evaluate(linearT);//value from curve
            float height = Mathf.Lerp(0, heightY, heightT); //interpolates b/w 0 and heightY
            dynamite.transform.position =Vector3.Lerp(start, target, linearT) + new Vector3(0f, height, 0f);
            yield return null; //this waits for next frame
        }

        //spawns explosion
        Instantiate(explosionPrefab, dynamite.transform.position, Quaternion.identity);
        Destroy(dynamite);
    }
}
