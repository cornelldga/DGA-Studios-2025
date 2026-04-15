using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class Cactus : MonoBehaviour, IDamageable
{
    [SerializeField] private float fireRate;
    [SerializeField] private GameObject spine;
    [SerializeField] private int spineCount;
    [SerializeField] private int maxHealth;
    private float timer;
    private float health;
    [HideInInspector]
    public int locationID = -1;
    private Ash ash;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 0;
        health=maxHealth;
        ash = FindAnyObjectByType<Ash>();
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
            GameObject bullet = Instantiate(spine, this.transform.position, this.transform.rotation);
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

    private void OnDestroy()
    {
        ash.deployedSeeds[locationID] = false;
    }
}
