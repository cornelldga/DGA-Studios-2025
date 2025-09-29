using UnityEngine;

public class BossManager : MonoBehaviour
{
    private int health;
    [SerializeField] private int startingHealth;
    [SerializeField] private Transform healthBar;
    [SerializeField] private Transform backdrop;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = startingHealth;
        healthBar.localScale.Set(health,1,1);
        backdrop.localScale.Set(health, 1, 1);
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
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            health--;
        }
    }

    public int getHealth() { return health; }
    public void setHealth(int h) { health=h; }

}
