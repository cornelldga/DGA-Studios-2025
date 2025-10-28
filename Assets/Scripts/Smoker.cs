using UnityEngine;

public class Smoker : MonoBehaviour
{
    //How fast should the smoker spin. (Can be modified later for different patterns)
    private float spinSpeed = 50;
    //How fast should smoke be shot out of smoker pipe.
    private float pelletSpeed = 3;
    //A counter on how long it has been since the last smoke was shot.
    private float smokeTimer = 0f;
    //The smoke should be shot once this amount of time has elapsed.
    private float resetTime = 0.15f;
    [SerializeField] GameObject smokePelletPrefab;
    //Where the smoke should be released from.
    [SerializeField] Transform releasePoint;
    //A transform centered on the smoker to allow for 360 smoking.
    [SerializeField] Transform pivot;

    // Update is called once per frame
    /// <summary>
    /// On update, the smoke releast point is pivoted to shot in 360 degrees over time. Shootig only happens if enough time has passed.
    /// </summary>
    void Update()
    {
        pivot.transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
        smokeTimer += Time.deltaTime;
        if (smokeTimer >= resetTime)
        {
            ShootSmoke();
            smokeTimer = 0;
            resetTime = Random.Range(0.05f, 0.1f);

        }

    }
    /// <summary>
    /// Instantiates a pellet of smoke to be shot. It will rotate, and fire in the given direction.
    /// </summary>
    void ShootSmoke()
    {
        GameObject pellet = Instantiate(smokePelletPrefab, releasePoint.transform.position, Quaternion.identity);
        Rigidbody2D rb = pellet.GetComponent<Rigidbody2D>();
        pellet.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
        Vector2 direction = new Vector2(releasePoint.position.x - transform.position.x, releasePoint.position.y - transform.position.y).normalized;
            if (rb != null)
            {
            rb.linearVelocity = direction * pelletSpeed * Random.Range(0.7f, 1.0f);
            rb.angularVelocity = Random.Range(-30f, 30f);
            }
    }
}
