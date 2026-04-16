using Unity.VisualScripting;
using UnityEngine;

public class TumbleweedOrbit : Bullet, IDamageable
{


    [SerializeField] private float fireDamage = 5f;
    [SerializeField] private float fireDuration = 1f;
    [SerializeField] private float orbitingSpeed = 360f;
    [SerializeField] private int maxHealth;
    private int health;
    private bool isOnFire = false;
    private GameObject ash;


    public override void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ash = GameObject.Find("Ash");
        health=maxHealth;

   
    }


    private void FixedUpdate()

    {
        transform.RotateAround(ash.transform.position, Vector3.back, orbitingSpeed);
        transform.Rotate(Vector3.back * 360f * Time.deltaTime);
        if (isOnFire) Destroy(gameObject, fireDuration);
    }

    private void setFire()
    {
        isOnFire = true;
        damage = fireDamage;
        duration = fireDuration;
        //placeholder until animation set
        GetComponent<SpriteRenderer>().color = Color.red;
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        OnProjectileHit(collision);
    }

    public override void OnProjectileHit(Collider2D collision)
    {
        if (collision.CompareTag("Bush"))
        {
            Bush bush = collision.gameObject.GetComponent<Bush>();
            if (bush.isOnFire) setFire();

        }
        else if(collision.CompareTag("Whip")){
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Base"))
        {
            Destroy(collision.gameObject);
        }
        else
        {
            base.OnProjectileHit(collision);
        }


       
    }

    public void TakeDamage(float damage)
    {
        health --;
        if (health == 0)
        {
            Destroy(gameObject);
        }
    }
}
