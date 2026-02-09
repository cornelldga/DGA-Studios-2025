using UnityEngine;

public class Cactus : MonoBehaviour, IDamageable
{
    [SerializeField] private float fireRate;
    [SerializeField] private Bullet spine;
    [SerializeField] private int spineCount;
    [SerializeField] private int maxHealth;
    private float timer;
    private float health;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 0;
        health=maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > fireRate)
        {
            timer = 0;
            SpineAttack();
        }
    }

    private void SpineAttack()
    {
   
        for (int i = 0; i < spineCount; i++)
        {
            Bullet bullet = Instantiate(spine, this.transform.position, this.transform.rotation);
            bullet.transform.Rotate(0, 0, (360/spineCount) * i, Space.Self);
        }
    }
    public void TakeDamage(float damage)
    {
        health --;
        if (health == 0)
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}
