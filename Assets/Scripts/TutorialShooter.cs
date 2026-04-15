using UnityEngine;

public class TutorialShooter : MonoBehaviour
{
    [SerializeField] float cooldown;

    private bool canShoot;
    private float cooldownTime;

    [SerializeField] Bullet bullet;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canShoot = true;
        cooldownTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        cooldownTime -= Time.deltaTime;
        if (canShoot && cooldownTime <= 0)
        {
            Instantiate(bullet, gameObject.transform.position, Quaternion.identity);
            cooldownTime = cooldown;
        }
    }
}
