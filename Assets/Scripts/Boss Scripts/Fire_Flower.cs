using UnityEngine;

public class Fire_Flower : MonoBehaviour, IDamageable
{
    [SerializeField] private float rotationRate;
    [SerializeField] private int maxHealth;
    [SerializeField] private GameObject fireLine;
    [SerializeField] private int damage;
    private SpriteRenderer fireRenderer;
    private float timer;
    private float health;
    private float totalRotation;
    private int ogOrder;

    [HideInInspector]
    public int locationID = -1;
    private Ash ash;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 0;
        health = maxHealth;
        totalRotation = 0;
        fireRenderer = fireLine.GetComponent<SpriteRenderer>();
        fireLine.GetComponent<Spine>().damage = damage;
        ogOrder = fireRenderer.sortingOrder;
        ash = FindAnyObjectByType<Ash>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        totalRotation += rotationRate * Time.deltaTime;
        totalRotation = totalRotation % 360;
        fireLine.transform.RotateAround(this.transform.position,Vector3.forward, rotationRate * Time.deltaTime);

        if (totalRotation <90 || totalRotation > 270)
        {
            fireRenderer.sortingOrder = ogOrder;
        }
        else
        {
            fireRenderer.sortingOrder = ogOrder + 1;
        }
    }

    public void TakeDamage(float damage)
    {
        health--;
        if (health == 0)
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
    }

    private void OnDestroy()
    {
        
        ash.deployedSeeds[locationID] = false;
    }

}
