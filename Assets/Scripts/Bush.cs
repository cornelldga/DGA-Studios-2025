using System.Collections;
using UnityEngine;
using UnityEngine.Animations;

public class Bush : MonoBehaviour
{

    [SerializeField] public bool isOnFire;
    [SerializeField] float damage;
    [SerializeField] float fireSpreadCooldown;
    [SerializeField] float fireSpreadRadius;
    private Coroutine fireCoroutine;
    [SerializeField] float witherDuration = 3f;
    [SerializeField] bool isWhipped; //can't be set on fire again
    private float deathAnimTime = .5f;

    [SerializeField] float whippedCooldown = 3f;
    private float witherTimer = 0f;
    private float whippedTimer = 0f;

    private GameObject ash;

    private float dukeY;

    private Animator animator;

    private SpriteRenderer sr;
    private SpriteRenderer ashSR;
    private SpriteRenderer plyrSR;

    private bool frontOfDuke;
    private bool frontOfAsh;
    //[Header("Numbers for bush layer rendering")]
    private float  dukeFootOffset = .3f;
    private float ashFootOffset = .7f;

    public void Start()
    {
        animator = GetComponent<Animator>();
        setFire(isOnFire);
        ash = GameObject.Find("Ash");
        sr = GetComponent<SpriteRenderer>();
        ashSR = ash.GetComponent<SpriteRenderer>();
        plyrSR = GameManager.Instance.player.GetComponent<SpriteRenderer>();
        frontOfDuke = false;
        frontOfAsh = false;

    }

    public void Update()
    {
        
        // start bush death animation
        if (witherDuration - witherTimer <= deathAnimTime && !animator.GetBool("isDying")) {
            animator.SetBool("isBurning", false);
            animator.SetBool("isDying", true);
        }

        if (witherTimer >= witherDuration) Destroy(gameObject);
        if (whippedTimer >= whippedCooldown)
        {
            setWhipped(false);
            whippedTimer = 0f;
        }
        if (isOnFire) witherTimer += Time.deltaTime;
      
        if (isWhipped) whippedTimer += Time.deltaTime;
    }

    /**
    isOnFire setter. 
    Changes sprite animation and starts/stops firespreading coroutine
    */
    public void setFire(bool isOnFire)
    {
        this.isOnFire = isOnFire;
        

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
    
        if (isOnFire) {
            witherTimer = 0f; //reset timer
            animator.SetBool("isBurning", true);

            fireCoroutine = StartCoroutine(fireSpreadRoutine());
        } else {
            animator.SetBool("isBurning", false);
            if (fireCoroutine != null)
                StopCoroutine(fireCoroutine);
        }
    }

    public bool isFire()
    {
        return isOnFire;
    }

/// <summary>
/// For every firespreadcooldown time that passes, the fire spreads from one bush to another within fire spreadradius.
/// </summary>
/// <param name="start"></param>
/// <param name="target"></param>
/// <returns></returns>
    public IEnumerator fireSpreadRoutine ()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0f, fireSpreadCooldown));
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, fireSpreadRadius);
            
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Bush") && collider.gameObject != this.gameObject)
                {
                    Bush bush = collider.GetComponent<Bush>();
                    if (!bush.isOnFire && !bush.isWhipped) bush.setFire(true);
                }
            }
            yield return null;
        }
    }

    /**
    If collides with player and is on fire, deal damage
    */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOnFire && collision.CompareTag("Player"))
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
    }
    
    /// <summary>
    /// Sets this projectile as 'whipped' to reverse collision logic
    /// and sets its damage based on the whipDamageMultiplier
    /// </summary>
    public void WhipBush()
    {
        whippedTimer = 0f;
        setFire(false);
        setWhipped(true);
    }

    public void setWhipped(bool whipped)
    {
        this.isWhipped = whipped;
    }
}