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

    [SerializeField] float playerFreezeTime;

    float bounceTimeLeft;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Flips the rigidbody of the colliding object away from the cactus at a specified bounce force
    /// </summary>
    void Bounce(Rigidbody2D rb)
    {
        Vector2 bounceDir = (rb.position - new Vector2(transform.position.x, transform.position.y)).normalized;
        Debug.Log(rb.linearVelocity);
        rb.linearVelocity = bounceDir * bounceForceStrength;
        Debug.Log(rb.linearVelocity);
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
        projectile.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        rb.linearVelocity = bounceDir * rb.linearVelocity.magnitude;
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
