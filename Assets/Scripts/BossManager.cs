using UnityEngine;

public class BossManager : MonoBehaviour
{
    private static int health;
    [SerializeField] private int startingHealth;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject healthBackDrop;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = startingHealth;
        healthBar.SetActive(true);
        healthBackDrop.SetActive(true);
        healthBackDrop.transform.localScale = new Vector3(startingHealth,1,1);
        healthBar.transform.localScale = new Vector3(startingHealth, 1, 1);

    }

    // Update is called once per frame
    void Update()
    {
        healthBar.transform.localScale = new Vector3(health, 1, 1);
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
