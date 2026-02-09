using UnityEngine;

public class Seed : MonoBehaviour
{
    [Header("Targeting")]
    public Vector2 target;
    [SerializeField] float landingTime;
    [SerializeField] float arcHeight;
    [SerializeField] float gravity;
    [Header("Flower Variables")]
    public bool planted;
    [SerializeField] GameObject flower;
    private float maxHeight;
    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        planted = false;
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocityX = (target.x - this.transform.position.x) /landingTime;
        maxHeight = (arcHeight + target.y);
        rb.linearVelocityY = maxHeight/(landingTime/2) + gravity * landingTime;

    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocityY -= gravity *Time.deltaTime;
        if(target.y > this.transform.position.y & !planted)
        {
            gravity = 0;
            rb.linearVelocity = new Vector2(0, 0);
            //Plant seed when it reaches its target
            planted = true;
            
        }
        if(this.transform.position.y >= maxHeight & rb.linearVelocityY>=0)
        {
            rb.linearVelocityY = -rb.linearVelocity.y;
        }
    }

    private void FixedUpdate()
    {
        
    }

    public void Blossom()
    {
        //Spawn Flower
        GameObject.Destroy(this.gameObject);
    }


}
