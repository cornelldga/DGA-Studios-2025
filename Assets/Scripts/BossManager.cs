using UnityEngine;

public class BossManager : MonoBehaviour
{
    private int health;
    [SerializeField] private int startingHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = startingHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (health == 0)
        {
            Time.timeScale = 0;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("FriendlyDamage"))
        {
            health--;
        }
    }

    public int getHealth() { return health; }
    public void setHealth(int h) { health=h; }

}
