using Unity.VisualScripting;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float bulletSpeed;
    public float vertical;
    public float horizontal;
    public float bulletLife;
    [SerializeField] private Rigidbody2D rb;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bulletLife -= Time.deltaTime;
        if (bulletLife <= 0 )
        {
            Destroy(this.gameObject);
        }

    }
    private void FixedUpdate()
    {
       rb.linearVelocity = new Vector2(bulletSpeed * horizontal * Time.deltaTime, bulletSpeed * vertical * Time.deltaTime);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Destroy(this.gameObject);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(this.gameObject);
        }
    }
}
/* 
 To do in file:
 
 */
