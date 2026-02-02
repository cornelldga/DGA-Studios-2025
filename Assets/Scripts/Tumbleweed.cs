using UnityEngine;

public class Tumbleweed : MonoBehaviour 
{
    // once done
    // [SerializeField] Ash ash;


    // initialization
    private bool isInitialized = false;
    private bool orbitState = false;


    private Rigidbody2D rb;
    private CircleCollider2D thisCollider;
    private Animator animator;
    

    // Movement
    private float moveDirectionX = 1f; // -1/1, left to right 

    [SerializeField] private float damageDealt = 1f; // fix this
    public enum State
    {
        Orbiting, 
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        thisCollider = GetComponent<CircleCollider2D>();
       
        
    }

    // <summary>
    // Initializes tumbleweed with delay (time for Ash to point)
    // </summary>
    private System.Collections.IEnumerator InitializeWithDelay(float delay, bool state)
    {
        yield return new WaitForSeconds(delay);
        isInitialized = true;
        orbitState = state;
    }

    private void Update()
    {
        // do nothing if not initialized
        if (!isInitialized)
        {
            return;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bool isPlayer = collision.gameObject.CompareTag("Player"); // make sure not whip
        bool isBullet = collision.gameObject.CompareTag("Bullet"); // check that this tag actually exists
        bool isWhip = false; // figure this one out b/c it's on playertag


        if (isPlayer) {
            // damage the player somehow idk figure it out
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
            Destroy(gameObject);
        }
        if (isBullet) {
            // get rid of tumbleweed
            //Destroy();
        }
    }
}
