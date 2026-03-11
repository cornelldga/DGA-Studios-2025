using System.Collections;
using UnityEngine;

public class Bush : MonoBehaviour
{

    [SerializeField] bool isOnFire;
    [SerializeField] float damage;
    [SerializeField] float fireSpreadCooldown;
    [SerializeField] float fireSpreadRadius;
    private Coroutine fireCoroutine;
    [SerializeField] float witherDuration = 3f;
    private float witherTimer = 0f;


    public void Start()
    {
        setFire(isOnFire);
    }

    public void Update()
    {
        if (witherTimer >= witherDuration) Destroy(gameObject);
        if (isOnFire) witherTimer += Time.deltaTime;
    }

    /**
    isOnFire setter. 
    Changes sprite color and starts/stops firespreading coroutine
    */
    public void setFire(bool isOnFire)
    {
        this.isOnFire = isOnFire;
        Debug.Log(" " + isOnFire);
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
    
        if (isOnFire) {
            witherTimer = 0f; //reset timer
            spriteRenderer.color = Color.orange;
            fireCoroutine = StartCoroutine(fireSpreadRoutine());
        } else {
            spriteRenderer.color = Color.white;
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
        float fireSpreadCooldownTimer = 0;
        while (true)
        {
            if (fireSpreadCooldownTimer >= fireSpreadCooldown)
            {
                fireSpreadCooldownTimer = 0;
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, fireSpreadRadius);
                
                foreach (Collider2D collider in colliders)
                {
                    if (collider.CompareTag("Bush") && collider.gameObject != this.gameObject)
                    {

                        Bush bush = collider.GetComponent<Bush>();
                        if (!bush.isOnFire) bush.setFire(true);
                    }
                }
            }
            fireSpreadCooldownTimer += Time.deltaTime;
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
}