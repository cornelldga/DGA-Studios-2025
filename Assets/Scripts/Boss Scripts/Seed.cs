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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        planted = false;
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocityX = (target.x - this.transform.position.x) /landingTime;
        maxHeight = (arcHeight + target.y);
        startVel = maxHeight / (landingTime / 2) + arcHeight * (landingTime / 2);
        rb.linearVelocityY = startVel;

    }

    // Update is called once per frame
    void Update()
    {
        if (!planted) { 
            if(target.y > this.transform.position.y )
            {
                rb.linearVelocity = new Vector2(0, 0);
                //Plant seed when it reaches its target
                planted = true;
            }
        else { rb.linearVelocityY -= ((2*startVel) / landingTime )* Time.deltaTime; }
         }

    }

    /// <summary>
    /// When called, will spawn given plant then be destroyed
    /// </summary>
    public void Blossom()
    {
        //Spawn Flower
        GameObject.Destroy(this.gameObject);
    }

    public void Whipped()
    {
        GameObject.Destroy(this.gameObject);
    }
}
