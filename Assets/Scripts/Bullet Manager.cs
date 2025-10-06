using UnityEditor.Build.Content;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BulletManager : MonoBehaviour {
    [Tooltip("Bullet/Bullet Pattern Being Fired")]
    [SerializeField] private GameObject bullet;

    [Tooltip("Time in Seconds between bullets being fired")]
    [SerializeField] private float bulletCooldown;

    [Tooltip("Timer to count seconds between bullets")]
    private float timer;

    public static BulletManager Instance;

    void Awake()
    {
        Instance = this;
        timer = 0f;
    }

    /*To be added:
     Switch cases for bullet patterns
     Bullet patterns may include homing attacks
     
     */

    void Update()
    {
        ///Set trans
        transform.position = new Vector2(Random.Range(-5,9), 5);
        timer += Time.deltaTime;
        if (timer > bulletCooldown)
        {
            Instantiate(bullet, transform.position, Quaternion.identity);
            timer = 0f;
        }
    }
    
}
