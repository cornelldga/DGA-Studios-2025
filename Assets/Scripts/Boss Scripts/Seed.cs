using UnityEngine;

public class Seed : MonoBehaviour
{
    [Header("Targeting")]
    public Vector2 target;
    [SerializeField] float landingTime;
    [SerializeField] float arcHeight;
    [Header("Flower Variables")]
    public bool planted;
    [SerializeField] GameObject flower;
    private float maxHeight;
    private Rigidbody2D rb;
    private float startVel;
    private float timer;
    private float startHeight;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        planted = false;
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocityX = (target.x - this.transform.position.x) /landingTime;
        maxHeight = (arcHeight + target.y);
        startVel = maxHeight / (landingTime / 2);
        rb.linearVelocityY = startVel + (target.y - this.transform.position.y) / landingTime; ;
        timer = 0;
        startHeight = this.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (!planted) { 
            if( timer >= landingTime )
            {
                rb.linearVelocity = new Vector2(0, 0);
                //Plant seed when it reaches its target
                planted = true;
            }
        else { rb.linearVelocityY -= ((2*startVel)/landingTime) * Time.deltaTime; }
         }

    }

    /// <summary>
    /// When called, will spawn "flower" then destroy seed
    /// </summary>
    public void Blossom()
    {
        //Spawn Flower
        GameObject.Destroy(this.gameObject);
    }

 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Whip"))
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}
