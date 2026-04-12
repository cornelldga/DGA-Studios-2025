using UnityEngine;

public class TutorialShooter : MonoBehaviour
{
    private bool canShoot;
    private float cooldown;

    [SerializeField] Bullet bullet;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canShoot = true;
        cooldown = 0;
    }

    // Update is called once per frame
    void Update()
    {
        cooldown -= Time.deltaTime;
        if (canShoot && cooldown <= 0)
        {
            Instantiate(bullet, gameObject.transform.position, Quaternion.identity);
            cooldown = 1;
        }
    }
}
