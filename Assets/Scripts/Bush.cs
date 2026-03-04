using System.Collections;
using UnityEngine;

public class Bush : MonoBehaviour
{

    [SerializeField] bool isOnFire;
    [SerializeField] float damage;
    [SerializeField] float fireSpreadCooldown;
    [SerializeField] float fireSpreadRadius;
    private Coroutine fireCoroutine;


    public void Start()
    {
        setFire(isOnFire);
    }

    public void setFire(bool isOnFire)
    {
        this.isOnFire = isOnFire;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
    
        if (isOnFire) {
            spriteRenderer.color = Color.orange;
            fireCoroutine = StartCoroutine(fireSpreadRoutine());
        } else {
            spriteRenderer.color = Color.white;
            if (fireCoroutine != null)
                StopCoroutine(fireCoroutine);
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOnFire && collision.CompareTag("Player"))
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
    }
}