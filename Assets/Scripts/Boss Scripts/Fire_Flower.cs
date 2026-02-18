using UnityEngine;

public class Fire_Flower : MonoBehaviour, IDamageable
{
    [SerializeField] private float rotationRate;
    [SerializeField] private int maxHealth;
    [SerializeField] private GameObject fireLine;
    private SpriteRenderer fireRenderer;
    private float timer;
    private float health;
    private float totalRotation;
    private int ogOrder;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 0;
        health = maxHealth;
        totalRotation = 0;
        fireRenderer = fireLine.GetComponent<SpriteRenderer>();
        ogOrder = fireRenderer.sortingOrder;
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
}
