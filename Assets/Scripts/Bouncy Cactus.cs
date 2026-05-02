using System.Collections;
using UnityEngine;
/// <summary>
/// Bounces objects and projectiles off of the cactus
/// </summary>
public class BouncyCactus : MonoBehaviour, IProjectileInteractable
{

    Animator animator;
    [SerializeField] float bounceWaitTime;
    [SerializeField] float bounceForceStrength;
    [Tooltip("random rotation of applied force")]
    [SerializeField] float ranDeflectionRange;

    [SerializeField] float playerFreezeTime;

    float bounceTimeLeft;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Vector2 rotate function found on Unity forum
    /// </summary>
    /// <param name="v">The vector to be rotated</param>
    /// <param name="delta">The rotation amount</param>
    /// <returns></returns>
    public static Vector2 Rotate(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    /// <summary>
    /// Flips the rigidbody of the colliding object away from the cactus at a specified bounce force
    /// </summary>
    void Bounce(Rigidbody2D rb)
    {
        Debug.Log("Bounce");
        Vector2 bounceDir = (rb.position - new Vector2(transform.position.x, transform.position.y)).normalized;
        bounceDir = Rotate(bounceDir, Random.Range(-ranDeflectionRange, ranDeflectionRange) * Mathf.Deg2Rad);
        rb.linearVelocity = bounceDir * bounceForceStrength;

    }
    /// <summary>
    /// Starts the bounce animation and resets the animation timer
    /// </summary>
    void SetBounceAnimation()
    {
        animator.SetBool("Bounce", true);
        bounceTimeLeft = bounceWaitTime;
    }

    public bool ProjectileInteraction(Projectile projectile)
    {
        SetBounceAnimation();
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        Vector2 bounceDir = (rb.position - new Vector2(transform.position.x, transform.position.y)).normalized;

        float angle = Mathf.Atan2(bounceDir.y, bounceDir.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0f, 0f, angle + Random.Range(-ranDeflectionRange, ranDeflectionRange));
        rb.linearVelocity = rb.linearVelocity.magnitude * projectile.transform.right;
        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(nameof(BouncePlayer));
        }
        if (collision.gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            Bounce(rb);
            SetBounceAnimation();
        }
    }


    IEnumerator BouncePlayer()
    {
        GameManager.Instance.FreezePlayer(true);
        yield return new WaitForSeconds(bounceWaitTime);
        GameManager.Instance.FreezePlayer(false);
    }


    private void Update()
    {
        bounceTimeLeft -= Time.deltaTime;
    }

    public void AnimationBounceComplete()
    {
        if ( bounceTimeLeft <= 0)
        {
            animator.SetBool("Bounce", false);
        }
    }
}
