using System.Collections;
using UnityEngine;
/// <summary>
/// Bounces objects and projectiles off of the cactus
/// </summary>
public class BouncyCactus : MonoBehaviour
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        animator.SetBool("Bounce", true);
        bounceTimeLeft = bounceWaitTime;
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(nameof(BouncePlayer));
        }
        Vector2 bounceForce = (collision.rigidbody.position - new Vector2(transform.position.x, transform.position.y)).normalized;
        collision.rigidbody.AddForce(bounceForce * bounceForceStrength, ForceMode2D.Impulse);
    }

    IEnumerator BouncePlayer()
    {
        GameManager.Instance.FreezePlayer(true);
        yield return new WaitForSeconds(bounceWaitTime);
        GameManager.Instance.FreezePlayer(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        animator.SetBool("Bounce", true);
        bounceTimeLeft = bounceWaitTime;
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
